#if DOUBLE_LAYERED_ASSUMPTION

using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="Cause"/>.
	/// </summary>
	/// <seealso cref="Cause"/>
	public static class CauseEx
	{
		/// <summary>
		/// Get the specified region label with the specified cause.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The cause.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionLabel GetRegionLabel(this Cause @this) => @this switch
		{
			Cause.BlockHiddenSingle => RegionLabel.Block,
			Cause.RowHiddenSingle => RegionLabel.Row,
			Cause.ColumnHiddenSingle => RegionLabel.Column
		};
	}
}

#endif