namespace SudokuStudio.Views.Pages.Settings.Analysis;

/// <summary>
/// Represents step searcher behaviors setting page.
/// </summary>
public sealed partial class StepSearcherBehaviorsSettingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherBehaviorsSettingPage"/> instance.
	/// </summary>
	public StepSearcherBehaviorsSettingPage() => InitializeComponent();


	private void GoToStepSearcherSorterPageSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage(typeof(StepSearcherSorterPage), true);
}
