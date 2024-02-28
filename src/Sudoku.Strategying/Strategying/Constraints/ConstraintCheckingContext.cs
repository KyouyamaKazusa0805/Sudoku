namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents context used by <see cref="Constraint"/> instance.
/// </summary>
/// <param name="grid">Indicates the reference to the grid to be checked.</param>
/// <param name="analyzerResult">Indicates the analyzer result.</param>
/// <seealso cref="Constraint"/>
public readonly ref partial struct ConstraintCheckingContext(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", GeneratedMemberName = "Grid")] ref readonly Grid grid,
	[PrimaryConstructorParameter] AnalyzerResult? analyzerResult
)
{
	/// <summary>
	/// Indicates whether a screening rule requires property <see cref="AnalyzerResult"/>.
	/// </summary>
	[MemberNotNullWhen(true, nameof(AnalyzerResult))]
	public bool RequiresAnalyzer => AnalyzerResult is not null;
}
