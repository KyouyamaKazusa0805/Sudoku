using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="short"/>.
	/// </summary>
	/// <seealso cref="short"/>
	public static class Int16Ex
	{
		/// <summary>
		/// To get the cell status through a mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		/// <returns>The cell status.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static CellStatus MaskToStatus(this short mask) => (CellStatus)(mask >> 9 & (int)CellStatus.All);
	}
}
