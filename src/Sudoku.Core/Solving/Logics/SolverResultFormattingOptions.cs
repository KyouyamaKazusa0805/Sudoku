namespace Sudoku.Solving.Logics;

/// <summary>
/// Indicates the formatting options of <see cref="LogicalSolverResult"/> instance.
/// </summary>
/// <seealso cref="LogicalSolverResult"/>
[Flags]
public enum SolverResultFormattingOptions : short
{
	/// <summary>
	/// Indicates the none of the formatting option.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the analysis result will append separators.
	/// </summary>
	ShowSeparators = 1,

	/// <summary>
	/// Indicates the analysis result will append the step label.
	/// </summary>
	ShowStepLabel = 1 << 1,

	/// <summary>
	/// Indicates the analysis result will use simple mode to show steps.
	/// </summary>
	ShowSimple = 1 << 2,

	/// <summary>
	/// Indicates the analysis result will show the bottleneck.
	/// </summary>
	ShowBottleneck = 1 << 3,

	/// <summary>
	/// Indicates the analysis result will show the difficulty.
	/// </summary>
	ShowDifficulty = 1 << 4,

	/// <summary>
	/// Indicates the analysis result will show all steps after the bottleneck.
	/// </summary>
	ShowStepsAfterBottleneck = 1 << 5,

	/// <summary>
	/// Indicates the analysis result will show the step detail.
	/// </summary>
	ShowStepDetail = 1 << 6,

	/// <summary>
	/// Indicates the analysis result will show the steps.
	/// </summary>
	ShowSteps = 1 << 7
}
