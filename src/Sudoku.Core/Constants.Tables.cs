#pragma warning disable CS8618, IDE0079

using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku
{
	partial class Constants
	{
		/// <summary>
		/// The tables for grid processing. All fields will be initialized in the static constructor.
		/// </summary>
		public static partial class Tables
		{
			/// <summary>
			/// The table of all blocks to iterate for each blocks.
			/// </summary>
			public static readonly byte[][] IntersectionBlockTable;

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
			/// <para>
			/// Indicates all maps that forms the each intersection. The pattern will be like:
			/// <code>
			/// .-------.-------.-------.
			/// | C C C | A A A | A A A |
			/// | B B B | . . . | . . . |
			/// | B B B | . . . | . . . |
			/// '-------'-------'-------'
			/// </code>
			/// </para>
			/// <para>
			/// In addition, in this data structure, "<c>CoverSet</c>" is a block and "<c>BaseSet</c>" is a line.
			/// </para>
			/// </summary>
			public static readonly IReadOnlyDictionary<
				(byte Line, byte Block),
				(Cells LineMap, Cells BlockMap, Cells IntersectionMap, byte[] OtherBlocks)
			> IntersectionMaps;

			/// <summary>
			/// Indicates the cover table that only used for <see cref="Cells"/>.
			/// </summary>
			/// <seealso cref="Cells"/>
			internal static readonly long[,] CellsCoverTable;
		}
	}
}
