namespace Sudoku.Solving.Data.Representation;

/// <summary>
/// Represents for a data set that describes the complete information about a guardian technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole guardian loop.</param>
/// <param name="Guardians">Indicates the extra cells that is used as guardians.</param>
/// <param name="Digit">Indicates the digit used.</param>
public readonly record struct GuardianData(scoped in Cells Loop, scoped in Cells Guardians, int Digit);
