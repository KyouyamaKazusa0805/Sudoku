namespace Sudoku.Presentation;

/// <summary>
/// Encapsulates an unknown value that used in the technique <b>Unknown Covering</b>.
/// </summary>
/// <param name="Cell">Indicates the cell that used and marked.</param>
/// <param name="UnknownIdentifier">Indicates the identifier that identifies the value range.</param>
/// <param name="DigitsMask">Indicates a mask that holds a serial of candidate values.</param>
public readonly partial record struct UnknownValue(int Cell, char UnknownIdentifier, short DigitsMask);