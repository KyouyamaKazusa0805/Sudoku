namespace Sudoku.Generating.Filtering;

/// <summary>
/// Represents context that will be called by method <see cref="Constraint.Check(ConstraintCheckingContext)"/>.
/// </summary>
/// <param name="grid">Indicates the reference to the grid to be checked.</param>
/// <param name="analyzerResult">Indicates the analyzer result.</param>
/// <seealso cref="Constraint.Check(ConstraintCheckingContext)"/>
public readonly ref partial struct ConstraintCheckingContext(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ref readonly Grid grid,
	[PrimaryConstructorParameter] AnalysisResult analyzerResult
);
