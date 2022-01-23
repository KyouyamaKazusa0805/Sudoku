#nullable disable warnings

namespace Sudoku;

partial class Constants
{
	/// <summary>
	/// The tables for grid processing.
	/// </summary>
	/// <remarks><i>
	/// All fields will be initialized in the static constructor, which is declared in the source generator.
	/// </i></remarks>
	public static partial class Tables
	{
		/// <summary>
		/// Indicates the first cell offset for each region.
		/// </summary>
		public static readonly int[] RegionFirst;

		/// <summary>
		/// <para>Indicates a table for each cell's peers.</para>
		/// </summary>
		/// <example>
		/// '<c>Peers[0]</c>': the array of peers for the cell 0 (row 1 column 1).
		/// </example>
		public static readonly int[][] Peers;

		/// <summary>
		/// <para>
		/// The map of all cell offsets in its specified region.
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
		/// '<c>RegionTable[0]</c>': all cell offsets in the region 0 (block 1).
		/// </example>
		public static readonly int[][] RegionCells;

		/// <summary>
		/// Indicates all grid maps that a grid contains.
		/// </summary>
		/// <example>
		/// '<c>RegionMaps[0]</c>': The map containing all cells in the block 1.
		/// </example>
		public static readonly Cells[] RegionMaps;

		/// <summary>
		/// Indicates the peer maps using <see cref="Peers"/> table.
		/// </summary>
		/// <seealso cref="Peers"/>
		public static readonly Cells[] PeerMaps;

		/// <summary>
		/// Indicates the possible region types to iterate.
		/// </summary>
		public static readonly Region[] Regions = new[] { Region.Block, Region.Row, Region.Column };

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
		public static readonly IReadOnlyDictionary<(byte Line, byte Block), (Cells LineMap, Cells BlockMap, Cells IntersectionMap, byte[] OtherBlocks)> IntersectionMaps;

		/// <summary>
		/// <para>The table of all blocks to iterate for each blocks.</para>
		/// <para>
		/// This field is only used for providing the data for another field <see cref="IntersectionMaps"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="IntersectionMaps"/>
		internal static readonly byte[][] IntersectionBlockTable;

		/// <summary>
		/// Indicates the combinatorial numbers from <c>C(1, 1)</c> to <c>C(30, 30)</c>.
		/// </summary>
		internal static readonly int[,] Combinatorials;
	}
}
