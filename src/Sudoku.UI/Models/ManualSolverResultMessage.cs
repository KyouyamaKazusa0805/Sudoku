namespace Sudoku.UI.Models;

/// <summary>
/// Defines a <see cref="InfoBarMessage"/> instance that can provide with the details
/// for a <see cref="LogicalSolverResult"/> instance.
/// </summary>
/// <seealso cref="LogicalSolverResult"/>
public sealed class ManualSolverResultMessage : InfoBarMessage
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	public required LogicalSolverResult AnalysisResult { get; set; }
}
