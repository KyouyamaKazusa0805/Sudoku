namespace Sudoku.Generating.Filtering;

/// <summary>
/// Represents context that will be called by method <see cref="Constraint.Check(ConstraintCheckingContext)"/>.
/// </summary>
/// <param name="grid">Indicates the reference to the grid to be checked.</param>
/// <param name="analysisResult">Indicates the analysis result.</param>
/// <seealso cref="Constraint.Check(ConstraintCheckingContext)"/>
public readonly ref partial struct ConstraintCheckingContext(
	[Field(Accessibility = "public", NamingRule = NamingRules.Property)] in Grid grid,
	[Property] AnalysisResult analysisResult
);
