using System.SourceGeneration;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.ComponentModel;
using SudokuStudio.Configuration;
using SudokuStudio.Interaction;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents step searcher sorter page.
/// </summary>
[DependencyProperty<StepSearcherInfo>("CurrentSelectedStepSearcher", IsNullable = true, Accessibility = Accessibility.Internal, DocSummary = "Indicates the currently selected step searcher and its details.")]
public sealed partial class StepSearcherSorterPage : Page
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherSorterPage"/> instance.
	/// </summary>
	public StepSearcherSorterPage() => InitializeComponent();


	private void StepSearcherView_ItemSelected(StepSearcherListView sender, StepSearcherListViewItemSelectedEventArgs e)
		=> CurrentSelectedStepSearcher = e.SelectedSearcherInfo;
}
