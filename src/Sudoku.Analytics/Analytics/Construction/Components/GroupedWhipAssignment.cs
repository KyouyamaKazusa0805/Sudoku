namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a type that is used by whip chaining, grouped nodes.
/// </summary>
/// <param name="Digit">Indicates the target digit.</param>
/// <param name="Cells">Indicates the target cells.</param>
/// <param name="Reason"><inheritdoc cref="WhipAssignment.Reason" path="/summary"/></param>
public sealed record GroupedWhipAssignment(Digit Digit, ref readonly CellMap Cells, Technique Reason) :
	WhipAssignment(Cells * Digit, Reason);
