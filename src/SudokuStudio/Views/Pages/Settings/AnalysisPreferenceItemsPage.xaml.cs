using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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


	private void GoToStepSearcherSorterPageSettingsCard_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<StepSearcherSorterPage>();
}
