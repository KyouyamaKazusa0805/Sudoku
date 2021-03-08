#if DOUBLE_LAYERED_ASSUMPTION

using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="RegionLabel"/>.
	/// </summary>
	/// <seealso cref="RegionLabel"/>
	public static class RegionLabelEx
	{
		/// <summary>
		/// Get the specified region cause with the specified region label.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The region label.</param>
		/// <returns>The cause.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cause GetRegionCause(this RegionLabel @this) => @this switch
		{
			RegionLabel.Row => Cause.RowHiddenSingle,
			RegionLabel.Column => Cause.ColumnHiddenSingle,
			RegionLabel.Block => Cause.BlockHiddenSingle
		};
	}
}

#endif
