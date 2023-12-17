using System.ComponentModel;
using System.Diagnostics;
using static System.Numerics.BitOperations;

namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of methods to filter the cells.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
internal static class GridCellPredicates
{
	/// <summary>
	/// Determines whether the specified cell in the specified grid is a given cell.
	/// </summary>
	/// <param name="g">The grid.</param>
	/// <param name="cell">The cell to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool GivenCells(scoped ref readonly Grid g, Cell cell) => g.GetState(cell) == CellState.Given;

	/// <summary>
	/// Determines whether the specified cell in the specified grid is a modifiable cell.
	/// </summary>
	/// <inheritdoc cref="GivenCells(ref readonly Grid, Cell)"/>
	public static bool ModifiableCells(scoped ref readonly Grid g, Cell cell) => g.GetState(cell) == CellState.Modifiable;

	/// <summary>
	/// Determines whether the specified cell in the specified grid is an empty cell.
	/// </summary>
	/// <inheritdoc cref="GivenCells(ref readonly Grid, Cell)"/>
	public static bool EmptyCells(scoped ref readonly Grid g, Cell cell) => g.GetState(cell) == CellState.Empty;

	/// <summary>
	/// Determines whether the specified cell in the specified grid is a bi-value cell, which means the cell is an empty cell,
	/// and contains and only contains 2 candidates.
	/// </summary>
	/// <inheritdoc cref="GivenCells(ref readonly Grid, Cell)"/>
	public static bool BivalueCells(scoped ref readonly Grid g, Cell cell) => PopCount((uint)g.GetCandidates(cell)) == 2;

	/// <summary>
	/// Checks the existence of the specified digit in the specified cell.
	/// </summary>
	/// <param name="g">The grid.</param>
	/// <param name="cell">The cell to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool CandidatesMap(scoped ref readonly Grid g, Cell cell, Digit digit) => g.Exists(cell, digit) is true;

	/// <summary>
	/// Checks the existence of the specified digit in the specified cell, or whether the cell is a value cell, being filled by the digit.
	/// </summary>
	/// <inheritdoc cref="CandidatesMap(ref readonly Grid, Cell, Digit)"/>
	public static bool DigitsMap(scoped ref readonly Grid g, Cell cell, Digit digit) => (g.GetCandidates(cell) >> digit & 1) != 0;

	/// <summary>
	/// Checks whether the cell is a value cell, being filled by the digit.
	/// </summary>
	/// <inheritdoc cref="CandidatesMap(ref readonly Grid, Cell, Digit)"/>
	public static bool ValuesMap(scoped ref readonly Grid g, Cell cell, Digit digit) => g.GetDigit(cell) == digit;
}
