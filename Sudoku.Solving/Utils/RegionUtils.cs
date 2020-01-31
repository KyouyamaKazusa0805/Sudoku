using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for region offsets.
	/// </summary>
	public static partial class RegionUtils
	{
		/// <summary>
		/// Gets the label ('r', 'c' or 'b') of the specified region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The label.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char GetRegionLabel(int regionOffset) => GetRegionName(regionOffset)[0];

		/// <summary>
		/// Gets a string of a region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToString(int regionOffset) =>
			$"{(char)(GetRegionLabel(regionOffset) + ' ')}{regionOffset % 9 + 1}";

		/// <summary>
		/// Get the region name ('row', 'column' or 'block') of the specified
		/// region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRegionName(int regionOffset) =>
			(new[] { "Block", "Row", "Column" })[regionOffset / 9];

		/// <summary>
		/// Get the cell offset of the relative position in the specified
		/// region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <param name="relativePos">The relative position.</param>
		/// <returns>The cell offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetCellOffset(int regionOffset, int relativePos)
		{
			return regionOffset switch
			{
				_ when regionOffset < 9 =>
					(regionOffset / 3 * 3 + relativePos / 3) * 9 + regionOffset % 3 * 3 + relativePos % 3,
				_ when regionOffset < 18 => (regionOffset - 9) * 9 + relativePos,
				_ => relativePos * 9 + (regionOffset - 18)
			};
		}

		/// <summary>
		/// Get the region offset from a string text.
		/// </summary>
		/// <param name="regionString">The text of region.</param>
		/// <returns>The region offset.</returns>
		public static int GetRegionOffset(string regionString)
		{
			return regionString[0] switch
			{
				'b' => regionString[1] - '1',
				'r' => regionString[1] - '1' + 9,
				'c' => regionString[1] - '1' + 18,
				_ => throw new ArgumentException(nameof(regionString))
			};
		}

		/// <summary>
		/// Get all cell offsets in the specified region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>All cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int[] GetCellOffsets(int regionOffset) => Map[regionOffset];
	}
}
