using System.SourceGeneration;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.ComponentModel;
using SudokuStudio.Configuration;
using SudokuStudio.Interaction;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents an analysis preference items page.
/// </summary>
[DependencyProperty<StepSearcherInfo>("CurrentSelectedStepSearcher", IsNullable = true, Accessibility = Accessibility.Internal, DocSummary = "Indicates the currently selected step searcher and its details.")]
public sealed partial class AnalysisPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AnalysisPreferenceItemsPage"/> instance.
	/// </summary>
	public AnalysisPreferenceItemsPage() => InitializeComponent();


	private void StepSearcherView_ItemSelected(StepSearcherListView sender, StepSearcherListViewItemSelectedEventArgs e)
		=> CurrentSelectedStepSearcher = e.SelectedSearcherInfo;
}
