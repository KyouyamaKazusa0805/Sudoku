namespace Sudoku;

/// <summary>
/// Provides with solution-wide read-only fields used.
/// </summary>
public static class SolutionFields
{
	/// <summary>
	/// Indicates the number of each cell's peer cells. The value is a constant.
	/// </summary>
	public const Cell PeersCount = 20;


	/// <summary>
	/// Indicates the possible house types to be iterated.
	/// </summary>
	public static readonly HouseType[] HouseTypes = [HouseType.Block, HouseType.Row, HouseType.Column];

	/// <summary>
	/// Indicates the digits used. The value can be also used for ordered houses by rows.
	/// </summary>
	public static readonly Digit[] Digits = [0, 1, 2, 3, 4, 5, 6, 7, 8];

	/// <summary>
	/// Indicates the houses ordered by column.
	/// </summary>
	public static readonly Digit[] HousesOrderedByColumn = [0, 3, 6, 1, 4, 7, 2, 5, 8];

	/// <summary>
	/// Indicates the first cell offset for each house.
	/// </summary>
	public static readonly Cell[] HouseFirst = [0, 3, 6, 27, 30, 33, 54, 57, 60, 0, 9, 18, 27, 36, 45, 54, 63, 72, 0, 1, 2, 3, 4, 5, 6, 7, 8];

	/// <summary>
	/// <para>
	/// The map of all cell offsets in its specified house.
	/// The indices is between 0 and 26, where:
	/// <list type="table">
	/// <item>
	/// <term><c>0..9</c></term>
	/// <description>Block 1 to 9.</description>
	/// </item>
	/// <item>
	/// <term><c>9..18</c></term>
	/// <description>Row 1 to 9.</description>
	/// </item>
	/// <item>
	/// <term><c>18..27</c></term>
	/// <description>Column 1 to 9.</description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	/// <example>
	/// '<c>HouseCells[0]</c>': all cell offsets in the house 0 (block 1).
	/// </example>
	public static readonly Cell[][] HousesCells = [
		[0, 1, 2, 9, 10, 11, 18, 19, 20], [3, 4, 5, 12, 13, 14, 21, 22, 23], [6, 7, 8, 15, 16, 17, 24, 25, 26],
		[27, 28, 29, 36, 37, 38, 45, 46, 47], [30, 31, 32, 39, 40, 41, 48, 49, 50], [33, 34, 35, 42, 43, 44, 51, 52, 53],
		[54, 55, 56, 63, 64, 65, 72, 73, 74], [57, 58, 59, 66, 67, 68, 75, 76, 77], [60, 61, 62, 69, 70, 71, 78, 79, 80],
		[0, 1, 2, 3, 4, 5, 6, 7, 8], [9, 10, 11, 12, 13, 14, 15, 16, 17], [18, 19, 20, 21, 22, 23, 24, 25, 26],
		[27, 28, 29, 30, 31, 32, 33, 34, 35], [36, 37, 38, 39, 40, 41, 42, 43, 44], [45, 46, 47, 48, 49, 50, 51, 52, 53],
		[54, 55, 56, 57, 58, 59, 60, 61, 62], [63, 64, 65, 66, 67, 68, 69, 70, 71], [72, 73, 74, 75, 76, 77, 78, 79, 80],
		[0, 9, 18, 27, 36, 45, 54, 63, 72], [1, 10, 19, 28, 37, 46, 55, 64, 73], [2, 11, 20, 29, 38, 47, 56, 65, 74],
		[3, 12, 21, 30, 39, 48, 57, 66, 75], [4, 13, 22, 31, 40, 49, 58, 67, 76], [5, 14, 23, 32, 41, 50, 59, 68, 77],
		[6, 15, 24, 33, 42, 51, 60, 69, 78], [7, 16, 25, 34, 43, 52, 61, 70, 79], [8, 17, 26, 35, 44, 53, 62, 71, 80]
	];

	/// <summary>
	/// Indicates a list of <see cref="CellMap"/> instances representing the cells belong to a house at the specified index.
	/// </summary>
	public static readonly CellMap[] HousesMap;

	/// <summary>
	/// Indicates a list of <see cref="CellMap"/> instances representing the peer cells of a cell at the specified index.
	/// </summary>
	public static readonly CellMap[] PeersMap;

	/// <summary>
	/// Indicates the chute maps.
	/// </summary>
	public static readonly CellMap[] ChuteMaps;

	/// <summary>
	/// Indicates a list of <see cref="Chute"/> instances representing chutes.
	/// </summary>
	public static readonly Chute[] Chutes;

