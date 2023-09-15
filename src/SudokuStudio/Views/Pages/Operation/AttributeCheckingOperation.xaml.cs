using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Algorithm.Backdoors;
using Sudoku.Algorithm.Ittoryu;
using Sudoku.Algorithm.TrueCandidates;
using Sudoku.Analytics;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using SudokuStudio.ComponentModel;
using WinRT;
using static Sudoku.Analytics.ConclusionType;
using static SudokuStudio.ProjectWideConstants;
using static SudokuStudio.Strings.StringsAccessor;

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
		if (!puzzle.IsValid)
		{
			ErrorDialog_PuzzleIsInvalid.Target = BackdoorButton;
			ErrorDialog_PuzzleIsInvalid.IsOpen = true;
			return;
		}

		var backdoors = await Task.Run(() =>
		{
			lock (AnalyzingRelatedSyncRoot)
			{
				return BackdoorSearcher.GetBackdoors(in puzzle);
			}
		});

		BasePage.VisualUnit = new BackdoorVisualUnit([
			..
			from backdoor in backdoors
			let type = backdoor.ConclusionType
			let candidate = backdoor.Candidate
			select new CandidateViewNode(type == Assignment ? WellKnownColorIdentifierKind.Assignment : WellKnownColorIdentifierKind.Elimination, candidate)
		]);
	}

	private async void TrueCandidateButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		if (!puzzle.IsValid)
		{
			ErrorDialog_PuzzleIsInvalid.Target = TrueCandidateButton;
			ErrorDialog_PuzzleIsInvalid.IsOpen = true;
			return;
		}

		var trueCandidates = await Task.Run(() =>
		{
			lock (AnalyzingRelatedSyncRoot)
			{
				return TrueCandidatesSearcher.GetAllTrueCandidates(in puzzle);
			}
		});
		if (trueCandidates.Count == 0)
		{
			ErrorDialog_PuzzleIsNotBugMultipleGrid.IsOpen = true;
			return;
		}

		BasePage.VisualUnit = new TrueCandidateVisualUnit([
			..
			from candidate in trueCandidates
			select new CandidateViewNode(WellKnownColorIdentifierKind.Assignment, candidate)
		]);
	}

	private async void DisorderedIttoryuButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		if (!puzzle.IsValid)
		{
			ErrorDialog_PuzzleIsInvalid.Target = TrueCandidateButton;
			ErrorDialog_PuzzleIsInvalid.IsOpen = true;
			return;
		}

		var ittoryuFinder = new IttoryuPathFinder();
		var digitPath = await Task.Run(() => ittoryuFinder.FindPath(in puzzle));
		if (!digitPath.IsComplete)
		{
			InfoDialog_DisorderedIttoryuDigitSequence.Subtitle = GetString("AnalyzePage_DisorderedIttoryuDoesNotExist");
			InfoDialog_DisorderedIttoryuDigitSequence.IsOpen = true;
			return;
		}

		InfoDialog_DisorderedIttoryuDigitSequence.Subtitle = string.Format(GetString("AnalyzePage_DisorderedIttoryuOrderIs"), digitPath.ToString());
		InfoDialog_DisorderedIttoryuDigitSequence.IsOpen = true;
	}
}

/// <summary>
/// Defines a backdoor visual unit.
/// </summary>
/// <param name="view">The view.</param>
file sealed class BackdoorVisualUnit(View view) : IRenderable
{
	/// <inheritdoc/>
	public View[]? Views { get; } = [view];

	/// <inheritdoc/>
	Conclusion[] IRenderable.Conclusions { get; } = [];
}

/// <summary>
/// Defines a true-candidate visual unit.
/// </summary>
/// <param name="view">The view.</param>
file sealed class TrueCandidateVisualUnit(View view) : IRenderable
{
	/// <inheritdoc/>
	public View[]? Views { get; } = [view];

	/// <inheritdoc/>
	Conclusion[] IRenderable.Conclusions { get; } = [];
}
