using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Encapsulates a <b>single</b> technique searcher.
	/// </summary>
	[DirectSearcher]
	public sealed class SingleStepSearcher : StepSearcher
	{
		/// <summary>
		/// Indicates the solver enables these options.
		/// </summary>
		private readonly bool _enableFullHouse, _enableLastDigit, _showDirectLines;


		/// <summary>
		/// Initializes an instance with enable options.
		/// </summary>
		/// <param name="enableFullHouse">
		/// Indicates whether the solver enables full house.
		/// </param>
		/// <param name="enableLastDigit">
		/// Indicates whether the solver enables last digit.
		/// </param>
		/// <param name="showDirectLines">
		/// Indicates whether the solver shows the direct lines (cross-hatching information).
		/// </param>
		public SingleStepSearcher(bool enableFullHouse, bool enableLastDigit, bool showDirectLines)
		{
			_enableFullHouse = enableFullHouse;
			_enableLastDigit = enableLastDigit;
			_showDirectLines = showDirectLines;
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(1, nameof(TechniqueCode.NakedSingle))
		{
			DisplayLevel = 0,
			IsReadOnly = true
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			GetFullHouses(accumulator, grid);
			GetHiddenSinglesOrLastDigits(accumulator, grid);
			GetNakedSingles(accumulator, grid);
		}

		/// <summary>
		/// Get all full houses.
		/// </summary>
		/// <param name="accumulator">The current accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		private void GetFullHouses(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			if (!_enableFullHouse)
			{
				return;
			}

			for (int region = 0; region < 27; region++)
			{
				var map = RegionMaps[region];
				int count = 0;
				bool flag = true;
				int resultCell = -1;
				foreach (int cell in map)
				{
					if (grid.GetStatus(cell) == CellStatus.Empty)
					{
						resultCell = cell;
						if (++count > 1)
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag || count == 0)
				{
					continue;
				}

				int digit = TrailingZeroCount(grid.GetCandidates(resultCell));
				accumulator.Add(
					new FullHouseStepInfo(
						new Conclusion[] { new(ConclusionType.Assignment, resultCell, digit) },
						new View[]
						{
							new()
							{
								Candidates = new DrawingInfo[] { new(0, resultCell * 9 + digit) },
								Regions = new DrawingInfo[] { new(0, region) }
							}
						},
						resultCell,
						digit));
			}
		}

		/// <summary>
		/// Get all hidden singles or last digits.
		/// </summary>
		/// <param name="accumulator">The current accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		private void GetHiddenSinglesOrLastDigits(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					var map = RegionMaps[region];
					int count = 0, resultCell = -1;
					bool flag = true;
					foreach (int cell in map)
					{
						if (grid.Exists(cell, digit) is true)
						{
							resultCell = cell;
							if (++count > 1)
							{
								flag = false;
								break;
							}
						}
					}
					if (!flag || count == 0)
					{
						continue;
					}

					bool enableAndIsLastDigit = false;
					var cellOffsets = new List<DrawingInfo>();
					if (_enableLastDigit)
					{
						// Sum up the number of appearing in the grid of 'digit'.
						int digitCount = 0;
						for (int i = 0; i < 81; i++)
						{
							if (grid[i] == digit)
							{
								digitCount++;
								cellOffsets.Add(new(0, i));
							}
						}

						enableAndIsLastDigit = digitCount == 8;
					}

					List<(Cells, Cells)>? directLines = null;
					if (!enableAndIsLastDigit && _showDirectLines)
					{
						directLines = new();

						// Step 1: Get all source cells that makes the result cell
						// can't be filled with the result digit.
						Cells crosshatchingCells = Cells.Empty, tempMap = Cells.Empty;
						foreach (int cell in RegionCells[region])
						{
							if (cell != resultCell && grid.GetStatus(cell) == CellStatus.Empty)
							{
								tempMap.AddAnyway(cell);
							}
						}
						foreach (int cell in tempMap)
						{
							foreach (int peerCell in PeerMaps[cell])
							{
								if (cell != resultCell && grid[peerCell] == digit)
								{
									crosshatchingCells.AddAnyway(peerCell);
								}
							}
						}

						// Step 2: Get all removed cells in this region.
						foreach (int cell in crosshatchingCells)
						{
							var removableCells = PeerMaps[cell] & tempMap;
							if (!removableCells.IsEmpty)
							{
								directLines.Add((new() { cell }, removableCells));
								tempMap -= removableCells;
							}
						}
					}

					accumulator.Add(
						new HiddenSingleStepInfo(
							new Conclusion[] { new(ConclusionType.Assignment, resultCell, digit) },
							new View[]
							{
								new()
								{
									Cells = enableAndIsLastDigit ? cellOffsets : null,
									Candidates = new DrawingInfo[] { new(0, resultCell * 9 + digit) },
									Regions = enableAndIsLastDigit ? null : new DrawingInfo[] { new(0, region) },
									DirectLines = directLines
								}
							},
							resultCell,
							digit,
							region,
							enableAndIsLastDigit));
				}
			}
		}

		/// <summary>
		/// Get all naked singles.
		/// </summary>
		/// <param name="accumulator">The current accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		private void GetNakedSingles(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				short mask = grid.GetCandidates(cell);
				if (!mask.IsPowerOfTwo())
				{
					continue;
				}

				int digit = TrailingZeroCount(mask);
				List<(Cells, Cells)>? directLines = null;
				if (_showDirectLines)
				{
					directLines = new();
					for (int i = 0; i < 9; i++)
					{
						if (digit != i)
						{
							bool flag = false;
							foreach (int peerCell in PeerMaps[cell])
							{
								if (grid[peerCell] == i)
								{
									directLines.Add((new() { peerCell }, Cells.Empty));
									flag = true;
									break;
								}
							}
							if (flag)
							{
								continue;
							}
						}
					}
				}

				accumulator.Add(
					new NakedSingleStepInfo(
						new Conclusion[] { new(ConclusionType.Assignment, cell, digit) },
						new View[]
						{
							new()
							{
								Candidates = new DrawingInfo[] { new(0, cell * 9 + digit) },
								DirectLines = directLines
							}
						},
						cell,
						digit));
			}
		}
	}
}
