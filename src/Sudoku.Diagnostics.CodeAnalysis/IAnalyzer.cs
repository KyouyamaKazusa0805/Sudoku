namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides a constraint set that defines an analyzer.
/// </summary>
internal interface IAnalyzer : ISyntaxContextReceiver
{
	/// <summary>
	/// The cancellation token to cancel the current operation.
	/// </summary>
	CancellationToken CancellationToken { get; }

	/// <summary>
	/// Indicates the possible diagnostic list to report.
	/// </summary>
	IList<Diagnostic> DiagnosticList { get; }
}
