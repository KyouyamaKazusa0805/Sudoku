namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a <see cref="ListView"/> control that displays for <see cref="IStepSearcher"/> instances.
/// </summary>
[DependencyProperty<ObservableCollection<IStepSearcher>>("StepSearchers", DefaultValueGeneratingMemberName = nameof(StepSearchersDefaultValue), DocSummary = "Indicates the step searchers.")]
public sealed partial class StepSearcherListView : UserControl
{
	private static readonly ObservableCollection<IStepSearcher> StepSearchersDefaultValue = new(StepSearcherPool.DefaultCollection);


	/// <summary>
	/// Initializes a <see cref="StepSearcherListView"/> instance.
	/// </summary>
	public StepSearcherListView() => InitializeComponent();
}
