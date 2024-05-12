namespace Sudoku.Concepts;

/// <summary>
/// Represents a pair of intersection information.
/// </summary>
/// <param name="Base">Indicates the base that describes the block and line index.</param>
/// <param name="Result">Indicates the result values.</param>
public readonly record struct Intersection(ref readonly IntersectionBase Base, ref readonly IntersectionResult Result)
{
	/// <summary>
	/// Indicates the mini-lines to be iterated, grouped by chute index.
	/// </summary>
	public static readonly CellMap[][] MinilinesGroupedByChuteIndex;

	/// <summary>
	/// <para>
	/// Indicates all maps that forms the each intersection. The pattern will be like:
	/// <code><![CDATA[
	/// .-------.-------.-------.
	/// | C C C | A A A | A A A |
	/// | B B B | . . . | . . . |
	/// | B B B | . . . | . . . |
	/// '-------'-------'-------'
	/// ]]></code>
	/// </para>
	/// <para>
	/// In addition, in this data pattern, a <b>CoverSet</b> is a block and a <b>BaseSet</b> is a line.
	/// </para>
	/// </summary>
	public static readonly FrozenDictionary<IntersectionBase, IntersectionResult> IntersectionMaps;

	/// <summary>
	/// Indicates the internal intersection block combinations.
	/// </summary>
	private static readonly byte[][] IntersectionBlockTable = [
		[1, 2], [0, 2], [0, 1], [1, 2], [0, 2], [0, 1], [1, 2], [0, 2], [0, 1],
		[4, 5], [3, 5], [3, 4], [4, 5], [3, 5], [3, 4], [4, 5], [3, 5], [3, 4],
		[7, 8], [6, 8], [6, 7], [7, 8], [6, 8], [6, 7], [7, 8], [6, 8], [6, 7],
		[3, 6], [0, 6], [0, 3], [3, 6], [0, 6], [0, 3], [3, 6], [0, 6], [0, 3],
		[4, 7], [1, 7], [1, 4], [4, 7], [1, 7], [1, 4], [4, 7], [1, 7], [1, 4],
		[5, 8], [2, 8], [2, 5], [5, 8], [2, 8], [2, 5], [5, 8], [2, 8], [2, 5]
	];


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Intersection()
	{
		MinilinesGroupedByChuteIndex = new CellMap[6][];
		for (var i = 0; i < 6; i++)
		{
			ref var currentMinilineGroup = ref MinilinesGroupedByChuteIndex[i];
			currentMinilineGroup = [[], [], [], [], [], [], [], [], []];

			var ((_, _, _, chuteHouses), isRow, tempIndex) = (Chutes[i], i is 0 or 1 or 2, 0);
			foreach (var chuteHouse in chuteHouses)
			{
				for (var (houseCell, j) = (HouseFirst[chuteHouse], 0); j < 3; houseCell += isRow ? 3 : 27, j++)
				{
					ref var current = ref currentMinilineGroup[tempIndex++];
					current.Add(houseCell);
					current.Add(houseCell + (isRow ? 1 : 9));
					current.Add(houseCell + (isRow ? 2 : 18));
				}
			}
		}

		var dic = new Dictionary<IntersectionBase, IntersectionResult>();
		for (var bs = (byte)9; bs < 27; bs++)
		{
			for (var j = (byte)0; j < 3; j++)
			{
				var cs = bs < 18 ? Digits[(bs - 9) / 3 * 3 + j] : HousesOrderedByColumn[(bs - 18) / 3 * 3 + j];
				ref readonly var bm = ref HousesMap[bs];
				ref readonly var cm = ref HousesMap[cs];
				var i = bm & cm;
				dic.Add(new(bs, (byte)cs), new(bm & ~i, cm & ~i, in i, IntersectionBlockTable[(bs - 9) * 3 + j]));
			}
		}

		IntersectionMaps = dic.ToFrozenDictionary(EqualityComparing.Create<IntersectionBase>(static (x, y) => x == y, static v => v.Line << 5 | v.Block));
	}
}
