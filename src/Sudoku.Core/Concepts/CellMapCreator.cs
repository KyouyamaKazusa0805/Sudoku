namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that has ability to create <see cref="CellMap"/> instances called by compiler.
/// For the users' aspect, we can just use collection expressions.
/// </summary>
/// <seealso cref="CellMap"/>
public static class CellMapCreator
{
	/// <summary>
	/// Creates a <see cref="CellMap"/> instance.
	/// </summary>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Create() => CellMap.Empty;

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance via the specified cells.
	/// </summary>
	/// <param name="cells">The cells.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap Create(scoped ReadOnlySpan<Cell> cells)
	{
		var result = CellMap.Empty;
		foreach (var cell in cells)
		{
			result.Add(cell);
		}
		return result;
	}
}
