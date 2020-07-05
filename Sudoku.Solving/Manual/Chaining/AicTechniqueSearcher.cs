using System.Collections.Generic;
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

			if (tempAccumulator.Count == 0)
			{
				return;
			}

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
						var pOn = new Node(cell, digit, true);
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
			var onToOn = new Set<Node> { pOn };
			var onToOff = new Set<Node>();
			DoLoops(grid, onToOn, onToOff, xEnabled, yEnabled, loops, pOn);

			if (xEnabled)
			{
				// Y-Chains don't exist (length must be both odd and even).

				// AICs with off implication.
				onToOn = new Set<Node> { pOn };
				onToOff = new Set<Node>();
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOn);

				// AICs with on implication.
				var pOff = new Node(pOn._cell, pOn.Digit, false);
				onToOn = new Set<Node>();
				onToOff = new Set<Node> { pOff };
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOff);
			}

			foreach (var destOn in loops)
			{
				var result = CreateLoopHint(grid, destOn, xEnabled, yEnabled);
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

		private ChainingTechniqueInfo? CreateLoopHint(IReadOnlyGrid grid, Node destOn, bool xEnabled, bool yEnabled)
		{
			var cells = GridMap.Empty;
			for (var p = destOn; p.ParentsCount != 0; p = p[0])
			{
				cells.Add(p._cell);
			}

			var cancelFore = new Set<Node>();
			var cancelBack = new Set<Node>();
			for (var p = destOn; p.ParentsCount != 0; p = p[0])
			{
				foreach (int cell in PeerMaps[p._cell])
				{
					if (!cells[cell] && grid.Exists(cell, p.Digit) is true)
					{
						(p.IsOn ? ref cancelFore : ref cancelBack).Add(new Node(cell, p.Digit, false));
					}
				}
			}

			cancelFore &= cancelBack;
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
				// Get eliminations as an AIC.
				var startNode = target.Chain[1];
				int startCandidate = startNode._cell * 9 + startNode.Digit;
				var endNode = target.Chain[^2];
				int endCandidate = endNode._cell * 9 + endNode.Digit;
				var elimMap = SudokuMap.CreateInstance(stackalloc[] { startCandidate, endCandidate });
				if (elimMap.IsEmpty)
				{
					return null;
				}

				foreach (int candidate in elimMap)
				{
					if (grid.Exists(candidate / 9, candidate % 9) is true)
					{
						conclusions.Add(new Conclusion(Elimination, candidate));
					}
				}

				//conclusions.Add(new Conclusion(Elimination, startCandidate));
			}
			//else
			//{
			//	conclusions.Add(new Conclusion(Assignment, target._cell, target.Digit));
			//}

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

		private ISet<Node> GetOnToOff(IReadOnlyGrid grid, Node p, bool yEnabled)
		{
			var result = new Set<Node>();

			if (yEnabled)
			{
				// First rule: Other candidates for this cell get off.
				for (int digit = 0; digit < 9; digit++)
				{
					if (digit != p.Digit && grid.Exists(p._cell, digit) is true)
					{
						result.Add(new Node(p._cell, digit, false, p));
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
						result.Add(new Node(cell, p.Digit, false, p));
					}
				}
			}

			return result;
		}

		private ISet<Node> GetOffToOn(IReadOnlyGrid grid, Node p, bool xEnabled, bool yEnabled)
		{
			var result = new Set<Node>();
			if (yEnabled)
			{
				// First rule: If there's only two candidates in this cell, the other one gets on.
				if (BivalueMap[p._cell])
				{
					short mask = (short)(grid.GetCandidateMask(p._cell) & ~(1 << p.Digit));
					if (mask.IsPowerOfTwo())
					{
						var pOn = new Node(p._cell, mask.FindFirstSet(), true, p);
						//AddHiddenParentsOfCell(pOn, grid, offNodes);
						result.Add(pOn);
					}
				}
			}

			if (xEnabled)
			{
				// Second rule: If there's only two positions for this candidate, the other ont gets on.
				for (var label = Block; label < UpperLimit; label++)
				{
					int region = GetRegion(p._cell, label);
					var cells = (CandMaps[p.Digit] & RegionMaps[region]) - p._cell;
					if (cells.Count == 1)
					{
						var pOn = new Node(cells.SetAt(0), p.Digit, true, p);
						//AddHiddenParentsOfRegion(pOn, region, offNodes);
						result.Add(pOn);
					}
				}
			}

			return result;
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
					var p = pendingOn[^1];
					pendingOn.RemoveLastElement();

					var makeOff = GetOnToOff(grid, p, yEnabled);
					foreach (var pOff in makeOff)
					{
						var pOn = new Node(pOff._cell, pOff.Digit, true);
						if (source == pOn)
						{
							// Loopy contradiction (AIC) found.
							chains.AddIfDoesNotContain(pOff);
						}

						if (!onToOff.Contains(pOff))
						{
							// Not processed yet.
							pendingOff.Add(pOff);
							onToOff.Add(pOff);
						}
					}
				}

				while (pendingOff.Count != 0)
				{
					var p = pendingOff[^1];
					pendingOff.RemoveLastElement();

					var makeOn = GetOffToOn(grid, p, true, yEnabled);
					foreach (var pOn in makeOn)
					{
						var pOff = new Node(pOn._cell, pOn.Digit, false);
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

			int length = 0;
			while (pendingOn.Count != 0 || pendingOff.Count != 0)
			{
				length++;
				while (pendingOn.Count != 0)
				{
					var p = pendingOn[^1];
					pendingOn.RemoveLastElement();

					var makeOff = GetOnToOff(grid, p, yEnabled);
					foreach (var pOff in makeOff)
					{
						// Not processed yet.
						pendingOff.AddIfDoesNotContain(pOff);
						onToOff.Add(pOff);
					}
				}

				length++;
				while (pendingOff.Count != 0)
				{
					var p = pendingOff[^1];
					pendingOff.RemoveLastElement();

					var makeOn = GetOffToOn(grid, p, xEnabled, yEnabled);
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
							pendingOn.AddIfDoesNotContain(pOn);
							onToOn.Add(pOn);
						}
					}
				}
			}
		}
	}
}