	/// <summary>
	/// Indicates a block list that each cell belongs to.
	/// </summary>
	internal static readonly BlockIndex[] BlockTable = [
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		6, 6, 6, 7, 7, 7, 8, 8, 8,
		6, 6, 6, 7, 7, 7, 8, 8, 8,
		6, 6, 6, 7, 7, 7, 8, 8, 8
	];

	/// <summary>
	/// Indicates a row list that each cell belongs to.
	/// </summary>
	internal static readonly RowIndex[] RowTable = [
		9, 9, 9, 9, 9, 9, 9, 9, 9,
		10, 10, 10, 10, 10, 10, 10, 10, 10,
		11, 11, 11, 11, 11, 11, 11, 11, 11,
		12, 12, 12, 12, 12, 12, 12, 12, 12,
		13, 13, 13, 13, 13, 13, 13, 13, 13,
		14, 14, 14, 14, 14, 14, 14, 14, 14,
		15, 15, 15, 15, 15, 15, 15, 15, 15,
		16, 16, 16, 16, 16, 16, 16, 16, 16,
		17, 17, 17, 17, 17, 17, 17, 17, 17
	];

	/// <summary>
	/// Indicates a column list that each cell belongs to.
	/// </summary>
	internal static readonly ColumnIndex[] ColumnTable = [
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26
	];

	/// <summary>
	/// Indicates a list of <see cref="CellMap"/> instances that are initialized as singleton element by its corresponding index.
	/// For example, <c>CellMaps[0]</c> is to <c>CellMap.Empty + 0</c>, i.e. <c>r1c1</c>.
	/// </summary>
	internal static readonly CellMap[] CellMaps;

#if CACHE_CANDIDATE_MAPS
	/// <summary>
	/// Indicates a list of <see cref="CandidateMap"/> instances that are initialized as singleton element by its corresponding index.
	/// For example, <c>CandidateMaps[0]</c> is to <c>CandidateMap.Empty + 0</c>, i.e. <c>r1c1(1)</c>.
	/// </summary>
	internal static readonly CandidateMap[] CandidateMaps;
#endif

	/// <summary>
	/// Indicates the chute house triplets.
	/// </summary>
	private static readonly (House, House, House)[] ChuteHouses = [(9, 10, 11), (12, 13, 14), (15, 16, 17), (18, 19, 20), (21, 22, 23), (24, 25, 26)];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static SolutionFields()
	{
		//
		// HousesMap
		//
		{
			HousesMap = new CellMap[27];
			for (var house = 0; house < 27; house++)
			{
				HousesMap[house] = HousesCells[house].AsCellMap();
			}
		}

		//
		// CellMaps
		//
		{
			CellMaps = new CellMap[81];
			var span = CellMaps.AsSpan();
			var cell = 0;
			foreach (ref var map in span)
			{
				map.Add(cell++);
			}
		}

		//
		// PeersMap
		//
		{
			PeersMap = new CellMap[81];
			for (var cell = 0; cell < 81; cell++)
			{
				var map = CellMap.Empty;
				for (var peerCell = 0; peerCell < 81; peerCell++)
				{
					if (cell != peerCell)
					{
						foreach (var houseType in HouseTypes)
						{
							if (HousesMap[cell.ToHouse(houseType)].Contains(peerCell))
							{
								map.Add(peerCell);
								break;
							}
						}
					}
					if (map.Count == PeersCount)
					{
						break;
					}
				}
				PeersMap[cell] = map;
			}
		}

#if CACHE_CANDIDATE_MAPS
		//
		// CandidateMaps
		//
		{
			CandidateMaps = new CandidateMap[729];
			var span = CandidateMaps.AsSpan();
			var candidate = 0;
			foreach (ref var map in span)
			{
				map.Add(candidate++);
			}
		}
#endif

		//
		// ChuteMaps
		//
		{
			ChuteMaps = new CellMap[6];
			for (var chute = 0; chute < 3; chute++)
			{
				var ((r1, r2, r3), (c1, c2, c3)) = (ChuteHouses[chute], ChuteHouses[chute + 3]);
				(ChuteMaps[chute], ChuteMaps[chute + 3]) = (HousesMap[r1] | HousesMap[r2] | HousesMap[r3], HousesMap[c1] | HousesMap[c2] | HousesMap[c3]);
			}
		}

		//
		// Chutes
		//
		{
			Chutes = new Chute[6];
			for (var chute = 0; chute < 3; chute++)
			{
				var ((r1, r2, r3), (c1, c2, c3)) = (ChuteHouses[chute], ChuteHouses[chute + 3]);
				(Chutes[chute], Chutes[chute + 3]) = (
					new(chute, true, 1 << r1 | 1 << r2 | 1 << r3),
					new(chute + 3, false, 1 << c1 | 1 << c2 | 1 << c3)
				);
			}
		}
	}
}
