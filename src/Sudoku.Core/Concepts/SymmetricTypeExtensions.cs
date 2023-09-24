using System.Runtime.CompilerServices;

namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="SymmetricType"/>.
/// </summary>
/// <seealso cref="SymmetricType"/>
public static class SymmetricTypeExtensions
{
	/// <summary>
	/// Try to get all possible cells in symmetry axis (or point).
	/// </summary>
	/// <param name="this">The symmetry.</param>
	/// <returns>Returns cells in the symmetry axis (or point).</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="this"/> is not defined or <see cref="SymmetricType.None"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap GetCellsInSymmetryAxis(this SymmetricType @this)
		=> @this switch
		{
			SymmetricType.Central => [40],
			SymmetricType.Diagonal => [0, 10, 20, 30, 40, 50, 60, 70, 80],
			SymmetricType.AntiDiagonal => [8, 16, 24, 32, 40, 48, 56, 64, 72],
			SymmetricType.XAxis => [36, 37, 38, 39, 40, 41, 42, 43, 44],
			SymmetricType.YAxis => [4, 13, 22, 31, 40, 49, 58, 67, 76],
			SymmetricType.AxisBoth => SymmetricType.XAxis.GetCellsInSymmetryAxis() | SymmetricType.YAxis.GetCellsInSymmetryAxis(),
			SymmetricType.DiagonalBoth => SymmetricType.Diagonal.GetCellsInSymmetryAxis() | SymmetricType.AntiDiagonal.GetCellsInSymmetryAxis(),
			SymmetricType.All => SymmetricType.AxisBoth.GetCellsInSymmetryAxis() | SymmetricType.DiagonalBoth.GetCellsInSymmetryAxis(),
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
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
			_ => (Cell[])[]
		};
}
