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
		SetComboBoxSelectedIndices();
	}


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <summary>
	/// Try to set indices.
	/// </summary>
	private void SetComboBoxSelectedIndices()
	{
		var flag = false;
		for (var (i, items, target) = (0, DifficultyLevelSelector.Items, ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel);
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

		for ((var i, var items, var target, flag) = (0, PuzzleSymmetricPatternSelector.Items, ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern, false);
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
	}


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.IsGeneratorLaunched = true;

		BasePage.ClearAnalyzeTabsData();

		var processingText = GetString("AnalyzePage_GeneratorIsProcessing")!;
		var difficultyLevelSelected = ((App)Application.Current).Preference.UIPreferences.GeneratorDifficultyLevel;
		var symmetricType = ((App)Application.Current).Preference.UIPreferences.GeneratorSymmetricPattern;
		var grid = await Task.Run(gridCreator);

		BasePage.IsGeneratorLaunched = false;

		if (((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
		{
			((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = grid });
		}

		BasePage.SudokuPane.Puzzle = grid;


		Grid gridCreator()
		{
			var progress = new SelfReportingProgress<GeneratorProgress>(reportingAction);
			for (var count = 0; ; count++)
			{
				if (HodokuPuzzleGenerator.Generate(symmetricType) is var grid
					&& ((App)Application.Current).Analyzer.Analyze(grid).DifficultyLevel is var puzzleDifficultyLevel
					&& (difficultyLevelSelected == 0 || puzzleDifficultyLevel == difficultyLevelSelected))
				{
					return grid;
				}

				progress.Report(new(count));
			}
		}

		void reportingAction(GeneratorProgress progress)
		{
			DispatcherQueue.TryEnqueue(progressCallback);


			void progressCallback()
			{
				var count = progress.Count;
				BasePage.AnalyzeProgressLabel.Text = processingText;
				BasePage.AnalyzeStepSearcherNameLabel.Text = count.ToString();
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
