namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Provides a view that displays for technique groups found.
/// </summary>
public sealed partial class TechniqueGroupView : UserControl
{
	/// <summary>
	/// Initializes a <see cref="TechniqueGroupView"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TechniqueGroupView() => InitializeComponent();


	/// <summary>
	/// Indicates the event that a step is chosen.
	/// </summary>
	public event EventHandler<IStep>? StepChosen;


	/// <summary>
	/// Clears the view.
	/// </summary>
	public void ClearView() => _cTechniqueGroups.View?.Clear();


	/// <summary>
	/// Triggers when an item is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ListView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.ClickedItem is not IStep step)
		{
			return;
		}

		StepChosen?.Invoke(this, step);
	}
}
