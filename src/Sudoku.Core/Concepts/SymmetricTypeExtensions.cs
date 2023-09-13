using System.Runtime.CompilerServices;

namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="SymmetricType"/>.
/// </summary>
/// <seealso cref="SymmetricType"/>
public static class SymmetricTypeExtensions
{
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
			SymmetricType.Central => [row * 9 + column, (8 - row) * 9 + 8 - column],
			SymmetricType.Diagonal => [row * 9 + column, column * 9 + row],
			SymmetricType.AntiDiagonal => [row * 9 + column, (8 - column) * 9 + 8 - row],
			SymmetricType.XAxis => [row * 9 + column, (8 - row) * 9 + column],
			SymmetricType.YAxis => [row * 9 + column, row * 9 + 8 - column],
			SymmetricType.DiagonalBoth => [row * 9 + column, column * 9 + row, (8 - column) * 9 + 8 - row, (8 - row) * 9 + 8 - column],
			SymmetricType.AxisBoth => [row * 9 + column, (8 - row) * 9 + column, row * 9 + 8 - column, (8 - row) * 9 + 8 - column],
			SymmetricType.All
				=> [
					row * 9 + column,
					row * 9 + (8 - column),
					(8 - row) * 9 + column,
					(8 - row) * 9 + (8 - column),
					column * 9 + row,
					column * 9 + (8 - row),
					(8 - column) * 9 + row,
					(8 - column) * 9 + (8 - row)
				],
			SymmetricType.None => [row * 9 + column],
			_ => []
		};
}
