namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the generating operation command bar.
/// </summary>
public sealed partial class GeneratingOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// Initializes a <see cref="GeneratingOperation"/> instance.
	/// </summary>
	public GeneratingOperation()
	{
		InitializeComponent();
		SetMemoryOptions();
	}


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <summary>
	/// Update control selection via user configuration.
	/// </summary>
	private void SetMemoryOptions()
	{
		//
		// DifficultyLevelSelector
		//
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		var flag = false;
		for (var (i, items, target) = (0, DifficultyLevelSelector.Items, uiPref.GeneratorDifficultyLevel);
			i < items.Count;
			i++)
		{
			if (items[i] is ComboBoxItem { Tag: int rawValue } && (DifficultyLevel)rawValue == target)
			{
				(DifficultyLevelSelector.SelectedIndex, flag) = (i, true);
				break;
			}
		}
		if (!flag)
		{
			DifficultyLevelSelector.SelectedIndex = 0;
		}

		//
		// PuzzleSymmetricPatternSelector
		//
		for ((var i, var items, var target, flag) = (0, PuzzleSymmetricPatternSelector.Items, uiPref.GeneratorSymmetricPattern, false);
			i < items.Count;
			i++)
		{
			if (items[i] is ComboBoxItem { Tag: int rawValue } && (SymmetricType)rawValue == target)
			{
				(PuzzleSymmetricPatternSelector.SelectedIndex, flag) = (i, true);
				break;
			}
		}
		if (!flag)
		{
			PuzzleSymmetricPatternSelector.SelectedIndex = 0;
		}

		//
		// GenerateForMinimalPuzzleToggleSwitch
		//
		GenerateForMinimalPuzzleToggleSwitch.IsOn = uiPref.GeneratedPuzzleShouldBeMinimal;

		//
		// PuzzleTechniqueSelector
		//
		PuzzleTechniqueSelector.SelectedIndex = Array.FindIndex(PuzzleTechniqueSelector.ItemsSource, e => e.Technique == uiPref.SelectedTechnique);
	}


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.IsGeneratorLaunched = true;
		BasePage.ClearAnalyzeTabsData();

		var processingText = GetString("AnalyzePage_GeneratorIsProcessing")!;
		var preferences = ((App)Application.Current).Preference.UIPreferences;
		var difficultyLevel = preferences.GeneratorDifficultyLevel;
		var symmetry = preferences.GeneratorSymmetricPattern;
		var minimal = preferences.GeneratedPuzzleShouldBeMinimal;
		var pearl = preferences.GeneratedPuzzleShouldBePearl;
		var technique = preferences.SelectedTechnique;
		var analyzer = ((App)Application.Current).Analyzer
			.WithStepSearchers(((App)Application.Current).GetStepSearchers(), difficultyLevel)
			.WithRuntimeIdentifierSetters(BasePage.SudokuPane);
		using var cts = new CancellationTokenSource();
		BasePage._ctsForAnalyzingRelatedOperations = cts;

		try
		{
			var grid = await Task.Run(() => gridCreator(analyzer, new(difficultyLevel, symmetry, minimal, pearl, technique)));
			if (grid.IsUndefined)
			{
				return;
			}

			if (((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
			{
				((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = grid });
			}

			BasePage.SudokuPane.Puzzle = grid;
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			BasePage._ctsForAnalyzingRelatedOperations = null;
			BasePage.IsGeneratorLaunched = false;
		}


		Grid gridCreator(Analyzer analyzer, GeneratingDetails details)
		{
			try
			{
				return generatePuzzleCore(progress => DispatcherQueue.TryEnqueue(() =>
				{
					var count = progress.Count;
					BasePage.AnalyzeProgressLabel.Text = processingText;
					BasePage.AnalyzeStepSearcherNameLabel.Text = count.ToString();
				}), details, cts.Token, analyzer) ?? Grid.Undefined;
			}
			catch (TaskCanceledException)
			{
				return Grid.Undefined;
			}
		}

		static Grid? generatePuzzleCore(
			Action<GeneratorProgress> reportingAction,
			GeneratingDetails details,
			CancellationToken cancellationToken,
			Analyzer analyzer
		)
		{
			try
			{
				for (
					var (count, progress, (difficultyLevel, symmetry, minimal, pearl, technique)) = (
						0,
						new SelfReportingProgress<GeneratorProgress>(reportingAction),
						details
					); ; count++, progress.Report(new(count)), cancellationToken.ThrowIfCancellationRequested()
				)
				{
					if (HodokuPuzzleGenerator.Generate(symmetry, cancellationToken) is var grid
						&& analyzer.Analyze(grid) is { IsSolved: true, IsPearl: var isPearl, DifficultyLevel: var puzzleDifficultyLevel } analyzerResult
						&& (difficultyLevel == 0 || puzzleDifficultyLevel == difficultyLevel)
						&& (minimal && grid.IsMinimal || !minimal)
						&& (pearl && isPearl is true || !pearl)
						&& (technique != 0 && analyzerResult.HasTechnique(technique) || technique == 0))
					{
						return grid;
					}
				}
			}
			catch (OperationCanceledException)
			{
				return null;
			}
		}
	}

	private void DifficultyLevelSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: int value } })
		{
			((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel = (DifficultyLevel)value;
		}
	}

	private void PuzzleSymmetricPatternSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ComboBox { SelectedItem: ComboBoxItem { Tag: int value } })
		{
			((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern = (SymmetricType)value;
		}
	}

	private void GenerateForMinimalPuzzleToggleSwitch_Toggled(object sender, RoutedEventArgs e)
	{
		if (sender is ToggleSwitch { IsOn: var value })
		{
			((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBeMinimal = value;
		}
	}

	private void GenerateForPearlPuzzleToggleSwitch_Toggled(object sender, RoutedEventArgs e)
	{
		if (sender is ToggleSwitch { IsOn: var value })
		{
			((App)Application.Current).Preference.UIPreferences.GeneratedPuzzleShouldBePearl = value;
		}
	}

	private void PuzzleTechniqueSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is TechniqueSelector { ItemsSource: var source, SelectedIndex: var index and not -1 })
		{
			((App)Application.Current).Preference.UIPreferences.SelectedTechnique = source[index].Technique;
		}
	}
}

/// <summary>
/// Defines a self-reporting progress type.
/// </summary>
/// <param name="handler"><inheritdoc cref="Progress{T}.Progress(Action{T})" path="/param[@name='handler']"/></param>
/// <typeparam name="T"><inheritdoc cref="Progress{T}" path="/typeparam[@name='T']"/></typeparam>
file sealed class SelfReportingProgress<T>(Action<T> handler) : Progress<T>(handler)
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
/// <param name="SelectedTechnique">Indicates the selected technique that you want it to be appeared in generated puzzles.</param>
file readonly record struct GeneratingDetails(
	DifficultyLevel DifficultyLevel,
	SymmetricType SymmetricPattern,
	bool ShouldBeMinimal,
	bool ShouldBePearl,
	Technique SelectedTechnique
);
