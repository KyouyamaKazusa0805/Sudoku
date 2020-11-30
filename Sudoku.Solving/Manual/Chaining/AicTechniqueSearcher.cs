using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Data.ConclusionType;

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
	[DisplayLevel(2)]
	[TechniqueDisplay(nameof(TechniqueCode.Aic))]
	public sealed class AicTechniqueSearcher : ChainingTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(46);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			var tempAccumulator = new List<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, grid, true, false);
			GetAll(tempAccumulator, grid, false, true);
			GetAll(tempAccumulator, grid, true, true);

			if (tempAccumulator.Count == 0)
			{
				return;
			}

			accumulator.AddRange(SortInfo(tempAccumulator));
		}

		/// <summary>
		/// Search for chains of each type.
		/// </summary>
		/// <param name="accumulator">(<see langword="in"/> parameter) The accumulator.</param>
		/// <param name="grid">Thr grid.</param>
		/// <param name="xEnabled">
		/// Indicates whether the strong links in regions are enabled to search for.
		/// </param>
		/// <param name="yEnabled">
		/// Indicates whether the strong links in cells are enabled to search for.
		/// </param>
		private void GetAll(
			IList<ChainingTechniqueInfo> accumulator, in SudokuGrid grid, bool xEnabled, bool yEnabled)
		{
			foreach (int cell in EmptyMap)
			{
				if (grid.GetCandidateMask(cell) is var mask && mask.PopCount() >= 2)
				{
					// Iterate on all candidates that aren't alone.
					foreach (int digit in mask)
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
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pOn">(<see langword="in"/> parameter) The node set on.</param>
		/// <param name="xEnabled">
		/// Indicates whether the strong links in regions are enabled to search for.
		/// </param>
		/// <param name="yEnabled">
		/// Indicates whether the strong links in cells are enabled to search for.
		/// </param>
		private void DoUnaryChaining(
			IList<ChainingTechniqueInfo> accumulator, in SudokuGrid grid, in Node pOn,
			bool xEnabled, bool yEnabled)
		{
			if (grid.GetCandidateMask(pOn.Cell).PopCount() > 2 && !xEnabled)
			{
				// Y-Chains can only start with the bivalue cell.
				return;
			}

			List<Node> loops = new(), chains = new();
			Set<Node> onToOn = new() { pOn }, onToOff = new();
			DoLoops(grid, onToOn, onToOff, xEnabled, yEnabled, loops, pOn);

			if (xEnabled)
			{
				// Y-Chains don't exist (length must be both odd and even).

				// AICs with off implication.
				onToOn = new() { pOn };
				onToOff = new();
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOn);

				// AICs with on implication.
				var pOff = new Node(pOn.Cell, pOn.Digit, false);
				onToOn = new();
				onToOff = new() { pOff };
				DoAic(grid, onToOn, onToOff, yEnabled, chains, pOff);
			}

			foreach (var destOn in loops)
			{
				if ((destOn.Chain.Count & 1) != 1
					&& CreateLoopHint(grid, destOn, xEnabled, yEnabled) is var result and not null)
				{
					accumulator.Add(result);
				}
			}
			foreach (var target in chains)
			{
				if (CreateAicHint(grid, target, xEnabled, yEnabled) is var result and not null)
				{
					accumulator.Add(result);
				}
			}
		}

		/// <summary>
		/// Create a loop hint (i.e. a <see cref="LoopTechniqueInfo"/>).
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="destOn">(<see langword="in"/> parameter) The start node.</param>
		/// <param name="xEnabled">Indicates whether X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether Y-Chains are enabled.</param>
		/// <returns>
		/// If the number of conclusions are not zero (in other words, if worth), the information
		/// will be returned; otherwise, <see langword="null"/>.
		/// </returns>
		/// <seealso cref="LoopTechniqueInfo"/>
		private LoopTechniqueInfo? CreateLoopHint(
			in SudokuGrid grid, in Node destOn, bool xEnabled, bool yEnabled)
		{
			var conclusions = new List<Conclusion>();
			var links = destOn.GetLinks(true); //! Maybe wrong when adding grouped nodes.
			foreach (var (start, end, type) in links)
			{
				if (type == LinkType.Weak
					&& new SudokuMap { start, end }.PeerIntersection is { IsNotEmpty: true } elimMap)
				{
					foreach (int candidate in elimMap)
					{
						if (grid.Exists(candidate / 9, candidate % 9) is true)
						{
							conclusions.Add(new(Elimination, candidate));
						}
					}
				}
			}

			if (conclusions.Count == 0)
			{
				return null;
			}

			var candidateOffsets = destOn.GetCandidateOffsets(simpleChain: true);

			var (destCandidate, _) = destOn.Chain[^1];
			candidateOffsets.Add(new(0, destCandidate));

			return new(
				conclusions,
				new View[] { new(null, candidateOffsets.AsReadOnlyList(), null, links) },
				xEnabled,
				yEnabled,
				destOn);
		}

		/// <summary>
		/// Create an AIC hint (i.e. a <see cref="AicTechniqueInfo"/>).
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="target">(<see langword="in"/> parameter) The elimination node (which is used for searching the whole chain).</param>
		/// <param name="xEnabled">Indicates whether X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether Y-Chains are enabled.</param>
		/// <returns>
		/// If the number of conclusions are not zero (in other words, if worth), the information
		/// will be returned; otherwise, <see langword="null"/>.
		/// </returns>
		/// <seealso cref="AicTechniqueInfo"/>
		private AicTechniqueInfo? CreateAicHint(in SudokuGrid grid, in Node target, bool xEnabled, bool yEnabled)
		{
			var conclusions = new List<Conclusion>();
			if (!target.IsOn)
			{
				// Get eliminations as an AIC.
				var (startCandidate, _) = target.Chain[1];
				var (endCandidate, _) = target.Chain[^2];
				var elimMap = new SudokuMap { startCandidate, endCandidate }.PeerIntersection;
				if (elimMap.IsEmpty)
				{
					return null;
				}

				foreach (int candidate in elimMap)
				{
					if (grid.Exists(candidate / 9, candidate % 9) is true)
					{
						conclusions.Add(new(Elimination, candidate));
					}
				}
			}
			//else
			//{
			//	conclusions.Add(new(Assignment, target.Cell, target.Digit));
			//}

			if (conclusions.Count == 0)
			{
				return null;
			}

			return new(
				conclusions,
				new View[]
				{
					new(
						null,
						target.GetCandidateOffsets(simpleChain: true).AsReadOnlyList(),
						null,
						target.GetLinks())
				},
				xEnabled,
				yEnabled,
				target);
		}

		/// <summary>
		/// Simulate the passing strong and weak links in AICs.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="onToOn">The nodes that the end candidates are currently on.</param>
		/// <param name="onToOff">The nodes the end candidates are currently off.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <param name="chains">The chain nodes.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source node.</param>
		private void DoAic(
			in SudokuGrid grid, ISet<Node> onToOn, ISet<Node> onToOff, bool yEnabled, IList<Node> chains,
			in Node source)
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
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="onToOn">The nodes that the end candidates are currently on.</param>
		/// <param name="onToOff">The nodes the end candidates are currently off.</param>
		/// <param name="xEnabled">Indicates whether the X-Chains are enabled.</param>
		/// <param name="yEnabled">Indicates whether the Y-Chains are enabled.</param>
		/// <param name="loops">The loop nodes.</param>
		/// <param name="source">(<see langword="in"/> parameter) The source node.</param>
		private void DoLoops(
			in SudokuGrid grid, ISet<Node> onToOn, ISet<Node> onToOff,
			bool xEnabled, bool yEnabled, IList<Node> loops, in Node source)
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
