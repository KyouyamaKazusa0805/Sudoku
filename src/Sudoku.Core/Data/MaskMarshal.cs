namespace Sudoku.Data;

/// <summary>
/// Provides extension methods on <see cref="short"/> value as a mask.
/// </summary>
/// <seealso cref="short"/>
public static class MaskMarshal
{
	/// <summary>
	/// To get the cell status through a mask.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static CellStatus MaskToStatus(this short mask) => (CellStatus)(mask >> 9 & (int)CellStatus.All);
}
