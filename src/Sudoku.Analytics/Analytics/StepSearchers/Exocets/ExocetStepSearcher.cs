namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Junior Exocet</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Junior Exocet</item>
/// <item>Senior Exocet</item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.JuniorExocet, Technique.SeniorExocet, Technique.ComplexSeniorExocet,
	Technique.SiameseJuniorExocet, Technique.SiameseSeniorExocet,
	ConditionalCases = ConditionalCase.TimeComplexity)]
public sealed partial class ExocetStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates the houses.
	/// </summary>
	private static readonly House[] Houses = [9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 0, 1, 2, 3, 4, 5, 6, 7, 8];

	/// <summary>
	/// Indicates the list of cells nearby crossline at the specified house. Suppose we don't use any blocks as crosslines,
	/// The former 9 values in the target array are unmeaningful to be used.
	/// </summary>
	private static readonly CellMap[][] BaseOrTargetCellsCombination = [
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		null!, // Do not use them
		[[9, 18], [10, 19], [11, 20], [12, 21], [13, 22], [14, 23], [15, 24], [16, 25], [17, 26]],
		[[0, 18], [1, 19], [2, 20], [3, 21], [4, 22], [5, 23], [6, 24], [7, 25], [8, 26]],
		[[0, 9], [1, 10], [2, 11], [3, 12], [4, 13], [5, 14], [6, 15], [7, 16], [8, 17]],
		[[9 + 27, 18 + 27], [10 + 27, 19 + 27], [11 + 27, 20 + 27], [12 + 27, 21 + 27], [13 + 27, 22 + 27], [14 + 27, 23 + 27], [15 + 27, 24 + 27], [16 + 27, 25 + 27], [17 + 27, 26 + 27]],
		[[0 + 27, 18 + 27], [1 + 27, 19 + 27], [2 + 27, 20 + 27], [3 + 27, 21 + 27], [4 + 27, 22 + 27], [5 + 27, 23 + 27], [6 + 27, 24 + 27], [7 + 27, 25 + 27], [8 + 27, 26 + 27]],
		[[0 + 27, 9 + 27], [1 + 27, 10 + 27], [2 + 27, 11 + 27], [3 + 27, 12 + 27], [4 + 27, 13 + 27], [5 + 27, 14 + 27], [6 + 27, 15 + 27], [7 + 27, 16 + 27], [8 + 27, 17 + 27]],
		[[9 + 54, 18 + 54], [10 + 54, 19 + 54], [11 + 54, 20 + 54], [12 + 54, 21 + 54], [13 + 54, 22 + 54], [14 + 54, 23 + 54], [15 + 54, 24 + 54], [16 + 54, 25 + 54], [17 + 54, 26 + 54]],
		[[0 + 54, 18 + 54], [1 + 54, 19 + 54], [2 + 54, 20 + 54], [3 + 54, 21 + 54], [4 + 54, 22 + 54], [5 + 54, 23 + 54], [6 + 54, 24 + 54], [7 + 54, 25 + 54], [8 + 54, 26 + 54]],
		[[0 + 54, 9 + 54], [1 + 54, 10 + 54], [2 + 54, 11 + 54], [3 + 54, 12 + 54], [4 + 54, 13 + 54], [5 + 54, 14 + 54], [6 + 54, 15 + 54], [7 + 54, 16 + 54], [8 + 54, 17 + 54]],
		[[1, 2], [10, 11], [19, 20], [28, 29], [37, 38], [46, 47], [55, 56], [64, 65], [73, 74]],
		[[0, 2], [9, 11], [18, 20], [27, 29], [36, 38], [45, 47], [54, 56], [63, 65], [72, 74]],
		[[0, 1], [9, 10], [18, 19], [27, 28], [36, 37], [45, 46], [54, 55], [63, 64], [72, 73]],
		[[1 + 3, 2 + 3], [10 + 3, 11 + 3], [19 + 3, 20 + 3], [28 + 3, 29 + 3], [37 + 3, 38 + 3], [46 + 3, 47 + 3], [55 + 3, 56 + 3], [64 + 3, 65 + 3], [73 + 3, 74 + 3]],
		[[0 + 3, 2 + 3], [9 + 3, 11 + 3], [18 + 3, 20 + 3], [27 + 3, 29 + 3], [36 + 3, 38 + 3], [45 + 3, 47 + 3], [54 + 3, 56 + 3], [63 + 3, 65 + 3], [72 + 3, 74 + 3]],
		[[0 + 3, 1 + 3], [9 + 3, 10 + 3], [18 + 3, 19 + 3], [27 + 3, 28 + 3], [36 + 3, 37 + 3], [45 + 3, 46 + 3], [54 + 3, 55 + 3], [63 + 3, 64 + 3], [72 + 3, 73 + 3]],
		[[1 + 6, 2 + 6], [10 + 6, 11 + 6], [19 + 6, 20 + 6], [28 + 6, 29 + 6], [37 + 6, 38 + 6], [46 + 6, 47 + 6], [55 + 6, 56 + 6], [64 + 6, 65 + 6], [73 + 6, 74 + 6]],
		[[0 + 6, 2 + 6], [9 + 6, 11 + 6], [18 + 6, 20 + 6], [27 + 6, 29 + 6], [36 + 6, 38 + 6], [45 + 6, 47 + 6], [54 + 6, 56 + 6], [63 + 6, 65 + 6], [72 + 6, 74 + 6]],
		[[0 + 6, 1 + 6], [9 + 6, 10 + 6], [18 + 6, 19 + 6], [27 + 6, 28 + 6], [36 + 6, 37 + 6], [45 + 6, 46 + 6], [54 + 6, 55 + 6], [63 + 6, 64 + 6], [72 + 6, 73 + 6]]
	];


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		return null;

		scoped ref readonly var grid = ref context.Grid;

		// This algorithm starts with cross-line cells.
		// Firstly, we should iterate on each size of cross-line houses. The maximum possible value is 4.
		for (var crosslineHousesCount = 2; crosslineHousesCount <= 4; crosslineHousesCount++)
		{
			// Iterate on each combination of houses.
			foreach (var houseCombination in Houses.GetSubsets(crosslineHousesCount))
			{
				// For each combination of houses, we should check for whether both numbers of rows and columns are at least 2.
				// If not, we cannot arrange base or target cells because they may not be put into a same chute and aligned.
				var houseTypeCounter = new Dictionary<HouseType, HouseMask>(3);
				foreach (var house in houseCombination)
				{
					var houseType = house.ToHouseType();
					if (!houseTypeCounter.TryAdd(houseType, house))
					{
						houseTypeCounter[houseType] |= 1 << house;
					}
				}
				if ((!houseTypeCounter.TryGetValue(HouseType.Row, out var rowMask) || PopCount((uint)rowMask) < 2)
					&& (!houseTypeCounter.TryGetValue(HouseType.Column, out var columnMask) || PopCount((uint)columnMask) < 2))
				{
					continue;
				}

				// In addition, the iterated combination cannot contain blocks only. We don't allow blocks as crosslines.
				if (!houseTypeCounter.ContainsKey(HouseType.Row) && !houseTypeCounter.ContainsKey(HouseType.Column))
				{
					continue;
				}

				// If the number of rows or columns is at least 2, we should make a pair for that 2 cross-line houses.
				foreach (var crosslinePairHouseType in stackalloc[] { HouseType.Row, HouseType.Column })
				{
					if (!houseTypeCounter.TryGetValue(crosslinePairHouseType, out var houseMask) || PopCount((uint)houseMask) < 2)
					{
						continue;
					}

					// Make a pair of houses that are of same house type.
					foreach (var crosslinePair in houseMask.GetAllSets().GetSubsets(2))
					{
						// Now we have a pair of houses. We should check all possible cases for the 4 cells.
						// Such 4 cells should be nearby the crossline, and orthogonal with the crossline, forming an "L" or "T" shape.
						// In addition, 4 cells should be separated with 2 parts, and both parts contain 2 cells.
						// One is nearby and so does the other.
						// According to the description, we can get the sketch like:
						//
						//   Crossline   Crossline
						//       ↓           ↓
						//   F F . | . . . | . . .
						//   . . . | . . . | . F F
						//   . . . | . . . | . . .
						//
						// 4 cells marked as letter F means the cells should be found here.
						// Such cells are called "Base Cells" and "Target Cells". One part is for "Base" and the other is for "Target".
						foreach (var (baseCrossline, targetCrossline) in stackalloc[]
						{
							(crosslinePair[0], crosslinePair[1]),
							(crosslinePair[1], crosslinePair[0])
						})
						{
							foreach (var baseCells in BaseOrTargetCellsCombination[baseCrossline])
							{
								foreach (var targetCells in BaseOrTargetCellsCombination[targetCrossline])
								{
									// Checks for base cells, collecting digits appeared in base cells.
									var digits = grid[baseCells];
									var digitsCount = PopCount((uint)digits);
									if (crosslineHousesCount == 3 && digitsCount > 5
										|| crosslineHousesCount is 2 or 4 && digitsCount != crosslineHousesCount)
									{
										continue;
									}


								}
							}
						}
					}
				}
			}
		}

		return null;
	}
}
