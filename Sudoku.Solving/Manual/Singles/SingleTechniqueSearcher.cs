using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Encapsulates a <b>single</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Singles")]
	public sealed class SingleTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicates the solver enables these options.
		/// </summary>
		private readonly bool _enableFullHouse, _enableLastDigit;


		/// <summary>
		/// Initializes an instance with enable options.
		/// </summary>
		/// <param name="enableFullHouse">
		/// Indicates whether the solver enables full house.
		/// </param>
		/// <param name="enableLastDigit">
		/// Indicates whether the solver enables last digit.
		/// </param>
		public SingleTechniqueSearcher(bool enableFullHouse, bool enableLastDigit) =>
			(_enableFullHouse, _enableLastDigit) = (enableFullHouse, enableLastDigit);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 10;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Search for full houses.
			if (_enableFullHouse)
			{
				for (int region = 0; region < 27; region++)
				{
					int cands = 0, emptyCellCount = 0, fullHouseCellOffset = 0;
					for (int pos = 0; pos < 9; pos++)
					{
						int cellOffset = RegionUtils.GetCellOffset(region, pos);
						int digit = grid[cellOffset];
						if (digit == -1)
						{
							// -1 means the cell is empty.
							switch (++emptyCellCount)
							{
								case 1:
								{
									fullHouseCellOffset = cellOffset;
									break;
								}
								case 2:
								{
									// Two or more empty cells, which means that
									// full house does not exist. Exit this loop.
									goto Label_ToNextRegion;
								}
							}
						}
						else
						{
							// The cell must be given or modifiables.
							cands |= 1 << digit;
						}
					}

					// Check the 'emptyCellCount' is 1 or not.
					if (emptyCellCount == 1)
					{
						// If the number of empty cells is only 1,
						// We can conclude that this only empty cell is the full house.
						int digit = ((short)(511 & ~cands)).FindFirstSet();
						accumulator.Add(
							new FullHouseTechniqueInfo(
								conclusions: new[]
								{
									new Conclusion(
										ConclusionType.Assignment, fullHouseCellOffset, digit)
								},
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets: new[] { (0, fullHouseCellOffset * 9 + digit) },
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								cellOffset: fullHouseCellOffset,
								digit));
					}

				Label_ToNextRegion:;
				}
			}

			// Search for hidden singles & last digits.
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					int hiddenSingleCellOffset = 0, count = 0;
					foreach (int cellOffset in GridMap.GetCellsIn(region))
					{
						if (grid.GetCellStatus(cellOffset) == CellStatus.Empty && !grid[cellOffset, digit])
						{
							switch (++count)
							{
								case 1:
								{
									hiddenSingleCellOffset = cellOffset;
									break;
								}
								case 2:
								{
									goto Label_ToNextRegion;
								}
							}
						}
					}

					if (count == 1)
					{
						bool enableAndIsLastDigit = false;
						if (_enableLastDigit)
						{
							// Sum up the number of appearing in the grid of 'digit'.
							int digitCount = 0;
							for (int i = 0; i < 81; i++)
							{
								if (grid[i] == digit)
								{
									digitCount++;
								}
							}
							enableAndIsLastDigit = digitCount == 8;
						}

						accumulator.Add(
							new HiddenSingleTechniqueInfo(
								conclusions: new[]
								{
									new Conclusion(ConclusionType.Assignment, hiddenSingleCellOffset, digit)
								},
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets: new[] { (0, hiddenSingleCellOffset * 9 + digit) },
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								regionOffset: region,
								cellOffset: hiddenSingleCellOffset,
								digit,
								enableAndIsLastDigit));
					}

				Label_ToNextRegion:;
				}
			}

			// Search for naked singles.
			for (int i = 0; i < 81; i++)
			{
				short mask = grid.GetCandidatesReversal(i);
				if (grid.GetCellStatus(i) == CellStatus.Empty && (mask & (mask - 1)) == 0)
				{
					int digit = mask.FindFirstSet();
					accumulator.Add(
						new NakedSingleTechniqueInfo(
							conclusions: new[] { new Conclusion(ConclusionType.Assignment, i, digit) },
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets: new[] { (0, i * 9 + digit) },
									regionOffsets: null,
									links: null)
							},
							cellOffset: i,
							digit));
				}
			}
		}
	}
}
