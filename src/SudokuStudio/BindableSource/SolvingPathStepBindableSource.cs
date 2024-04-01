namespace SudokuStudio.BindableSource;

/// <summary>
/// Defines a path step in a whole solving path.
/// </summary>
public sealed class SolvingPathStepBindableSource
{
	/// <summary>
	/// Indicates whether displaying HoDoKu difficulty.
	/// </summary>
	public bool ShowHodokuDifficulty { get; set; }

	/// <summary>
	/// Indicates whether displaying Sudoku Explainer difficulty.
	/// </summary>
	public bool ShowSudokuExplainerDifficulty { get; set; }

	/// <summary>
	/// Indicates the index of the step.
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// Indicates the step grid used.
	/// </summary>
	public Grid InterimGrid { get; set; }

	/// <summary>
	/// Indicates the kinds of items which the step tooltip will be displayed.
	/// </summary>
	public required StepTooltipDisplayItems DisplayItems { get; set; }

	/// <summary>
	/// Indicates the step details.
	/// </summary>
	public required Step InterimStep { get; set; }
}
