namespace SudokuStudio.Views.Pages.Analyze;

/// <summary>
/// Defines the gathering page.
/// </summary>
public sealed partial class GridGathering : Page, IAnalyzeTabPage
{
	/// <summary>
	/// Initializes a <see cref="GridGathering"/> instance.
	/// </summary>
	public GridGathering() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;

	/// <inheritdoc/>
	LogicalSolverResult? IAnalyzeTabPage.AnalysisResult
	{
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => throw new NotSupportedException();

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => throw new NotSupportedException();
	}


	private void TechniqueGroupView_StepChosen(object sender, TechniqueGroupViewStepChosenEventArgs e)
		=> BasePage.VisualUnit = e.ChosenStep;
}
