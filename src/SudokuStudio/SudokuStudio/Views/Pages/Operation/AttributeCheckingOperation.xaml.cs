namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Attribute checking operation page.
/// </summary>
public sealed partial class AttributeCheckingOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// Initializes an <see cref="AttributeCheckingOperation"/> instance.
	/// </summary>
	public AttributeCheckingOperation() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	private async void BackdoorButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		if (!puzzle.IsValid())
		{
			ErrorDialog_PuzzleIsInvalid.Target = BackdoorButton;
			ErrorDialog_PuzzleIsInvalid.IsOpen = true;

			return;
		}

		var backdoors = await Task.Run(getBackdoors);
		var view = View.Empty;
		foreach (var (type, candidate) in backdoors)
		{
			view.Add(new CandidateViewNode(type == Assignment ? DisplayColorKind.Assignment : DisplayColorKind.Elimination, candidate));
		}

		var visualUnit = new BackdoorVisualUnit(view);
		BasePage.VisualUnit = visualUnit;


		Conclusion[] getBackdoors()
		{
			lock (App.SyncRoot)
			{
				return BackdoorSearcher.GetBackdoors(puzzle);
			}
		}
	}

	private async void TrueCandidateButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		if (!puzzle.IsValid())
		{
			ErrorDialog_PuzzleIsInvalid.Target = TrueCandidateButton;
			ErrorDialog_PuzzleIsInvalid.IsOpen = true;

			return;
		}

		var trueCandidates = await Task.Run(getTrueCandidates);
		if (trueCandidates.Count == 0)
		{
			ErrorDialog_PuzzleIsNotBugMultipleGrid.IsOpen = true;

			return;
		}

		var view = View.Empty;
		trueCandidates.ForEach(candidate => view.Add(new CandidateViewNode(DisplayColorKind.Assignment, candidate)));

		var visualUnit = new TrueCandidateVisualUnit(view);
		BasePage.VisualUnit = visualUnit;


		CandidateMap getTrueCandidates()
		{
			lock (App.SyncRoot)
			{
				return TrueCandidatesSearcher.GetAllTrueCandidates(puzzle);
			}
		}
	}
}

/// <summary>
/// Defines a backdoor visual unit.
/// </summary>
file sealed class BackdoorVisualUnit : VisualUnit
{
	/// <summary>
	/// Initializes a <see cref="BackdoorVisualUnit"/> instance via the specified view.
	/// </summary>
	/// <param name="view">The view.</param>
	public BackdoorVisualUnit(View view) => Views = new[] { view };


	/// <inheritdoc/>
	public View[]? Views { get; }

	/// <inheritdoc/>
	Conclusion[] VisualUnit.Conclusions { get; } = Array.Empty<Conclusion>();
}

/// <summary>
/// Defines a true-candidate visual unit.
/// </summary>
file sealed class TrueCandidateVisualUnit : VisualUnit
{
	/// <summary>
	/// Initializes a <see cref="TrueCandidateVisualUnit"/> instance via the specified view.
	/// </summary>
	/// <param name="view">The view.</param>
	public TrueCandidateVisualUnit(View view) => Views = new[] { view };


	/// <inheritdoc/>
	public View[]? Views { get; }

	/// <inheritdoc/>
	Conclusion[] VisualUnit.Conclusions { get; } = Array.Empty<Conclusion>();
}
