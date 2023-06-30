namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents an analysis preference items page.
/// </summary>
public sealed partial class AnalysisPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AnalysisPreferenceItemsPage"/> instance.
	/// </summary>
	public AnalysisPreferenceItemsPage() => InitializeComponent();


	private void StepSearcherView_ItemSelected(StepSearcherListView sender, StepSearcherListViewItemSelectedEventArgs e)
	{
		if (e.SelectedSearcherInfo is not { TypeName: var rawName })
		{
			goto HideControl;
		}

		StepSearcherDetailsDisplayer.Visibility = Visibility.Visible;

		var stepSearchersFound = StepSearcherPool.GetStepSearchers(rawName, false);
		if (stepSearchersFound is not [{ Name: var name, DifficultyLevelRange: var difficultyLevels, SupportedTechniques: var techniques }])
		{
			goto HideControl;
		}

		var commaToken = GetString("_Token_Comma");
		StepSearcherNameDisplayer.Text = string.Format(GetString("AnalysisPreferenceItemsPage_StepSearcherName"), name);
		StepSearcherDifficultyLevelRangeDisplayer.Text = string.Format(
			GetString("AnalysisPreferenceItemsPage_StepSearcherDifficultyLevelRange"),
			string.Join(commaToken, from dl in difficultyLevels select GetString($"_DifficultyLevel_{dl}"))
		);
		StepSearcherSupportedTechniquesDisplayer.Text = string.Format(
			GetString("AnalysisPreferenceItemsPage_StepSearcherSupportedTechniques"),
			string.Join(commaToken, from technique in techniques select technique.GetName())
		);

		return;

	HideControl:
		StepSearcherNameDisplayer.Text = string.Empty;
		StepSearcherDifficultyLevelRangeDisplayer.Text = string.Empty;
		StepSearcherSupportedTechniquesDisplayer.Text = string.Empty;
		StepSearcherDetailsDisplayer.Visibility = Visibility.Collapsed;
	}
}
