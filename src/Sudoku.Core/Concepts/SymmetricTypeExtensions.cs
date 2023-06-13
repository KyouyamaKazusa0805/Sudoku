namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="SymmetricType"/>.
/// </summary>
/// <seealso cref="SymmetricType"/>
public static class SymmetricTypeExtensions
{
	/// <summary>
	/// Try to get excluded cells for the specified symmetric type.
	/// </summary>
	/// <param name="this">The symmetric type.</param>
	/// <returns>The excluded cell.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<Cell> GetExcludedCells(this SymmetricType @this)
		=> @this switch
		{
			SymmetricType.None => Array.Empty<Cell>(),
			SymmetricType.Central => new[] { 40 },
			SymmetricType.Diagonal => new[] { 0, 10, 20, 30, 40, 50, 60, 70, 80 },
			SymmetricType.AntiDiagonal => new[] { 8, 16, 24, 32, 40, 48, 56, 64, 72 },
			SymmetricType.XAxis => new[] { 36, 37, 38, 39, 40, 41, 42, 43, 44 },
			SymmetricType.YAxis => new[] { 4, 13, 22, 31, 40, 49, 58, 67, 76 },
			SymmetricType.AxisBoth => new[] { 36, 37, 38, 39, 40, 41, 42, 43, 44, 4, 13, 22, 31, 49, 58, 67, 76 },
			SymmetricType.DiagonalBoth => new[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 8, 16, 24, 32, 48, 56, 64, 72 },
			SymmetricType.All => new[] { 36, 37, 38, 39, 40, 41, 42, 43, 44, 4, 13, 22, 31, 49, 58, 67, 76, 0, 10, 20, 30, 50, 60, 70, 80, 8, 16, 24, 32, 48, 56, 64, 72 },
		};

	/// <summary>
	/// Get the cells that is used for swapping via the specified symmetric type, and the specified row and column value.
	/// </summary>
	/// <param name="this">The symmetric type.</param>
	/// <param name="row">The row value.</param>
	/// <param name="column">The column value.</param>
	/// <returns>The cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<Cell> GetCells(this SymmetricType @this, int row, int column)
		=> @this switch
		{
			SymmetricType.Central => new[] { row * 9 + column, (8 - row) * 9 + 8 - column },
			SymmetricType.Diagonal => new[] { row * 9 + column, column * 9 + row },
			SymmetricType.AntiDiagonal => new[] { row * 9 + column, (8 - column) * 9 + 8 - row },
			SymmetricType.XAxis => new[] { row * 9 + column, (8 - row) * 9 + column },
			SymmetricType.YAxis => new[] { row * 9 + column, row * 9 + 8 - column },
			SymmetricType.DiagonalBoth => new[] { row * 9 + column, column * 9 + row, (8 - column) * 9 + 8 - row, (8 - row) * 9 + 8 - column },
			SymmetricType.AxisBoth => new[] { row * 9 + column, (8 - row) * 9 + column, row * 9 + 8 - column, (8 - row) * 9 + 8 - column },
			SymmetricType.All
				=> new[]
				{
					row * 9 + column,
					row * 9 + (8 - column),
					(8 - row) * 9 + column,
					(8 - row) * 9 + (8 - column),
					column * 9 + row,
					column * 9 + (8 - row),
					(8 - column) * 9 + row,
					(8 - column) * 9 + (8 - row)
				},
			SymmetricType.None => new[] { row * 9 + column },
			_ => Array.Empty<Cell>()
		};
}
