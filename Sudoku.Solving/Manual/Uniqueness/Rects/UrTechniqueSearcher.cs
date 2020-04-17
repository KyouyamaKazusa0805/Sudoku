using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Encapsulates an <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique searcher.
	/// </summary>
	[TechniqueDisplay("Unique Rectangle")]
	public sealed partial class UrTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the UR can be uncompleted. In other words,
		/// some of UR candidates can be removed before the pattern forms.
		/// </summary>
		private readonly bool _allowUncompletedUr;

		/// <summary>
		/// Indicates whether the searcher can search for extended URs.
		/// </summary>
		private readonly bool _searchExtended;


		/// <summary>
		/// Initializes an instance with the specified value indicating
		/// whether the structure can be uncompleted, and a value indicating
		/// whether the searcher can search for extended URs.
		/// </summary>
		/// <param name="allowUncompletedUr">
		/// A <see cref="bool"/> value indicating that.
		/// </param>
		/// <param name="searchExtended">A <see cref="bool"/> value indicating that.</param>
		public UrTechniqueSearcher(bool allowUncompletedUr, bool searchExtended) =>
			(_allowUncompletedUr, _searchExtended) = (allowUncompletedUr, searchExtended);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 45;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on mode (whether use AR or UR mode to search).
			var tempList = new List<UrTechniqueInfo>();
			foreach (bool arMode in stackalloc[] { false, true })
			{
				// Iterate on each possible UR structure.
				foreach (int[] urCells in UrCellsList)
				{
					// Check preconditions.
					if (!checkPreconditions(grid, urCells, arMode))
					{
						continue;
					}

					// Get all candidates that all four cells appeared.
					short mask = 0;
					foreach (int urCell in urCells)
					{
						mask |= grid.GetCandidatesReversal(urCell);
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
							if (!arMode && urCells.All(c => (grid.GetCandidatesReversal(c) & comparer).CountSet() != 2))
							{
								continue;
							}

							// Iterate on each corner of four cells.
							for (int c1 = 0; c1 < 4; c1++)
							{
								int corner1 = urCells[c1];
								var otherCellsMap = new GridMap(urCells) { [corner1] = false };

								CheckType1(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap);
								CheckType5(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap);
								CheckHidden(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, otherCellsMap);

								if (c1 == 3)
								{
									break;
								}

								for (int c2 = c1 + 1; c2 < 4; c2++)
								{
									int corner2 = urCells[c2];
									var tempOtherCellsMap = new GridMap(otherCellsMap) { [corner2] = false };

									// Both diagonal and non-diagonal.
									CheckType2(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);

									if (c1 == 0 && c2 == 3 || c1 == 1 && c2 == 2)
									{
										// Diagonal type.
										if (!arMode)
										{
											CheckType6(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);
										}

										if (_searchExtended)
										{
											Check2DOr3X(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);
										}
									}
									else
									{
										// Non-diagonal type.
										for (int size = 2; size <= 4; size++)
										{
											CheckType3Naked(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size);
											CheckType3Hidden(tempList, grid, urCells, arMode, comparer, d1, d2, corner1, corner2, tempOtherCellsMap, size);
										}

										if (!arMode)
										{
											CheckType4(tempList, grid, urCells, false, comparer, d1, d2, corner1, corner2, tempOtherCellsMap);
										}
									}
								}
							}
						}
					}
				}
			}

			// Sort if worth.
			if (tempList.Count != 0)
			{
				tempList.Sort();
				accumulator.AddRange(tempList);
			}

			static bool checkPreconditions(IReadOnlyGrid grid, IEnumerable<int> urCells, bool arMode)
			{
				byte emptyCountWhenArMode = 0, modifiableCount = 0;
				foreach (int urCell in urCells)
				{
					switch (grid.GetCellStatus(urCell))
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

				return modifiableCount != 4 && emptyCountWhenArMode != 4;
			}
		}


		/// <summary>
		/// To determine whether the specified region forms a conjugate pair
		/// of the specified digit, and the cells where they contain the digit
		/// is same as the given map contains.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="map">The map.</param>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsConjugatePair(
			IReadOnlyGrid grid, int digit, GridMap map, int region) =>
			grid.GetDigitAppearingCells(digit, region) == map;

		/// <summary>
		/// Check highlight type.
		/// </summary>
		/// <param name="pair">The pair.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CheckHighlightType((int _id, int) pair) => pair._id == 0;

		/// <summary>
		/// Get a cell that cannot see each other.
		/// </summary>
		/// <param name="urCells">The UR cells.</param>
		/// <param name="cell">The current cell.</param>
		/// <returns>The diagonal cell.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the specified argument <paramref name="cell"/> is invalid.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDiagonalCell(int[] urCells, int cell)
		{
			return true switch
			{
				_ when cell == urCells[0] => urCells[3],
				_ when cell == urCells[1] => urCells[2],
				_ when cell == urCells[2] => urCells[1],
				_ when cell == urCells[3] => urCells[0],
				_ => throw new ArgumentException("The cell is invalid.", nameof(cell))
			};
		}

		/// <summary>
		/// Get all highlight cells.
		/// </summary>
		/// <param name="urCells">The all UR cells used.</param>
		/// <returns>The list of highlight cells.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IReadOnlyList<(int, int)> GetHighlightCells(int[] urCells) =>
			new List<(int, int)>(from cell in urCells select (0, cell));


		#region Method statements
		partial void CheckType1(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap);

		partial void CheckType2(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap);

		partial void CheckType3Naked(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap, int size);

		partial void CheckType3Hidden(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap, int size);

		partial void CheckType4(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap);

		partial void CheckType5(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap);

		partial void CheckType6(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap);

		partial void CheckHidden(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int cornerCell, GridMap otherCellsMap);

		partial void Check2DOr3X(
			IList<UrTechniqueInfo> accumulator, IReadOnlyGrid grid, int[] urCells, bool arMode,
			short comparer, int d1, int d2, int corner1, int corner2, GridMap otherCellsMap);
		#endregion
	}
}
