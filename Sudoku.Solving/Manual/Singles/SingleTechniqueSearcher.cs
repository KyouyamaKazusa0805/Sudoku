using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Encapsulates a <b>single</b> technique searcher.
	/// </summary>
	[DirectSearcher]
	[TechniqueDisplay(nameof(TechniqueCode.NakedSingle))]
	public sealed class SingleTechniqueSearcher : TechniqueSearcher
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
		public SingleTechniqueSearcher(bool enableFullHouse, bool enableLastDigit, bool showDirectLines)
		{
			_enableFullHouse = enableFullHouse;
			_enableLastDigit = enableLastDigit;
			_showDirectLines = showDirectLines;
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(10)
		{
			DisplayLevel = 0,
			IsReadOnly = true
		};


		/// <inheritdoc/>
		/// <remarks>
		/// Note that this technique searcher will be used in other functions,
		/// so we should not use base maps like '<see cref="TechniqueSearcher.EmptyMap"/>'.
		/// Those maps will be initialized in the special cases.
		/// </remarks>
		/// <seealso cref="TechniqueSearcher.EmptyMap"/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
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
		private void GetFullHouses(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
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

				int digit = grid.GetCandidateMask(resultCell).FindFirstSet();
				accumulator.Add(
					new FullHouseTechniqueInfo(
						new Conclusion[] { new(Assignment, resultCell, digit) },
						new View[]
						{
							new(
								null,
								new DrawingInfo[] { new(0, resultCell * 9 + digit) },
								new DrawingInfo[] { new(0, region) },
								null)
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
		private void GetHiddenSinglesOrLastDigits(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
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

					List<(GridMap, GridMap)>? directLines = null;
					if (!enableAndIsLastDigit && _showDirectLines)
					{
						directLines = new();

						// Step 1: Get all source cells that makes the result cell can't be filled with the result digit.
						GridMap crosshatchingCells = GridMap.Empty, tempMap = GridMap.Empty;
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
							if (removableCells.IsNotEmpty)
							{
								directLines.Add((new() { cell }, removableCells));
								tempMap -= removableCells;
							}
						}
					}

					accumulator.Add(
						new HiddenSingleTechniqueInfo(
							new Conclusion[] { new(Assignment, resultCell, digit) },
							new View[]
							{
								new(
									enableAndIsLastDigit ? cellOffsets : null,
									new DrawingInfo[] { new(0, resultCell * 9 + digit) },
									enableAndIsLastDigit ? null : new DrawingInfo[] { new(0, region) },
									null,
									directLines)
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
		private void GetNakedSingles(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid.GetStatus(cell) == CellStatus.Empty
					&& grid.GetCandidateMask(cell) is var mask && mask.IsPowerOfTwo()
					&& mask.FindFirstSet() is var digit)
				{
					List<(GridMap, GridMap)>? directLines = null;
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
										directLines.Add((new() { peerCell }, GridMap.Empty));
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
						new NakedSingleTechniqueInfo(
							new Conclusion[] { new(Assignment, cell, digit) },
							new View[]
							{
								new(
									null,
									new DrawingInfo[] { new(0, cell * 9 + digit) },
									null,
									null,
									directLines)
							},
							cell,
							digit));
				}
			}
		}
	}
}
