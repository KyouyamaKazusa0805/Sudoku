namespace Sudoku.Solving.Dlx;

/// <summary>
/// Represents an assertion that fixes the result for the target cell with target digits.
/// </summary>
/// <param name="TargetCell">Indicates the target cell.</param>
/// <param name="ReservedDigits">Indicates the reserved digits.</param>
public readonly record struct CellAssertion(Cell TargetCell, Mask ReservedDigits);
