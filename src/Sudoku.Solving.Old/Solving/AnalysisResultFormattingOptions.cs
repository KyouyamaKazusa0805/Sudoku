namespace Sudoku.Solving;

/// <summary>
/// Indicates the formatting options of <see cref="AnalysisResult"/> instance.
/// </summary>
[Flags]
public enum AnalysisResultFormattingOptions : short
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
	ShowStepLabel = 2,

	/// <summary>
	/// Indicates the analysis result will use simple mode to show steps.
	/// </summary>
	ShowSimple = 4,

	/// <summary>
	/// Indicates the analysis result will show the bottleneck.
	/// </summary>
	ShowBottleneck = 8,

	/// <summary>
	/// Indicates the analysis result will show the difficulty.
	/// </summary>
	ShowDifficulty = 16,

	/// <summary>
	/// Indicates the analysis result will show all steps after the bottleneck.
	/// </summary>
	ShowStepsAfterBottleneck = 32,

	/// <summary>
	/// Indicates the analysis result will append the attributes of the grid.
	/// </summary>
	ShowAttributes = 64,

	/// <summary>
	/// Indicates the analysis result will append the backdoors of the grid.
	/// </summary>
	ShowBackdoors = 128,

	/// <summary>
	/// Indicates the analysis result will show the step detail.
	/// </summary>
	ShowStepDetail = 256,

	/// <summary>
	/// Indicates the analysis result will show the steps.
	/// </summary>
	ShowSteps = 512
}
