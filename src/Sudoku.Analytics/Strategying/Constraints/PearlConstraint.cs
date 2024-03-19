namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for pearl property.
/// </summary>
public sealed class PearlConstraint() : PearlOrDiamondConstraint(true);
