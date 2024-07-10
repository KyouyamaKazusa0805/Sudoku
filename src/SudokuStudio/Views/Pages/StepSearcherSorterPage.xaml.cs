namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents step searcher sorter page.
/// </summary>
public sealed partial class StepSearcherSorterPage : Page
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherSorterPage"/> instance.
	/// </summary>
	public StepSearcherSorterPage() => InitializeComponent();


	/// <summary>
	/// Indicates the currently selected step searcher and its details.
	/// </summary>
	[DependencyProperty]
	internal partial StepSearcherInfo? CurrentSelectedStepSearcher { get; set; }


	private void StepSearcherView_ItemSelected(StepSearcherListView sender, StepSearcherListViewItemSelectedEventArgs e)
		=> CurrentSelectedStepSearcher = e.SelectedSearcherInfo;
}
