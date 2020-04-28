using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static Sudoku.GridProcessings;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for region offsets.
	/// </summary>
	[DebuggerStepThrough]
	public static class RegionUtils
	{
		/// <summary>
		/// Gets a string of a region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToString(int regionOffset) =>
			$"{(char)(GetRegionName(regionOffset).ToLower()[0] + ' ')}{regionOffset % 9 + 1}";

		/// <summary>
		/// Get the region name ('row', 'column' or 'block') of the specified
		/// region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRegionName(int regionOffset) => (new[] { "Block", "Row", "Column" })[regionOffset / 9];
	}
}
