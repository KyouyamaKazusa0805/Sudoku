using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Encapsulates the converter methods of conversions between <see cref="int"/>
	/// and <see cref="RegionLabel"/>.
	/// </summary>
	/// <seealso cref="RegionLabel"/>
	public static class RegionLabelConverter
	{
		/// <summary>
		/// Get the label in the specified region.
		/// </summary>
		/// <param name="region">(<see langword="this"/> parameter) The region.</param>
		/// <returns>The region label.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RegionLabel ToLabel(this int region) => (RegionLabel)(region / 9);
	}
}
