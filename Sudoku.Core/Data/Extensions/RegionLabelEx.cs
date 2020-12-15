using System.Runtime.CompilerServices;
using Sudoku.Constants;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="RegionLabel"/>.
	/// </summary>
	/// <seealso cref="RegionLabel"/>
	public static class RegionLabelEx
	{
		/// <summary>
		/// Get the region index for the specified cell and the region type.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="@this">(<see langword="this"/> parameter) The label.</param>
		/// <returns>The region index (<c>0..27</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToRegion(this RegionLabel @this, int cell) =>
		(
			@this switch
			{
				RegionLabel.Row => Processings.RowTable,
				RegionLabel.Column => Processings.ColumnTable,
				RegionLabel.Block => Processings.BlockTable
			}
		)[cell];

#if DOUBLE_LAYERED_ASSUMPTION
		/// <summary>
		/// Get the specified region cause with the specified region label.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The region label.</param>
		/// <returns>The cause.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cause GetRegionCause(this RegionLabel @this) =>
			@this switch
			{
				RegionLabel.Row => Cause.RowHiddenSingle,
				RegionLabel.Column => Cause.ColumnHiddenSingle,
				RegionLabel.Block => Cause.BlockHiddenSingle
			};
#endif
	}
}
