using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using Sudoku.Algorithm.Generating;
using Sudoku.Algorithm.Ittoryu;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using SudokuStudio.Interaction.Conversions;
using SudokuStudio.Storage;
using SudokuStudio.Strings;
using SudokuStudio.Views.Attached;
using Windows.Storage.Pickers;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the generating operation command bar.
/// </summary>
public sealed partial class GeneratingOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// The fields for core usages on counting puzzles.
	/// </summary>
	private int _generatingCount, _generatingFilteredCount;


	/// <summary>
	/// Initializes a <see cref="GeneratingOperation"/> instance.
	/// </summary>
	public GeneratingOperation() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		if (e.Parameter is AnalyzePage p)
		{
			SetConfiguredOptions(p);
		}
	}

	/// <summary>
	/// Update control selection via user's configuration.
	/// </summary>
	/// <param name="basePage">The base page.</param>
	private void SetConfiguredOptions(AnalyzePage basePage)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		var comma = GetString("_Token_Comma");
		var openBrace = GetString("_Token_OpenBrace");
		var closedBrace = GetString("_Token_ClosedBrace");
		TextBlockBindable.SetInlines(
			GeneratorStrategyTooltip,
			[
				new Run().WithText($"{null:AnalyzePage_GeneratingStrategySelected}"),
				new LineBreak(),
				new Run().WithText($"{(uiPref.CanRestrictGeneratingGivensCount, uiPref.GeneratedPuzzleGivensCount) switch
				{
					(false, _) or (_, -1) => (GetString("AnalyzePage_GeneratedPuzzleGivensNoRestriction"), string.Empty),
					_ => (uiPref.GeneratedPuzzleGivensCount, GetString("AnalyzePage_NumberOfGivens"))
				}:AnalyzePage_GeneratedPuzzleGivensIs}"),
				new LineBreak(),
				new Run().WithText($"{DifficultyLevelConversion.GetNameWithDefault(
					uiPref.GeneratorDifficultyLevel,
					GetString("DifficultyLevel_None")
				):AnalyzePage_SelectedDifficultyLevelIs}"),
				new LineBreak(),
				new Run().WithText($"{GetString($"SymmetricType_{uiPref.GeneratorSymmetricPattern}"):AnalyzePage_SelectedSymmetricTypeIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratorSelectedTechniques switch
				{
					[var f] => string.Format(GetString("AnalyzePage_SingleTechniquesSelected"), f.GetName()),
					[var f, ..] t and { Count: var fc } => string.Format(GetString("AnalyzePage_MultipleTechniquesSelected"), f.GetName(), fc),
					_ => GetString("TechniqueSelector_NoTechniqueSelected"),
				}:AnalyzePage_SelectedTechniqueIs}"),
				new LineBreak(),
				new Run().WithText($"{(
				uiPref.GeneratedPuzzleShouldBeMinimal
					? GetString("AnalyzePage_IsAMinimal")
					: GetString("AnalyzePage_IsNotMinimal")
				):AnalyzePage_SelectedMinimalRuleIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratedPuzzleShouldBePearl switch
				{
					true => GetString("GeneratingStrategyPage_PearlPuzzle"),
					false => GetString("GeneratingStrategyPage_NormalPuzzle"),
					//_ => GetString("GeneratingStrategyPage_DiamondPuzzle")
				}:AnalyzePage_SelectedDiamondRuleIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratorDifficultyLevel switch
				{
					DifficultyLevel.Easy => string.Format(GetString("AnalyzePage_IttoryuLength"), uiPref.IttoryuLength),
					_ => GetString("AnalyzePage_IttoryuPathIsNotLimited")
				}:AnalyzePage_SelectedIttoryuIs}")
			]
		);

		if (basePage._puzzleLibraries is { } libs)
		{
			switch (libs.Count)
			{
				case 0:
				{
					PuzzleLibraryChoser.Visibility = Visibility.Collapsed;
					FetchInPuzzleLibraryButton.Visibility = Visibility.Collapsed;
					break;
				}
				default:
				{
					PuzzleLibraryChoser.Visibility = Visibility.Visible;
					FetchInPuzzleLibraryButton.Visibility = Visibility.Visible;
					PuzzleLibraryChoser.ItemsSource = libs;
					var fileId = ((App)Application.Current).Preference.UIPreferences.FetchingPuzzleLibrary;
					var foundElementCorrespondingIndex = -1;
					for (var i = 0; i < libs.Count; i++)
					{
						if (libs[i] is { FileId: var fileIdToCheck } && fileIdToCheck == fileId)
						{
							foundElementCorrespondingIndex = i;
							break;
						}
					}
					PuzzleLibraryChoser.SelectedIndex = foundElementCorrespondingIndex == -1 ? 0 : foundElementCorrespondingIndex;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Handle generating operation.
	/// </summary>
	/// <typeparam name="T">The type of the progress data provider.</typeparam>
	/// <param name="onlyGenerateOne">Indicates whether the generator engine only generates for one puzzle.</param>
	/// <param name="gridStateChanger">
	/// The method that can change the state of the target grid. This callback method will be used for specify the grid state
	/// when a user has set the techniques that must be appeared.
	/// </param>
	/// <param name="gridHandler">The grid handler.</param>
	/// <returns>The task that holds the asynchronous operation.</returns>
	private async Task HandleGeneratingAsync<T>(
		bool onlyGenerateOne,
		GridStateChanger<(Analyzer Analyzer, TechniqueSet Techniques)>? gridStateChanger = null,
		ActionRefReadOnly<Grid>? gridHandler = null
	) where T : struct, IEquatable<T>, IProgressDataProvider<T>
	{
		BasePage.IsGeneratorLaunched = true;
		BasePage.ClearAnalyzeTabsData();

		var processingText = GetString("AnalyzePage_GeneratorIsProcessing");
		var preferences = ((App)Application.Current).Preference.UIPreferences;
		var difficultyLevel = preferences.GeneratorDifficultyLevel;
		var symmetry = preferences.GeneratorSymmetricPattern;
		var minimal = preferences.GeneratedPuzzleShouldBeMinimal;
		var pearl = preferences.GeneratedPuzzleShouldBePearl;
		var techniques = preferences.GeneratorSelectedTechniques;
		var givensCount = preferences switch
		{
			{ CanRestrictGeneratingGivensCount: true, GeneratedPuzzleGivensCount: var count and not -1 } => count,
			_ => HodokuPuzzleGenerator.AutoClues
		};
		var ittoryuLength = preferences.IttoryuLength;
		var analyzer = ((App)Application.Current).GetAnalyzerConfigured(BasePage.SudokuPane, preferences.GeneratorDifficultyLevel);
		var ittoryuFinder = new IttoryuPathFinder([.. ((App)Application.Current).Preference.AnalysisPreferences.IttoryuSupportedTechniques]);

		using var cts = new CancellationTokenSource();
		BasePage._ctsForAnalyzingRelatedOperations = cts;

		try
		{
			(_generatingCount, _generatingFilteredCount) = (0, 0);
			var details = new GeneratingDetails(difficultyLevel, symmetry, minimal, pearl, techniques, givensCount, ittoryuLength);
			if (onlyGenerateOne)
			{
				if (await Task.Run(task) is { IsUndefined: false } grid)
				{
					gridStateChanger?.Invoke(ref grid, (analyzer, techniques));
					gridHandler?.Invoke(ref grid);
				}
			}
			else
			{
				while (true)
				{
					if (await Task.Run(task) is { IsUndefined: false } grid)
					{
						gridStateChanger?.Invoke(ref grid, (analyzer, techniques));
						gridHandler?.Invoke(ref grid);

						_generatingFilteredCount++;
						continue;
					}

					break;
				}
			}


			Grid task() => gridCreator(analyzer, ittoryuFinder, details);
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			BasePage._ctsForAnalyzingRelatedOperations = null;
			BasePage.IsGeneratorLaunched = false;
		}


		Grid gridCreator(Analyzer analyzer, IttoryuPathFinder finder, GeneratingDetails details)
		{
			try
			{
				return generatePuzzleCore(
					progress => DispatcherQueue.TryEnqueue(
						() =>
						{
							BasePage.AnalyzeProgressLabel.Text = processingText;
							BasePage.AnalyzeStepSearcherNameLabel.Text = progress.ToDisplayString();
						}
					), details, cts.Token, analyzer, finder
				) ?? Grid.Undefined;
			}
			catch (TaskCanceledException)
			{
				return Grid.Undefined;
			}
		}

		Grid? generatePuzzleCore(
			Action<T> reportingAction,
			GeneratingDetails details,
			CancellationToken cancellationToken,
			Analyzer analyzer,
			IttoryuPathFinder finder
		)
		{
			try
			{
				for (
					var (progress, (difficultyLevel, symmetry, minimal, pearl, techniques, givensCount, ittoryuLength)) = (
						new SelfReportingProgress<T>(reportingAction),
						details
					); ; _generatingCount++, progress.Report(T.Create(_generatingCount, _generatingFilteredCount)), cancellationToken.ThrowIfCancellationRequested()
				)
				{
					var grid = HodokuPuzzleGenerator.Generate(givensCount, symmetry, cancellationToken);

					switch (difficultyLevel)
					{
						case DifficultyLevel.Easy:
						{
							// Optimize: transform the grid if worth.
							var foundIttoryu = finder.FindPath(in grid);
							if (ittoryuLength >= 5 && foundIttoryu.Digits.Length >= 5)
							{
								grid.MakeIttoryu(foundIttoryu);
							}

							if (basicCondition() && (ittoryuLength != -1 && foundIttoryu.Digits.Length >= ittoryuLength || ittoryuLength == -1))
							{
								return grid;
							}
							break;
						}
						case var _ when basicCondition():
						{
							return grid;
						}
					}


					bool basicCondition()
						=> (givensCount != -1 && grid.GivensCount == givensCount || givensCount == -1)
						&& analyzer.Analyze(in grid) is
						{
							IsSolved: true,
							IsPearl: var isPearl,
							DifficultyLevel: var puzzleDifficultyLevel,
							SolvingPath: var p
						}
						&& (difficultyLevel == 0 || puzzleDifficultyLevel == difficultyLevel)
						&& (minimal && grid.IsMinimal || !minimal)
						&& (pearl && isPearl is true || !pearl)
						&& (!!(techniques && p & techniques) || !techniques);
				}
			}
			catch (OperationCanceledException)
			{
				return null;
			}
		}
	}


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
		=> await HandleGeneratingAsync<GeneratorProgress>(
			true,
			gridHandler: (scoped ref readonly Grid grid) =>
			{
				if (((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
				{
					((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = grid });
				}

				BasePage.SudokuPane.Puzzle = grid;
			}
		);

	private void FetchInPuzzleLibraryButton_Click(object sender, RoutedEventArgs e)
	{
		var source = (PuzzleLibraryBindableSource)PuzzleLibraryChoser.SelectedValue;
		if (source.Puzzles.Length == 0)
		{
			// No puzzles in this library.
			return;
		}

		var grid = source.Puzzles[Random.Shared.Next(0, source.Puzzles.Length)];
		transformGrid(ref grid, ((App)Application.Current).Preference.LibraryPreferences.LibraryPuzzleTransformations);

		BasePage.SudokuPane.Puzzle = grid;
		BasePage.ClearAnalyzeTabsData();


		static unsafe void transformGrid(scoped ref Grid grid, LibraryPuzzleTransformKinds transformKinds)
		{
			if (transformKinds == LibraryPuzzleTransformKinds.None)
			{
				return;
			}

			var random = new Random();
			foreach (var kind in transformKinds)
			{
				(
					kind switch
					{
						LibraryPuzzleTransformKinds.Digit => &swapDigits,
						LibraryPuzzleTransformKinds.RowSwap => &swapRow,
						LibraryPuzzleTransformKinds.ColumnSwap => &swapColumn,
						LibraryPuzzleTransformKinds.BandSwap => &swapBand,
						LibraryPuzzleTransformKinds.TowerSwap => &swapTower,
						LibraryPuzzleTransformKinds.MirrowLeftRight => &mirrorLeftRight,
						LibraryPuzzleTransformKinds.MirrorTopBottom => &mirrorTopBottom,
						LibraryPuzzleTransformKinds.MirrorDiagonal => &mirrorDiagonal,
						LibraryPuzzleTransformKinds.MirrorAntidigaonal => &mirrorAntidiagonal,
						LibraryPuzzleTransformKinds.RotateClockwise => &rotateClockwise,
						LibraryPuzzleTransformKinds.RotateCounterclockwise => &rotateCounterclockwise,
						_ => default(delegate*<Random, ref Grid, void>)
					}
				)(random, ref grid);
			}


			static void swapDigits(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 10; i++)
				{
					var d1 = random.Next(0, 9);
					int d2;
					do
					{
						d2 = random.Next(0, 9);
					} while (d1 == d2);

					grid.SwapTwoDigits(d1, d2);
				}
			}

			static void swapRow(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 10; i++)
				{
					var l1 = random.Next(9, 18);
					var l1p = l1 / 3;
					int l2;
					do
					{
						l2 = l1p * 3 + random.Next(0, 3);
					} while (l1 == l2);

					grid.SwapTwoHouses(l1, l2);
				}
			}

			static void swapColumn(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 10; i++)
				{
					var l1 = random.Next(18, 27);
					var l1p = l1 / 3;
					int l2;
					do
					{
						l2 = l1p * 3 + random.Next(0, 3);
					} while (l1 == l2);

					grid.SwapTwoHouses(l1, l2);
				}
			}

			static void swapBand(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 2; i++)
				{
					var c1 = random.Next(0, 3);
					int c2;
					do
					{
						c2 = random.Next(0, 3);
					} while (c1 == c2);

					grid.SwapChute(c1, c2);
				}
			}

			static void swapTower(Random random, scoped ref Grid grid)
			{
				for (var i = 0; i < 2; i++)
				{
					var c1 = random.Next(3, 6);
					int c2;
					do
					{
						c2 = random.Next(3, 6);
					} while (c1 == c2);

					grid.SwapChute(c1, c2);
				}
			}

			static void mirrorLeftRight(Random _, scoped ref Grid grid) => grid.MirrorLeftRight();

			static void mirrorTopBottom(Random _, scoped ref Grid grid) => grid.MirrorTopBottom();

			static void mirrorDiagonal(Random _, scoped ref Grid grid) => grid.MirrorDiagonal();

			static void mirrorAntidiagonal(Random _, scoped ref Grid grid) => grid.MirrorAntidiagonal();

			static void rotateClockwise(Random random, scoped ref Grid grid)
			{
				var times = random.Next(0, 4);
				for (var i = 0; i < times; i++)
				{
					grid.RotateClockwise();
				}
			}

			static void rotateCounterclockwise(Random random, scoped ref Grid grid)
			{
				var times = random.Next(0, 4);
				for (var i = 0; i < times; i++)
				{
					grid.RotateCounterclockwise();
				}
			}
		}
	}

	private void PuzzleLibraryChoser_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var source = (PuzzleLibraryBindableSource)PuzzleLibraryChoser.SelectedValue;
		((App)Application.Current).Preference.UIPreferences.FetchingPuzzleLibrary = source.FileId;
	}

	private async void BatchGeneratingButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (!BasePage.EnsureUnsnapped(true))
		{
			return;
		}

		var fsp = new FileSavePicker();
		fsp.Initialize(this);
		fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fsp.SuggestedFileName = GetString("Sudoku");
		fsp.AddFileFormat(FileFormats.PlainText);

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath })
		{
			return;
		}

		await HandleGeneratingAsync<FilteredGeneratorProgress>(
			false,
			static (scoped ref Grid grid, (Analyzer Analyzer, TechniqueSet Techniques) pair) =>
			{
				var (analyzer, techniques) = pair;
				var analyzerResult = analyzer.Analyze(in grid);
				if (!analyzerResult.IsSolved)
				{
					return;
				}

				foreach (var (g, s) in analyzerResult.SolvingPath)
				{
					if (techniques.Contains(s.Code))
					{
						grid = g;
						break;
					}
				}
			},
			(scoped ref readonly Grid grid) =>
			{
				File.AppendAllText(filePath, $"{grid:#}{Environment.NewLine}");

				if (((App)Application.Current).Preference.UIPreferences.AlsoSaveBatchGeneratedPuzzlesIntoHistory
					&& ((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
				{
					((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = grid });
				}
			}
		);
	}
}

/// <summary>
/// Defines a self-reporting progress type.
/// </summary>
/// <param name="handler"><inheritdoc cref="Progress{T}.Progress(Action{T})" path="/param[@name='handler']"/></param>
/// <typeparam name="T"><inheritdoc cref="Progress{T}" path="/typeparam[@name='T']"/></typeparam>
file sealed class SelfReportingProgress<T>(Action<T> handler) : Progress<T>(handler) where T : struct, IEquatable<T>, IProgressDataProvider<T>
{
	/// <inheritdoc cref="Progress{T}.OnReport(T)"/>
	public void Report(T value) => OnReport(value);
}

/// <summary>
/// The encapsulated type to describe the details for generating puzzles.
/// </summary>
/// <param name="DifficultyLevel">Indicates the difficulty level selected.</param>
/// <param name="SymmetricPattern">Indicates the symmetric pattern selected.</param>
/// <param name="ShouldBeMinimal">Indicates whether generated puzzles should be minimal.</param>
/// <param name="ShouldBePearl">Indicates whether generated puzzles should be pearl.</param>
/// <param name="SelectedTechniques">Indicates the selected technique that you want it to be appeared in generated puzzles.</param>
/// <param name="CountOfGivens">Indicates the limit of givens count.</param>
/// <param name="IttoryuLength">Indicates the ittoryu length.</param>
file sealed record GeneratingDetails(
	DifficultyLevel DifficultyLevel,
	SymmetricType SymmetricPattern,
	bool ShouldBeMinimal,
	bool ShouldBePearl,
	TechniqueSet SelectedTechniques,
	int CountOfGivens,
	int IttoryuLength
);

/// <summary>
/// Provides with extension methods on <see cref="Run"/>.
/// </summary>
/// <seealso cref="Run"/>
file static class RunExtensions
{
	/// <summary>
	/// Sets with <see cref="Run.Text"/> property, reducing indenting.
	/// </summary>
	/// <param name="this">The <see cref="Run"/> instance.</param>
	/// <param name="text">The text to be initialized.</param>
	public static Run WithText(this Run @this, scoped ref ResourceFetcher text)
	{
		@this.Text = text.ToString();
		return @this;
	}
}
