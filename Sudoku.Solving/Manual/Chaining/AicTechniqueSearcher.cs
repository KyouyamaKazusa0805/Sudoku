using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
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
	[SearcherProperty(46)]
	public sealed class AicTechniqueSearcher : ChainingTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var tempAccumulator = new List<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, grid, true, false);
			GetAll(tempAccumulator, grid, false, true);
			GetAll(tempAccumulator, grid, true, true);

			if (tempAccumulator.Count == 0)
			{
				return;
			}

			var set = new Set<ChainingTechniqueInfo>(tempAccumulator);
			set.Sort((i1, i2) =>
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

			accumulator.AddRange(set);
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
		private void GetAll(IList<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, bool xEnabled, bool yEnabled)
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
			IList<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, Node pOn, bool xEnabled, bool yEnabled)
		{
			if (grid.GetCandidateMask(pOn.Cell).CountSet() > 2 && !xEnabled)
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
				var pOff = new Node(pOn.Cell, pOn.Digit, false);
				onToOn = new Set<Node>();
				onToOff = new Set<Node> { pOff };
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOff);
			}

			foreach (var destOn in loops)
			{
				if ((destOn.Chain.Count & 1) == 1)
				{
					// Odd length.
					continue;
				}

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

		/// <summary>
		/// Create a loop hint (i.e. a <see cref="LoopTechniqueInfo"/>).
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="destOn">The start node.</param>
		/// <param name="xEnabled">Indicates whether X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether Y-Chains are enabled.</param>
		/// <returns>
		/// If the number of conclusions are not zero (in other words, if worth), the information
		/// will be returned; otherwise, <see langword="null"/>.
		/// </returns>
		/// <seealso cref="LoopTechniqueInfo"/>
		private ChainingTechniqueInfo? CreateLoopHint(IReadOnlyGrid grid, Node destOn, bool xEnabled, bool yEnabled)
		{
			var conclusions = new List<Conclusion>();
			var links = GetLinks(destOn, true); //! Maybe wrong when adding grouped nodes.
			foreach (var (start, end, type) in links)
			{
				if (type == LinkType.Weak)
				{
					var elimMap = SudokuMap.CreateInstance(stackalloc[] { start, end });
					if (elimMap.IsEmpty)
					{
						continue;
					}

					foreach (int candidate in elimMap)
					{
						if (grid.Exists(candidate / 9, candidate % 9) is true)
						{
							conclusions.Add(new Conclusion(Elimination, candidate));
						}
					}
				}
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
							links)
					},
					xEnabled,
					yEnabled,
					target: destOn);
		}

		/// <summary>
		/// Create an AIC hint (i.e. a <see cref="AicTechniqueInfo"/>).
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="target">The elimination node (which is used for searching the whole chain).</param>
		/// <param name="xEnabled">Indicates whether X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether Y-Chains are enabled.</param>
		/// <returns>
		/// If the number of conclusions are not zero (in other words, if worth), the information
		/// will be returned; otherwise, <see langword="null"/>.
		/// </returns>
		/// <seealso cref="AicTechniqueInfo"/>
		private ChainingTechniqueInfo? CreateAicHint(IReadOnlyGrid grid, Node target, bool xEnabled, bool yEnabled)
		{
			var conclusions = new List<Conclusion>();
			if (!target.IsOn)
			{
				// Get eliminations as an AIC.
				var startNode = target.Chain[1];
				int startCandidate = startNode.Cell * 9 + startNode.Digit;
				var endNode = target.Chain[^2];
				int endCandidate = endNode.Cell * 9 + endNode.Digit;
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

		/// <summary>
		/// Simulate the passing strong and weak links in AICs.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="onToOn">The nodes that the end candidates are currently on.</param>
		/// <param name="onToOff">The nodes the end candidates are currently off.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <param name="chains">The chain nodes.</param>
		/// <param name="source">The source node.</param>
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
						var pOn = new Node(pOff.Cell, pOff.Digit, true);
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
						var pOff = new Node(pOn.Cell, pOn.Digit, false);
						if (source == pOff)
						{
							// Loopy contradiction (AIC) found.
							chains.AddIfDoesNotContain(pOn);
						}

						if (!pOff.IsParentOf(p) && !onToOn.Contains(pOn))
						{
							// Not processed yet.
							pendingOn.Add(pOn);
							onToOn.Add(pOn);
						}
					}
				}
			}
		}

		/// <summary>
		/// Simulate the passing strong and weak links in CNLs.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="onToOn">The nodes that the end candidates are currently on.</param>
		/// <param name="onToOff">The nodes the end candidates are currently off.</param>
		/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <param name="loops">The loop nodes.</param>
		/// <param name="source">The source node.</param>
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
