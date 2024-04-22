namespace Sudoku.Concepts;

/// <summary>
/// Represents a data that describes for a strong link used by patterns using link logic.
/// </summary>
/// <param name="House">The house.</param>
/// <param name="Start">The start node.</param>
/// <param name="End">The end node.</param>
/// <param name="SpannedHouses">The spanned houses.</param>
internal sealed record StrongLinkInfo(House House, ref readonly CellMap Start, ref readonly CellMap End, HouseMask SpannedHouses);
