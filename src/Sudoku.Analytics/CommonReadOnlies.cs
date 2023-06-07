namespace Sudoku.Analytics;

/// <summary>
/// Represents a type holding some common read-only fields used by runtime or compiling-time.
/// </summary>
public static class CommonReadOnlies
{
	/// <summary>
	/// Indicates the total number of unique square patterns.
	/// </summary>
	public const int UniqueSquareTemplatesCount = 162;

	/// <summary>
	/// Indicates the total number of Qiu's deadly patterns.
	/// </summary>
	public const int QiuDeadlyPatternTemplatesCount = 972;

	/// <summary>
	/// Indicates the total number of exocet patterns.
	/// </summary>
	public const int ExocetTemplatesCount = 1458;

	/// <summary>
	/// Indicates the total number of firework subset patterns.
	/// </summary>
	public const int FireworkSubsetCount = 3645;

	/// <summary>
	/// Indicates the total number of Unique Polygon (Heptagon) possible templates of size 3.
	/// </summary>
	public const int BdpTemplatesSize3Count = 14580;

	/// <summary>
	/// Indicates the total number of Unique Polygon (Octagon) possible templates of size 4.
	/// </summary>
	public const int BdpTemplatesSize4Count = 11664;

	/// <summary>
	/// Indicates the total number of pair fireworks.
	/// </summary>
	public const int PairFireworksCount = 103518;

	/// <summary>
	/// Indicates the total number of multi-sector locked sets possible templates.
	/// </summary>
	public const int MultisectorLockedSetsTemplatesCount = 74601;

	/// <summary>
	/// Indicates all houses iterating on the specified block forming an empty rectangle.
	/// </summary>
	public static readonly House[,] EmptyRectangleLinkIds =
	{
		{ 12, 13, 14, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
		{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
		{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
		{  9, 10, 11, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
		{  9, 10, 11, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
		{  9, 10, 11, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
		{  9, 10, 11, 12, 13, 14, 21, 22, 23, 24, 25, 26 },
		{  9, 10, 11, 12, 13, 14, 18, 19, 20, 24, 25, 26 },
		{  9, 10, 11, 12, 13, 14, 18, 19, 20, 21, 22, 23 }
	};

	/// <summary>
	/// All possible blocks combinations being reserved for chromatic pattern searcher's usages.
	/// </summary>
	public static readonly Mask[] ChromaticPatternBlocksCombinations =
	{
		0b000_011_011, 0b000_101_101, 0b000_110_110,
		0b011_000_011, 0b101_000_101, 0b110_000_110,
		0b011_011_000, 0b101_101_000, 0b110_110_000
	};

	/// <summary>
	/// The order of cell offsets to get values.
	/// </summary>
	/// <remarks>
	/// For example, the first value is 40, which means the first cell to be tried to be filled
	/// is the 40th cell in the grid (i.e. the cell <c>r5c5</c>).
	/// </remarks>
	public static readonly Cell[] BruteForceTryAndErrorOrder =
	{
		40, 41, 32, 31, 30, 39, 48, 49, 50,
		51, 42, 33, 24, 23, 22, 21, 20, 29,
		38, 47, 56, 57, 58, 59, 60, 61, 52,
		43, 34, 25, 16, 15, 14, 13, 12, 11,
		10, 19, 28, 37, 46, 55, 64, 65, 66,
		67, 68, 69, 70, 71, 62, 53, 44, 35,
		26, 17,  8,  7,  6,  5,  4,  3,  2,
		 1,  0,  9, 18, 27, 36, 45, 54, 63,
		72, 73, 74, 75, 76, 77, 78, 79, 80
	};

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
	/// In addition, in this data structure, a <b>CoverSet</b> is a block and a <b>BaseSet</b> is a line.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyDictionary<HousePair, HouseCellsTuple> IntersectionMaps;

	/// <summary>
	/// <para>The table of all blocks to iterate for each blocks.</para>
	/// <para>
	/// This field is only used for providing the data for another field <see cref="IntersectionMaps"/>.
	/// </para>
	/// </summary>
	/// <seealso cref="IntersectionMaps"/>
	internal static readonly byte[][] IntersectionBlockTable =
	{
		new byte[] {  1,  2 }, new byte[] {  0,  2 }, new byte[] {  0,  1 },
		new byte[] {  1,  2 }, new byte[] {  0,  2 }, new byte[] {  0,  1 },
		new byte[] {  1,  2 }, new byte[] {  0,  2 }, new byte[] {  0,  1 },
		new byte[] {  4,  5 }, new byte[] {  3,  5 }, new byte[] {  3,  4 },
		new byte[] {  4,  5 }, new byte[] {  3,  5 }, new byte[] {  3,  4 },
		new byte[] {  4,  5 }, new byte[] {  3,  5 }, new byte[] {  3,  4 },
		new byte[] {  7,  8 }, new byte[] {  6,  8 }, new byte[] {  6,  7 },
		new byte[] {  7,  8 }, new byte[] {  6,  8 }, new byte[] {  6,  7 },
		new byte[] {  7,  8 }, new byte[] {  6,  8 }, new byte[] {  6,  7 },
		new byte[] {  3,  6 }, new byte[] {  0,  6 }, new byte[] {  0,  3 },
		new byte[] {  3,  6 }, new byte[] {  0,  6 }, new byte[] {  0,  3 },
		new byte[] {  3,  6 }, new byte[] {  0,  6 }, new byte[] {  0,  3 },
		new byte[] {  4,  7 }, new byte[] {  1,  7 }, new byte[] {  1,  4 },
		new byte[] {  4,  7 }, new byte[] {  1,  7 }, new byte[] {  1,  4 },
		new byte[] {  4,  7 }, new byte[] {  1,  7 }, new byte[] {  1,  4 },
		new byte[] {  5,  8 }, new byte[] {  2,  8 }, new byte[] {  2,  5 },
		new byte[] {  5,  8 }, new byte[] {  2,  8 }, new byte[] {  2,  5 },
		new byte[] {  5,  8 }, new byte[] {  2,  8 }, new byte[] {  2,  5 }
	};


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static CommonReadOnlies()
	{
		scoped var r = (stackalloc[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
		scoped var c = (stackalloc[] { 0, 3, 6, 1, 4, 7, 2, 5, 8 });
		var dic = new Dictionary<HousePair, HouseCellsTuple>(new ValueTupleComparer());
		for (byte bs = 9; bs < 27; bs++)
		{
			for (byte j = 0; j < 3; j++)
			{
				var cs = (byte)(bs < 18 ? r[(bs - 9) / 3 * 3 + j] : c[(bs - 18) / 3 * 3 + j]);
				scoped ref readonly var bm = ref HousesMap[bs];
				scoped ref readonly var cm = ref HousesMap[cs];
				var i = bm & cm;
				dic.Add((bs, cs), (bm - i, cm - i, i, IntersectionBlockTable[(bs - 9) * 3 + j]));
			}
		}

		IntersectionMaps = dic;
	}
}

/// <summary>
/// Represents a comparer instance that compares two tuples.
/// </summary>
file sealed class ValueTupleComparer : IEqualityComparer<HousePair>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(HousePair x, HousePair y) => x == y;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(HousePair obj) => obj.Line << 5 | obj.Block;
}
