namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents context used by <see cref="Constraint"/> instance.
/// </summary>
/// <param name="grid">Indicates the reference to the grid to be checked.</param>
/// <param name="analyzerResult">Indicates the analyzer result.</param>
/// <param name="collectorResult">Indicates the collector result.</param>
/// <param name="cancellationToken">Indicates the cancellation token that can cancel the current operation.</param>
/// <seealso cref="Constraint"/>
public readonly ref partial struct ConstraintCheckingContext(
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", GeneratedMemberName = "Grid")] ref readonly Grid grid,
	[PrimaryConstructorParameter] AnalyzerResult? analyzerResult,
	[PrimaryConstructorParameter] Step[][]? collectorResult,
	[PrimaryConstructorParameter] CancellationToken cancellationToken = default
)
{
	/// <summary>
	/// Indicates whether a screening rule requires property <see cref="AnalyzerResult"/>.
	/// </summary>
	[MemberNotNullWhen(true, nameof(AnalyzerResult))]
	public bool RequiresAnalyzer => AnalyzerResult is not null;

	/// <summary>
	/// Indicates whether a screening rule requires property <see cref="CollectorResult"/>.
	/// </summary>
	[MemberNotNullWhen(true, nameof(CollectorResult))]
	public bool RequiresCollector => CollectorResult is not null;
}
