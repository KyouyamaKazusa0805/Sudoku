namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a <see cref="ListView"/> control that displays for <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
[DependencyProperty<ObservableCollection<StepSearcherInfo>>("StepSearchers", DocSummary = "Indicates the step searchers.")]
public sealed partial class StepSearcherListView : UserControl
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherListView"/> instance.
	/// </summary>
	public StepSearcherListView() => InitializeComponent();


	/// <summary>
	/// Indicates the event triggered when an item is selected.
	/// </summary>
	public event StepSearcherListViewItemSelectedEventHandler? ItemSelected;


	private void MainListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
	{
		if (e is not { Items: [StepSearcherInfo { CanDrag: true }] })
		{
			e.Cancel = true;
		}
	}

	private void MainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> ItemSelected?.Invoke(this, new((StepSearcherInfo)MainListView.SelectedItem));
}
