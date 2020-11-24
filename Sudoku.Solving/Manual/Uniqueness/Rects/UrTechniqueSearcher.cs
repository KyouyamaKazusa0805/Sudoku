using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.Values;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Encapsulates an <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.UrType1))]
	public sealed partial class UrTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the UR can be incomplete. In other words,
		/// some of UR candidates can be removed before the pattern forms.
		/// </summary>
		private readonly bool _allowIncompleteUr;

		/// <summary>
		/// Indicates whether the searcher can search for extended URs.
		/// </summary>
		private readonly bool _searchExtended;


		/// <summary>
		/// Initializes an instance with the specified value indicating
		/// whether the structure can be incomplete, and a value indicating
		/// whether the searcher can search for extended URs.
		/// </summary>
		/// <param name="allowIncomplete">
		/// A <see cref="bool"/> value indicating that.
		/// </param>
		/// <param name="searchExtended">A <see cref="bool"/> value indicating that.</param>
		public UrTechniqueSearcher(bool allowIncomplete, bool searchExtended) =>
			(_allowIncompleteUr, _searchExtended) = (allowIncomplete, searchExtended);


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(45);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			// Iterate on mode (whether use AR or UR mode to search).
			var tempList = new List<UrTechniqueInfo>();
			foreach (bool arMode in BooleanValues)
			{
				// Iterate on each possible UR structure.
				for (int index = 0; index < PossibleUrList.Length; index++)
				{
					int[] urCells = PossibleUrList[index];
					// Check preconditions.
					if (!checkPreconditions(grid, urCells, arMode))
					{
						continue;
					}

					// Get all candidates that all four cells appeared.
					short mask = 0;
					foreach (int urCell in urCells)
					{
						mask |= grid.GetCandidateMask(urCell);
					}

					// Iterate on each possible digit combination.
					int[] allDigitsInThem = mask.GetAllSets().ToArray();
					for (int i = 0, length = allDigitsInThem.Length; i < length - 1; i++)
					{
						int d1 = allDigitsInThem[i];
						for (int j = i + 1; j < length; j++)
						{
							int d2 = allDigitsInThem[j];

							// All possible UR patterns should contain at least one cell
							// that contains both 'd1' and 'd2'.
							short comparer = (short)(1 << d1 | 1 << d2);
							unsafe
							{
								static bool v(int c, in short comparer, in SudokuGrid grid) =>
									(grid.GetCandidateMask(c) & comparer).PopCount() != 2;

								if (!arMode && urCells.All(&v, comparer, grid))
								{
									continue;
								}
							}

							// Iterate on each corner of four cells.
							for (int c1 = 0; c1 < 4; c1++)
							{
								int corner1 = urCells[c1];
								var otherCellsMap = new GridMap(urCells) { ~corner1 };

								CheckType1(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
								CheckType5(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);
								CheckHidden(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap, index);

								if (!arMode && _searchExtended)
								{
									Check3X(tempList, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
									Check3X2SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
									Check3N2SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
									Check3U2SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
									Check3E2SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, otherCellsMap, index);
								}

								if (c1 == 3)
								{
									break;
								}

								for (int c2 = c1 + 1; c2 < 4; c2++)
								{
									int corner2 = urCells[c2];
									var tempOtherCellsMap = otherCellsMap - corner2;

									// Both diagonal and non-diagonal.
									CheckType2(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

									if (_searchExtended)
									{
										for (int size = 2; size <= 4; size++)
										{
											CheckWing(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size, index);
										}
									}

									if ((c1, c2) is (0, 3) or (1, 2))
									{
										// Diagonal type.
										if (!arMode)
										{
											CheckType6(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

											if (_searchExtended)
											{
												Check2D(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
												Check2D1SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											}
										}
									}
									else
									{
										// Non-diagonal type.
										CheckType3Naked(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

										if (!arMode)
										{
											CheckType4(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);

											if (_searchExtended)
											{
												Check2B1SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
												Check4X3SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
												Check4C3SL(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, index);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Sort and remove duplicate instances if worth.
			if (tempList.Count != 0)
			{
				tempList.Distinct();
				tempList.Sort();
				accumulator.AddRange(tempList);
			}

			static bool checkPreconditions(in SudokuGrid grid, IEnumerable<int> urCells, bool arMode)
			{
				byte emptyCountWhenArMode = 0, modifiableCount = 0;
				foreach (int urCell in urCells)
				{
					switch (grid.GetStatus(urCell))
					{
						case Given:
						case Modifiable when !arMode:
						{
							return false;
						}
						case Empty when arMode:
						{
							emptyCountWhenArMode++;
							break;
						}
						case Modifiable:
						{
							modifiableCount++;
							break;
						}
					}
				}

				return (modifiableCount, emptyCountWhenArMode) is (not 4, not 4);
			}
		}


		/// <summary>
		/// To determine whether the specified region forms a conjugate pair
		/// of the specified digit, and the cells where they contain the digit
		/// is same as the given map contains.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsConjugatePair(int digit, in GridMap map, int region) =>
			(RegionMaps[region] & CandMaps[digit]) == map;

		/// <summary>
		/// Check highlight type.
		/// </summary>
		/// <param name="pair">(<see langword="in"/> parameter) The pair.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CheckHighlightType(in DrawingInfo pair) => pair.Id == 0;

		/// <summary>
		/// Get a cell that can't see each other.
		/// </summary>
		/// <param name="urCells">The UR cells.</param>
		/// <param name="cell">The current cell.</param>
		/// <returns>The diagonal cell.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the specified argument <paramref name="cell"/> is invalid.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDiagonalCell(int[] urCells, int cell) =>
			true switch
			{
				_ when cell == urCells[0] => urCells[3],
				_ when cell == urCells[1] => urCells[2],
				_ when cell == urCells[2] => urCells[1],
				_ when cell == urCells[3] => urCells[0],
				_ => throw new ArgumentException("The cell is invalid.", nameof(cell))
			};

		/// <summary>
		/// Get a cell that is in the same region of the specified cell lies in.
		/// </summary>
		/// <param name="currentCell">The current cell.</param>
		/// <param name="anotherCell">Another cell to check.</param>
		/// <param name="region">
		/// (<see langword="out"/> parameter) The result regions that both cells lie in.
		/// If the cell can't be found, the parameter will be an empty array of type <see cref="int"/>.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/> value indicating whether the another cell is same region as the current one.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsSameRegionCell(int currentCell, int anotherCell, out IEnumerable<int> region)
		{
			if (new GridMap { anotherCell, currentCell }.CoveredRegions is var coveredRegions
				&& coveredRegions.None())
			{
				region = Array.Empty<int>();
				return false;
			}
			else
			{
				region = coveredRegions;
				return true;
			}
		}

		/// <summary>
		/// Get all highlight cells.
		/// </summary>
		/// <param name="urCells">The all UR cells used.</param>
		/// <returns>The list of highlight cells.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IReadOnlyList<DrawingInfo> GetHighlightCells(int[] urCells) =>
			(from cell in urCells select new DrawingInfo(0, cell)).ToArray();


		partial void CheckType1(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void CheckType2(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void CheckType3Naked(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void CheckType4(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void CheckType5(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void CheckType6(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void CheckHidden(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void Check2D(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void Check2B1SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void Check2D1SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void Check3X(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void Check3X2SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void Check3N2SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void Check3U2SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void Check3E2SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in GridMap otherCellsMap, int index);
		partial void Check4X3SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void Check4C3SL(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int index);
		partial void CheckWing(IList<UrTechniqueInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in GridMap otherCellsMap, int size, int index);
	}
}
