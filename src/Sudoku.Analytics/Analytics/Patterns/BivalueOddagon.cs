namespace Sudoku.Analytics.Patterns;

/// <summary>
/// Represents for a data set that describes the complete information about a bi-value oddagon technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole bi-value oddagon loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="Mask"/>.</param>
public readonly record struct BivalueOddagon(scoped in CellMap Loop, Mask DigitsMask);
