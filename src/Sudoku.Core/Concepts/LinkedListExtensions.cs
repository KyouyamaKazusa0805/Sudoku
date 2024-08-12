namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="LinkedList{T}"/>.
/// </summary>
/// <seealso cref="LinkedList{T}"/>
public static class LinkedListExtensions
{
	/// <summary>
	/// Creates a <see cref="CellMap"/> instance via a linked list.
	/// </summary>
	/// <param name="this">A linked list to be used.</param>
	/// <returns>A <see cref="CellMap"/> instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap AsCellMap(this LinkedList<Cell> @this) => [.. @this];
}
