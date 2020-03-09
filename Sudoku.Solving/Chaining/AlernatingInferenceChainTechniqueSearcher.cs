using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Chaining
{
	/// <summary>
	/// Encapsulates an alternating inference chain (AIC) technique searcher.
	/// </summary>
	public sealed class AlernatingInferenceChainTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <inheritdoc/>
		public override int Priority { get; set; } = 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (!(grid is Grid tempGrid))
			{
				// If the grid is not a normal grid, the solver cannot find a way
				// to process, so do nothing.
				return;
			}

			// Iterate on each strong relation, and search for weak relations.
			var undoable = new UndoableGrid(tempGrid);
			var strongRelations = GetAllStrongRelations(undoable);
			foreach (var (start, end) in strongRelations)
			{
				foreach (var infer in new[]
				{
					new Inference(start, false, end, true),
					new Inference(end, false, start, true)
				})
				{
					var (_, _, _, endCell, endDigit, _) = infer;
					undoable[endCell] = endDigit;

					var stack = new List<int> { infer.EndCandidate };

					// Get 'on' to 'off' nodes and 'off' to 'on' nodes recursively.
					GetOnToOffRecursively(accumulator, undoable, stack, endCell, endDigit, strongRelations);

					// Undo the step to recover the candidate status.
					undoable.Undo();
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get 'on' nodes to 'off' nodes recursively (Searching for weak links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="undoable">Undoable grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <param name="strongRelations">The strong relations.</param>
		private void GetOnToOffRecursively(
			IBag<TechniqueInfo> accumulator, UndoableGrid undoable, List<int> candidateList,
			int currentCell, int currentDigit, IReadOnlyList<(int, int)> strongRelations)
		{
			var (r, c, b) = CellUtils.GetRegion(currentCell);
			r += 9;
			c += 18;

			// Search for other regions.
			foreach (int region in stackalloc[] { r, c, b })
			{
				foreach (int cell in new GridMap(undoable.GetDigitAppearingCells(currentDigit, region))
				{
					[currentCell] = false
				}.Offsets)
				{
					if (candidateList.Exists(cand => cand / 9 == cell))
					{
						// The list contains the specified cell,
						// so this cell should not be added into this list.
						continue;
					}

					candidateList.Add(cell * 9 + currentDigit);
					undoable[cell, currentDigit] = true;

					GetOffToOnRecursively(accumulator, undoable, candidateList, cell, currentDigit, strongRelations);

					undoable.Undo();
					candidateList.RemoveLastElement();
				}
			}

			// Search for the cells.
			short mask = (short)(undoable.GetCandidatesReversal(currentCell) & ~(1 << currentDigit));
			foreach (int digit in mask.GetAllSets())
			{
				candidateList.Add(currentCell * 9 + digit);
				undoable[currentCell, digit] = true;

				GetOffToOnRecursively(accumulator, undoable, candidateList, currentCell, digit, strongRelations);

				undoable.Undo();
				candidateList.RemoveLastElement();
			}
		}

		/// <summary>
		/// Get 'off' nodes to 'on' nodes recursively (Searching for strong links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="undoable">Undoable grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <param name="strongRelations">All strong relations.</param>
		private void GetOffToOnRecursively(
			IBag<TechniqueInfo> accumulator, UndoableGrid undoable, List<int> candidateList,
			int currentCell, int currentDigit, IReadOnlyList<(int, int)> strongRelations)
		{
			// Now we should check elimination.
			// If the elimination exists, the chain will be added to the accumulator;
			// otherwise, continue to search.
			int startCandidate = candidateList[0], endCandidate = candidateList[^1];
			var elimMap = FullGridMap.CreateInstance(new[] { startCandidate, endCandidate });
			if (elimMap.IsNotEmpty)
			{
				var conclusions = new List<Conclusion>();
				foreach (int candidate in elimMap.Offsets)
				{
					if (undoable.CandidateExists(candidate / 9, candidate % 9))
					{
						conclusions.Add(new Conclusion(ConclusionType.Elimination, candidate));
					}
				}

				if (conclusions.Count != 0)
				{
					// Now we should construct a node list.
					// Record all highlight candidates.
					int lastCand = default;
					var nodes = new List<Node>();
					var candidateOffsets = new List<(int, int)>();
					var linkMasks = new List<Inference>();
					bool @switch = false;
					int i = 0;
					foreach (int candidate in candidateList)
					{
						nodes.Add(new Node(candidate, @switch));
						candidateOffsets.Add((@switch ? 1 : 0, candidate));

						// To ensure this loop has the predecessor.
						if (i++ > 0)
						{
							linkMasks.Add(new Inference(lastCand, @switch, candidate, !@switch));
						}

						lastCand = candidate;
						@switch = !@switch;
					}

					accumulator.Add(
						new AlternatingInferenceChainTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: null,
									linkMasks)
							},
							nodes));
				}
			}

			// Continue to search strong links.
			foreach (var (start, end) in strongRelations)
			{
				foreach (var infer in new[]
				{
					new Inference(start, false, end, true),
					new Inference(end, false, start, true)
				})
				{
					var (_, _, _, endCell, endDigit, _) = infer;

					undoable[endCell] = endDigit;
					candidateList.Add(infer.EndCandidate);

					// Get 'on' to 'off' nodes and 'off' to 'on' nodes recursively.
					GetOnToOffRecursively(accumulator, undoable, candidateList, endCell, endDigit, strongRelations);

					// Undo the step to recover the candidate status.
					undoable.Undo();
					candidateList.RemoveLastElement();
				}
			}
		}


		/// <summary>
		/// Get all strong relations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All strong relations.</returns>
		private static IReadOnlyList<(int, int)> GetAllStrongRelations(IReadOnlyGrid grid)
		{
			var result = new List<(int, int)>();
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					if (!grid.IsBilocationRegion(digit, region, out short mask))
					{
						continue;
					}

					int pos1 = mask.FindFirstSet();
					result.Add((
						RegionUtils.GetCellOffset(region, pos1) * 9 + digit,
						RegionUtils.GetCellOffset(region, mask.GetNextSetBit(pos1)) * 9 + digit));
				}
			}
			for (int cell = 0; cell < 81; cell++)
			{
				if (!grid.IsBivalueCell(cell))
				{
					continue;
				}

				short mask = grid.GetCandidatesReversal(cell);
				int digit1 = mask.FindFirstSet();
				result.Add((cell * 9 + digit1, cell * 9 + mask.GetNextSetBit(digit1)));
			}

			return result;
		}
	}
}
