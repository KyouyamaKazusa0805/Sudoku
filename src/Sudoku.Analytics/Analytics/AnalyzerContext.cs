namespace Sudoku.Analytics;

/// <summary>
/// Represents a context that is created by <see cref="Analyzer"/> to analyze a puzzle.
/// </summary>
/// <param name="puzzle">The puzzle to be analyzed.</param>
/// <seealso cref="Analyzer"/>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
public readonly ref partial struct AnalyzerContext([
	PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ref readonly Grid puzzle
) : IContext
{
	/// <summary>
	/// The cancellation token that can cancel the current analyzing operation.
	/// </summary>
	public CancellationToken CancellationToken { get; init; }

	/// <summary>
	/// An <see cref="IProgress{T}"/> instance that is used for reporting the state.
	/// </summary>
	public IProgress<AnalysisProgress>? ProgressReporter { get; init; }

	/// <inheritdoc/>
	readonly ref readonly Grid IContext.Grid => ref Puzzle;
}
