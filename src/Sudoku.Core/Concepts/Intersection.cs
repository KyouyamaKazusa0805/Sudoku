namespace Sudoku.Concepts;

/// <summary>
/// Defines a type that stores some fields as shared one.
/// </summary>
public static class Intersection
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


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Intersection()
	{
		MinilinesGroupedByChuteIndex = new CellMap[6][];
		for (var i = 0; i < 6; i++)
		{
			scoped ref var currentMinilineGroup = ref MinilinesGroupedByChuteIndex[i];
			currentMinilineGroup = [[], [], [], [], [], [], [], [], []];

			var ((_, _, _, chuteHouses), isRow, tempIndex) = (Chutes[i], i is 0 or 1 or 2, 0);
			foreach (var chuteHouse in chuteHouses)
			{
				for (var (houseCell, j) = (HouseFirst[chuteHouse], 0); j < 3; houseCell += isRow ? 3 : 27, j++)
				{
					scoped ref var current = ref currentMinilineGroup[tempIndex++];
					current.Add(houseCell);
					current.Add(houseCell + (isRow ? 1 : 9));
					current.Add(houseCell + (isRow ? 2 : 18));
				}
			}
		}


#pragma warning disable format
		var intersectionBlockTable = (byte[][])[
			[1, 2], [0, 2], [0, 1], [1, 2], [0, 2], [0, 1], [1, 2], [0, 2], [0, 1],
			[4, 5], [3, 5], [3, 4], [4, 5], [3, 5], [3, 4], [4, 5], [3, 5], [3, 4],
			[7, 8], [6, 8], [6, 7], [7, 8], [6, 8], [6, 7], [7, 8], [6, 8], [6, 7],
			[3, 6], [0, 6], [0, 3], [3, 6], [0, 6], [0, 3], [3, 6], [0, 6], [0, 3],
			[4, 7], [1, 7], [1, 4], [4, 7], [1, 7], [1, 4], [4, 7], [1, 7], [1, 4],
			[5, 8], [2, 8], [2, 5], [5, 8], [2, 8], [2, 5], [5, 8], [2, 8], [2, 5]
		];
#pragma warning restore format

		scoped var r = (ReadOnlySpan<byte>)[0, 1, 2, 3, 4, 5, 6, 7, 8];
		scoped var c = (ReadOnlySpan<byte>)[0, 3, 6, 1, 4, 7, 2, 5, 8];
		var dic = new Dictionary<IntersectionBase, IntersectionResult>();
		for (var bs = (byte)9; bs < 27; bs++)
		{
			for (var j = (byte)0; j < 3; j++)
			{
				var cs = bs < 18 ? r[(bs - 9) / 3 * 3 + j] : c[(bs - 18) / 3 * 3 + j];
				scoped ref readonly var bm = ref HousesMap[bs];
				scoped ref readonly var cm = ref HousesMap[cs];
				var i = bm & cm;
				dic.Add(new(bs, cs), new(bm - i, cm - i, in i, intersectionBlockTable[(bs - 9) * 3 + j]));
			}
		}

		IntersectionMaps = dic.ToFrozenDictionary(new EqualityComparer());
	}
}

/// <summary>
/// Represents a comparer instance that compares two tuples.
/// </summary>
file sealed class EqualityComparer : IEqualityComparer<IntersectionBase>
{
	/// <inheritdoc/>
	public bool Equals(IntersectionBase x, IntersectionBase y) => x == y;

	/// <inheritdoc/>
	public int GetHashCode(IntersectionBase obj) => obj.Line << 5 | obj.Block;
}
