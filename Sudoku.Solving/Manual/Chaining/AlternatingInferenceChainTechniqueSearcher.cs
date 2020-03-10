using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an alternating inference chain (AIC) technique searcher.
	/// </summary>
	public sealed class AlternatingInferenceChainTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the searcher will search for X-Chains.
		/// </summary>
		private readonly bool _searchX;

		/// <summary>
		/// Indicates whether the searcher will search for Y-Chains.
		/// </summary>
		private readonly bool _searchY;

		/// <summary>
		/// Indicates the maximum length to search.
		/// </summary>
		private readonly int _maxLength;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="searchX">Indicates searching X-Chains or not.</param>
		/// <param name="searchY">Indicates searching Y-Chains or not.</param>
		/// <param name="maxLength">
		/// Indicates the maximum length of a chain to search.
		/// </param>
		public AlternatingInferenceChainTechniqueSearcher(
			bool searchX, bool searchY, int maxLength)
		{
			_searchX = searchX;
			_searchY = searchY;
			_maxLength = maxLength;
		}


		/// <inheritdoc/>
		public override int Priority { get; set; } = 45;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on each strong relation, and search for weak relations.
			var candidateList = FullGridMap.Empty;
			var stack = new List<int>();
			var strongRelations = GetAllStrongRelations(grid);
			foreach (var (start, end) in strongRelations)
			{
				foreach (var (startCandidate, endCandidate) in stackalloc[] { (start, end), (end, start) })
				{
					int endCell = endCandidate / 9;
					int endDigit = endCandidate % 9;
					candidateList[startCandidate] = true;
					candidateList[endCandidate] = true;
					stack.Add(startCandidate);
					stack.Add(endCandidate);

					// Get 'on' to 'off' nodes and 'off' to 'on' nodes recursively.
					GetOnToOffRecursively(
						accumulator, grid, candidateList, endCell, endDigit,
						strongRelations, stack, _maxLength - 2);

					// Undo the step to recover the candidate status.
					candidateList[startCandidate] = false;
					candidateList[endCandidate] = false;
					stack.RemoveLastElement();
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get 'on' nodes to 'off' nodes recursively (Searching for weak links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <param name="strongRelations">The strong relations.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="length">The last length to search.</param>
		private void GetOnToOffRecursively(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidateList,
			int currentCell, int currentDigit, IReadOnlyList<(int, int)> strongRelations,
			IList<int> stack, int length)
		{
			if (length < 0)
			{
				return;
			}

			// Search for same regions.
			foreach (int nextCell in new GridMap(currentCell, false).Offsets)
			{
				if (!grid.CandidateExists(nextCell, currentDigit))
				{
					continue;
				}

				int nextCandidate = nextCell * 9 + currentDigit;
				if (candidateList[nextCandidate])
				{
					continue;
				}

				candidateList[nextCandidate] = true;
				stack.Add(nextCandidate);

				GetOffToOnRecursively(
					accumulator, grid, candidateList, nextCell, currentDigit,
					strongRelations, stack, length - 1);

				candidateList[nextCandidate] = false;
				stack.RemoveLastElement();
			}

			// Search for the cells.
			if (_searchY)
			{
				foreach (int nextDigit in grid.GetCandidatesReversal(currentCell).GetAllSets())
				{
					if (nextDigit == currentDigit)
					{
						continue;
					}

					int nextCandidate = currentCell * 9 + nextDigit;
					if (candidateList[nextCandidate])
					{
						continue;
					}

					candidateList[nextCandidate] = true;
					stack.Add(nextCandidate);

					GetOffToOnRecursively(
						accumulator, grid, candidateList, currentCell, nextDigit,
						strongRelations, stack, length - 1);

					candidateList[nextCandidate] = false;
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get 'off' nodes to 'on' nodes recursively (Searching for strong links).
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="currentDigit">The current digit.</param>
		/// <param name="strongRelations">All strong relations.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="length">The last length to search.</param>
		private void GetOffToOnRecursively(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidateList,
			int currentCell, int currentDigit, IReadOnlyList<(int, int)> strongRelations,
			IList<int> stack, int length)
		{
			if (length < 0)
			{
				return;
			}

			// Search for same regions.
			var (r, c, b) = CellUtils.GetRegion(currentCell);
			foreach (int region in stackalloc[] { r + 9, c + 18, b })
			{
				var map = grid.GetDigitAppearingCells(currentDigit, region);
				if (map.Count != 2)
				{
					continue;
				}

				map[currentCell] = false;
				int nextCell = map.SetBitAt(0);
				int nextCandidate = nextCell * 9 + currentDigit;
				if (candidateList[nextCandidate])
				{
					continue;
				}

				candidateList[nextCandidate] = true;
				stack.Add(nextCandidate);

				// Now check elimination.
				// If the elimination exists, the chain will be added to the accumulator.
				CheckElimination(accumulator, grid, candidateList, stack);

				GetOnToOffRecursively(
					accumulator, grid, candidateList, nextCell, currentDigit,
					strongRelations, stack, length - 1);

				candidateList[nextCandidate] = false;
				stack.RemoveLastElement();
			}

			// Search for cell.
			if (_searchY)
			{
				if (grid.IsBivalueCell(currentCell, out short mask))
				{
					mask &= (short)~(1 << currentDigit);
					int nextDigit = mask.FindFirstSet();
					int nextCandidate = currentCell * 9 + nextDigit;
					if (candidateList[nextCandidate])
					{
						return;
					}

					candidateList[nextCandidate] = true;
					stack.Add(nextCandidate);

					// Now check elimination.
					// If the elimination exists, the chain will be added to the accumulator.
					CheckElimination(accumulator, grid, candidateList, stack);

					GetOnToOffRecursively(
						accumulator, grid, candidateList, currentCell, nextDigit,
						strongRelations, stack, length - 1);

					candidateList[nextCandidate] = false;
					stack.RemoveLastElement();
				}
			}
		}

		/// <summary>
		/// Get all strong relations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All strong relations.</returns>
		private IReadOnlyList<(int, int)> GetAllStrongRelations(IReadOnlyGrid grid)
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
			if (_searchY)
			{
				for (int cell = 0; cell < 81; cell++)
				{
					if (!grid.IsBivalueCell(cell, out short mask))
					{
						continue;
					}

					int digit1 = mask.FindFirstSet();
					result.Add((cell * 9 + digit1, cell * 9 + mask.GetNextSetBit(digit1)));
				}
			}

			return result;
		}


		/// <summary>
		/// Check the elimination, and save the chain into the accumulator
		/// when the chain is valid and worth.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="candidateList">The candidate list.</param>
		/// <param name="stack">The stack.</param>
		private static void CheckElimination(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, FullGridMap candidateList,
			IList<int> stack)
		{
			int startCandidate = stack[0], endCandidate = stack[^1];
			var elimMap = FullGridMap.CreateInstance(new[] { startCandidate, endCandidate });
			if (!elimMap.IsNotEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int candidate in elimMap.Offsets)
			{
				if (grid.CandidateExists(candidate / 9, candidate % 9))
				{
					conclusions.Add(new Conclusion(ConclusionType.Elimination, candidate));
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			// Now we should construct a node list.
			// Record all highlight candidates.
			int lastCand = default;
			var candidateOffsets = new List<(int, int)>();
			var nodes = new List<Node>();
			var links = new List<Inference>();
			bool @switch = false;
			int i = 0;
			foreach (int candidate in stack)
			{
				nodes.Add(new Node(candidate, @switch));
				candidateOffsets.Add((@switch ? 1 : 0, candidate));

				// To ensure this loop has the predecessor.
				if (i++ > 0)
				{
					links.Add(new Inference(lastCand, !@switch, candidate, @switch));
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
							links)
					},
					nodes));
		}
	}
}
