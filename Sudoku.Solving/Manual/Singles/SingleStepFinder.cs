using System.Collections.Generic;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	public sealed class SingleStepFinder : StepFinder
	{
		private readonly bool _enableFullHouse, _enableLastDigit;


		public SingleStepFinder(bool enableFullHouse, bool enableLastDigit) =>
			(_enableFullHouse, _enableLastDigit) = (enableFullHouse, enableLastDigit);


		public override IList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<TechniqueInfo>();

			#region Full house
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
						result.Add(
							new FullHouseTechniqueInfo(
								conclusions: new List<Conclusion>
								{
									new Conclusion(ConclusionType.Assignment, fullHouseCellOffset, digit)
								},
								views: new List<View>
								{
									new View(
										cellOffsets: null,
										candidateOffsets: new List<(int, int)>
										{
											(0, fullHouseCellOffset * 9 + digit)
										},
										regionOffsets: new List<(int, int)>
										{
											(0, region)
										},
										linkMasks: null)
								},
								cellOffset: fullHouseCellOffset,
								digit));
					}

				Label_ToNextRegion:;
				}
			}
			#endregion

			#region Hidden Single and Last Digit
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					int hiddenSingleCellOffset = 0, count = 0;
					foreach (int cellOffset in RegionUtils.GetCellOffsets(region))
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

						result.Add(
							new HiddenSingleTechniqueInfo(
								conclusions: new List<Conclusion>
								{
									new Conclusion(ConclusionType.Assignment, hiddenSingleCellOffset, digit)
								},
								views: new List<View>
								{
									new View(
										cellOffsets: null,
										candidateOffsets: new List<(int, int)>
										{
											(0, hiddenSingleCellOffset * 9 + digit)
										},
										regionOffsets: new List<(int, int)>
										{
											(0, region)
										},
										linkMasks: null)
								},
								regionOffset: region,
								cellOffset: hiddenSingleCellOffset,
								digit,
								enableAndIsLastDigit));
					}

				Label_ToNextRegion:;
				}
			}
			#endregion

			#region Naked single
			for (int i = 0; i < 81; i++)
			{
				short mask = (short)(511 & ~grid.GetMask(i));
				if (grid.GetCellStatus(i) == CellStatus.Empty && (mask & (mask - 1)) == 0)
				{
					int digit = mask.FindFirstSet();
					result.Add(
						new NakedSingleTechniqueInfo(
							conclusions: new List<Conclusion>
							{
								new Conclusion(ConclusionType.Assignment, i, digit)
							},
							views: new List<View>
							{
								new View(
									cellOffsets: null,
									candidateOffsets: new List<(int, int)>
									{
										(0, i * 9 + digit)
									},
									regionOffsets: null,
									linkMasks: null)
							},
							cellOffset: i,
							digit));
				}
			}
			#endregion

			return result;
		}
	}
}
