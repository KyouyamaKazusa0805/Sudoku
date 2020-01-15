using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	public static partial class RegionUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char GetRegionLabel(int regionOffset) => GetRegionName(regionOffset)[0];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToString(int regionOffset) =>
			$"{(char)(GetRegionLabel(regionOffset) + ' ')}{regionOffset % 9 + 1}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRegionName(int regionOffset) =>
			(new[] { "Block", "Row", "Column" })[regionOffset / 9];

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int> GetCellOffsets(int regionOffset) => Map[regionOffset];
	}
}
