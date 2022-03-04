using Sudoku.Solving.Manual;

namespace Sudoku.UI.Data;

/// <summary>
/// Defines a <see cref="InfoBarMessage"/> instance that can provide with the details
/// for a <see cref="ManualSolverResult"/> instance.
/// </summary>
/// <seealso cref="ManualSolverResult"/>
public sealed class ManualSolverResultMessage : InfoBarMessage
{
	/// <summary>
	/// Indicates the analysis result.
	/// </summary>
	public ManualSolverResult AnalysisResult { get; set; } = null!;
}
