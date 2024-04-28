namespace Sudoku.Strategying;

/// <summary>
/// Represents context that will be called by method <see cref="Constraint.Check(ConstraintCheckingContext)"/>.
/// </summary>
/// <param name="Grid">Indicates the reference to the grid to be checked.</param>
/// <param name="AnalyzerResult">Indicates the analyzer result.</param>
/// <seealso cref="Constraint.Check(ConstraintCheckingContext)"/>
public readonly record struct ConstraintCheckingContext(ref readonly Grid Grid, AnalysisResult AnalyzerResult);
