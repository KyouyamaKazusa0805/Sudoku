namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a <see cref="ListView"/> control that displays for <see cref="IStepSearcher"/> instances.
/// </summary>
[DependencyProperty<ObservableCollection<StepSearcherSerializationData>>("StepSearchers", DocSummary = "Indicates the step searchers.")]
public sealed partial class StepSearcherListView : UserControl
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherListView"/> instance.
	/// </summary>
	public StepSearcherListView() => InitializeComponent();
}
