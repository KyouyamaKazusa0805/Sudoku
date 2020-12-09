using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="SymmetryType"/>.
	/// </summary>
	/// <seealso cref="SymmetryType"/>
	public static class SymmetryTypeEx
	{
		/// <summary>
		/// Get the name of the current symmetry type.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The type.</param>
		/// <returns>The name.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetName(this SymmetryType @this) =>
			@this switch
			{
				SymmetryType.None => "No symmetry",
				SymmetryType.Central => "Central symmetry type",
				SymmetryType.Diagonal => "Diagonal symmetry type",
				SymmetryType.AntiDiagonal => "Anti-diagonal symmetry type",
				SymmetryType.XAxis => "X-axis symmetry type",
				SymmetryType.YAxis => "Y-axis symmetry type",
				SymmetryType.AxisBoth => "Both X-axis and Y-axis",
				SymmetryType.DiagonalBoth => "Both diagonal and anti-diagonal",
				SymmetryType.All => "All symmetry type"
			};
	}
}
