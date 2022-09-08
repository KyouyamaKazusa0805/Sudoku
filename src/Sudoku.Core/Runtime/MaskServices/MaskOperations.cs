namespace Sudoku.Runtime.MaskServices;

/// <summary>
/// Provides with a set of methods that operates with mask defined in basic sudoku concepts, as data structures.
/// </summary>
public static class MaskOperations
{
	/// <summary>
	/// To get the cell status for a mask value. The mask is an inner representation to describe a cell's state.
	/// For more information please visit the details of the design for type <see cref="Grid"/>.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell status.</returns>
	/// <seealso cref="Grid"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellStatus MaskToStatus(short mask) => (CellStatus)(mask >> 9 & 7);
}
