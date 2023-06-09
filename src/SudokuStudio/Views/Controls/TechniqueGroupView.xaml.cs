namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a technique group view control.
/// </summary>
public sealed partial class TechniqueGroupView : UserControl
{
	/// <summary>
	/// Initializes a <see cref="TechniqueGroupView"/> instance.
	/// </summary>
	public TechniqueGroupView() => InitializeComponent();


	/// <summary>
	/// Indicates the event that a step is applied.
	/// </summary>
	public event TechniqueGroupViewStepAppliedEventHandler? StepApplied;

	/// <summary>
	/// Indicates the event that a step is chosen.
	/// </summary>
	public event TechniqueGroupViewStepChosenEventHandler? StepChosen;


	/// <summary>
	/// Clears the view source.
	/// </summary>
	public void ClearViewSource() => TechniqueGroups.Source = null;


	private void ListView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.ClickedItem is SolvingPathStepBindableSource { Step: var step })
		{
			StepChosen?.Invoke(this, new(step));
		}
	}

	private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
	{
		if (sender is ListViewItem { Tag: SolvingPathStepBindableSource { Step: var step } })
		{
			StepApplied?.Invoke(this, new(step));
		}
	}
}
