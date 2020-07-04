using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>(grouped) alternating inference chain</b> (<b>(grouped) AIC</b>) 
	/// or <b>(grouped) continuous nice loop</b> (<b>(grouped) CNL</b>) technique searcher.
	/// </summary>
	/// <remarks>
	/// I want to use BFS (breadth-first searching) to search for chains, which can avoid
	/// the redundant backtracking.
	/// </remarks>
	[TechniqueDisplay(nameof(TechniqueCode.Aic))]
	public sealed class AicTechniqueSearcher : ChainingTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 46;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var tempAccumulator = new Bag<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, grid, true, false);
			GetAll(tempAccumulator, grid, false, true);
			GetAll(tempAccumulator, grid, true, true);

			tempAccumulator.Sort((i1, i2) =>
			{
				decimal d1 = i1.Difficulty, d2 = i2.Difficulty;
				if (d1 < d2)
				{
					return -1;
				}
				else if (d1 > d2)
				{
					return 1;
				}
				else
				{
					int l1 = i1.Complexity;
					int l2 = i2.Complexity;
					return l1 == l2 ? i1.SortKey - i2.SortKey : l1 - l2;
				}
			});

			accumulator.AddRange(tempAccumulator);
		}


		/// <summary>
		/// Search for chains of each type.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">Thr grid.</param>
		/// <param name="xEnabled">
		/// Indicates whether the strong links in regions are enabled to search for.
		/// </param>
		/// <param name="yEnabled">
		/// Indicates whether the strong links in cells are enabled to search for.
		/// </param>
		private void GetAll(IBag<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, bool xEnabled, bool yEnabled)
		{
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidateMask(cell);
				if (mask.CountSet() >= 2)
				{
					// Iterate on all candidates that aren't alone.
					foreach (int digit in mask.GetAllSets())
					{
						var pOn = new Node(digit, cell, true);
						DoUnaryChaining(accumulator, grid, pOn, xEnabled, yEnabled);
					}
				}
			}
		}

		/// <summary>
		/// Do unary chaining.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">Thr grid.</param>
		/// <param name="pOn">The node set on.</param>
		/// <param name="xEnabled">
		/// Indicates whether the strong links in regions are enabled to search for.
		/// </param>
		/// <param name="yEnabled">
		/// Indicates whether the strong links in cells are enabled to search for.
		/// </param>
		private void DoUnaryChaining(
			IBag<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, Node pOn, bool xEnabled, bool yEnabled)
		{
			if (grid.GetCandidateMask(pOn._cell).CountSet() > 2 && !xEnabled)
			{
				// Y-Chains can only start with the bivalue cell.
				return;
			}

			var loops = new List<Node>();
			var chains = new List<Node>();
			var onToOn = new HashSet<Node> { pOn };
			var onToOff = new HashSet<Node>();
			DoLoops(grid, onToOn, onToOff, xEnabled, yEnabled, loops, pOn);

			if (xEnabled)
			{
				// Y-Chains don't exist (length must be both odd and even).

				// AICs with off implication.
				onToOn = new HashSet<Node> { pOn };
				onToOff = new HashSet<Node>();
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOn);

				// AICs with on implication.
				var pOff = new Node(pOn.Digit, pOn._cell, false);
				onToOn = new HashSet<Node>();
				onToOff = new HashSet<Node> { pOff };
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOff);
			}

			foreach (var destOn in loops)
			{
				var destOff = GetReversedLoop(destOn);
				var result = CreateLoopHint(grid, destOn, destOff, xEnabled, yEnabled);
				if (!(result is null))
				{
					accumulator.Add(result);
				}
			}
			foreach (var target in chains)
			{
				var result = CreateAicHint(grid, target, xEnabled, yEnabled);
				if (!(result is null))
				{
					accumulator.Add(result);
				}
			}
		}

		private unsafe ChainingTechniqueInfo? CreateLoopHint(
			IReadOnlyGrid grid, Node destOn, Node destOff, bool xEnabled, bool yEnabled)
		{
			var cells = GridMap.Empty;
			for (var p = &destOn; p->ParentsCount != 0; p = p->Parents[0])
			{
				cells.Add(p->_cell);
			}

			var cancelFore = new HashSet<Node>();
			var cancelBack = new HashSet<Node>();
			for (var p = &destOn; p->ParentsCount != 0; p = p->Parents[0])
			{
				foreach (int cell in PeerMaps[p->_cell])
				{
					if (!cells[cell] && grid.Exists(cell, p->Digit) is true)
					{
						(p->IsOn ? ref cancelFore : ref cancelBack).Add(new Node(p->Digit, cell, false));
					}
				}
			}

			cancelFore.IntersectWith(cancelBack);
			var conclusions = new List<Conclusion>();
			foreach (var node in cancelFore)
			{
				conclusions.Add(new Conclusion(Elimination, node._cell, node.Digit));
			}

			return conclusions.Count == 0
				? null
				: new LoopTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets: GetCandidateOffsets(destOn),
							regionOffsets: null,
							links: GetLinks(destOn))
					},
					xEnabled,
					yEnabled,
					target: destOn);
		}

		private ChainingTechniqueInfo? CreateAicHint(IReadOnlyGrid grid, Node target, bool xEnabled, bool yEnabled)
		{
			var conclusions = new List<Conclusion>();
			if (!target.IsOn)
			{
				conclusions.Add(new Conclusion(Elimination, target._cell, target.Digit));
			}
			else
			{
				foreach (int digit in grid.GetCandidates(target._cell))
				{
					if (digit != target.Digit)
					{
						conclusions.Add(new Conclusion(Elimination, target._cell, digit));
					}
				}
			}

			return conclusions.Count == 0
				? null
				: new AicTechniqueInfo(
					conclusions,
					views: new[]
					{
						new View(
							cellOffsets: null,
							candidateOffsets: GetCandidateOffsets(target),
							regionOffsets: null,
							links: GetLinks(target))
					},
					xEnabled,
					yEnabled,
					target);
		}

		private unsafe Node GetReversedLoop(Node destOn)
		{
			var result = new List<Node>();
			for (Node? org = destOn; org.HasValue;)
			{
				var o = org.Value;
				var rev = new Node(o.Digit, o._cell, !o.IsOn);
				result.Prepend(rev);
				org = o.ParentsCount != 0 ? (Node?)*o.Parents[0] : null;
			}

			Node? prev = null;
			foreach (var rev in result)
			{
				if (prev.HasValue)
				{
					prev.Value.AddParent(&rev);
				}

				prev = rev;
			}

			return result[0];
		}

		private unsafe ISet<Node> GetOnToOff(IReadOnlyGrid grid, Node p, bool yEnabled)
		{
			var result = new HashSet<Node>();

			if (yEnabled)
			{
				// First rule: Other candidates for this cell get off.
				for (int digit = 0; digit < 9; digit++)
				{
					if (digit != p.Digit && grid.Exists(p._cell, p.Digit) is true)
					{
						result.Add(new Node(p.Digit, p._cell, false, &p));
					}
				}
			}

			// Second rule: Other positions for this digit get off.
			for (var label = Block; label < UpperLimit; label++)
			{
				int region = GetRegion(p._cell, label);
				for (int pos = 0; pos < 9; pos++)
				{
					int cell = RegionCells[region][pos];
					if (cell != p._cell && grid.Exists(cell, p.Digit) is true)
					{
						result.Add(new Node(p.Digit, cell, false, &p));
					}
				}
			}

			return result;
		}

		private unsafe ISet<Node> GetOffToOn(
			IReadOnlyGrid grid, Node p, ISet<Node> offNodes, bool xEnabled, bool yEnabled)
		{
			var result = new HashSet<Node>();
			if (yEnabled)
			{
				// First rule: If there's only two candidates in this cell, the other one gets on.
				if (BivalueMap[p._cell])
				{
					short mask = grid.GetCandidateMask(p._cell);
					int otherDigit = mask.FindFirstSet();
					if (otherDigit == p.Digit)
					{
						otherDigit = mask.GetNextSet(otherDigit);
					}

					var pOn = new Node(otherDigit, p._cell, true, &p);
					AddHiddenParentsOfCell(pOn, grid, offNodes);
					result.Add(pOn);
				}
			}

			if (xEnabled)
			{
				// Second rule: If there's only two positions for this candidate, the other ont gets on.
				for (var label = Block; label < UpperLimit; label++)
				{
					int region = GetRegion(p._cell, label);
					var cells = CandMaps[p.Digit] & RegionMaps[region];
					if (cells.Count == 2)
					{
						int otherPos = cells.SetAt(0);
						if (otherPos == p._cell)
						{
							otherPos = cells.SetAt(1);
						}

						var pOn = new Node(p.Digit, otherPos, true, &p);
						AddHiddenParentsOfRegion(pOn, region, offNodes);
						result.Add(pOn);
					}
				}
			}

			return result;
		}

		private unsafe void AddHiddenParentsOfRegion(Node p, int region, ISet<Node> offNodes)
		{
			foreach (int cell in CandMaps[p.Digit] & RegionMaps[region])
			{
				var parent = new Node(p.Digit, cell, false);
				if (offNodes.Contains(parent))
				{
					p.AddParent(&parent);
				}
				else
				{
					throw new SudokuRuntimeException("The specified parent cannot found.");
				}
			}
		}

		private unsafe void AddHiddenParentsOfCell(Node p, IReadOnlyGrid grid, ISet<Node> offNodes)
		{
			foreach (int digit in grid.GetCandidates(p._cell))
			{
				var parent = new Node(digit, p._cell, false);
				if (offNodes.Contains(parent))
				{
					p.AddParent(&parent);
				}
				else
				{
					throw new SudokuRuntimeException("The specified parent cannot found.");
				}
			}
		}

		private void DoAic(
			IReadOnlyGrid grid, ISet<Node> onToOn, ISet<Node> onToOff, bool yEnabled,
			IList<Node> chains, Node source)
		{
			var pendingOn = new List<Node>(onToOn);
			var pendingOff = new List<Node>(onToOff);

			while (pendingOn.Count != 0 || pendingOff.Count != 0)
			{
				while (pendingOn.Count != 0)
				{
					var p = pendingOn[0];
					pendingOn.RemoveAt(0);

					var makeOff = GetOnToOff(grid, p, yEnabled);
					foreach (var pOff in makeOff)
					{
						var pOn = new Node(pOff.Digit, pOff._cell, true);
						if (source == pOn)
						{
							// Loopy contradiction (AIC) found.
							chains.AddIfDoesNotContain(pOff);
						}

						if (p.IsParentOf(pOff) && !onToOff.Contains(pOff))
						{
							// Not processed yet.
							pendingOff.Add(pOff);
							onToOff.Add(pOff);
						}
					}
				}

				while (pendingOff.Count != 0)
				{
					var p = pendingOff[0];
					pendingOff.RemoveAt(0);
					var makeOn = GetOffToOn(grid, p, onToOff, true, yEnabled);
					foreach (var pOn in makeOn)
					{
						var pOff = new Node(pOn.Digit, pOn._cell, false);
						if (source == pOff)
						{
							// Loopy contradiction (AIC) found.
							chains.AddIfDoesNotContain(pOn);
						}

						if (!onToOn.Contains(pOn))
						{
							// Not processed yet.
							pendingOn.Add(pOn);
							onToOn.Add(pOn);
						}
					}
				}
			}
		}

		private void DoLoops(
			IReadOnlyGrid grid, ISet<Node> onToOn, ISet<Node> onToOff, bool xEnabled, bool yEnabled,
			IList<Node> loops, Node source)
		{
			var pendingOn = new List<Node>(onToOn);
			var pendingOff = new List<Node>(onToOff);

			for (int length = 0; pendingOn.Count != 0 || pendingOff.Count != 0;)
			{
				length++;
				while (pendingOn.Count != 0)
				{
					var p = pendingOn[0];
					pendingOn.RemoveAt(0);

					var makeOff = GetOnToOff(grid, p, yEnabled);
					foreach (var pOff in makeOff)
					{
						// Not processed yet.
						pendingOff.Add(pOff);
						onToOff.Add(pOff);
					}
				}

				length++;
				while (pendingOff.Count != 0)
				{
					var p = pendingOff[0];
					pendingOff.RemoveAt(0);
					var makeOn = GetOffToOn(grid, p, onToOff, xEnabled, yEnabled);
					foreach (var pOn in makeOn)
					{
						if (length >= 4 && pOn == source)
						{
							// Loop found.
							loops.Add(pOn);
						}

						if (!onToOn.Contains(pOn))
						{
							// Not processed yet.
							pendingOn.Add(pOn);
							onToOn.Add(pOn);
						}
					}
				}
			}
		}
	}
}
