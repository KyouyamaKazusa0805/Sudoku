namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides with the <see cref="ISyntaxContextReceiver"/> with the diagnotic results.
/// </summary>
internal interface ISyntaxContextReceiverWithResult : ISyntaxContextReceiver
{
	/// <summary>
	/// Indicates the diagnostic result found and gathered.
	/// </summary>
	List<Diagnostic> Diagnostics { get; }
}
