namespace Sudoku.Concepts;

/// <summary>
/// Represents data describing for an H-Wing pattern.
/// </summary>
/// <param name="CommonDigit">The common digit for those two digits.</param>
/// <param name="OtherDigitsMask">Indicates the other digits used.</param>
/// <param name="Cells">Indicates the two cells.</param>
internal sealed record HWingAlmostLockedSetLinkInfo(Digit CommonDigit, Mask OtherDigitsMask, scoped ref readonly CellMap Cells);
