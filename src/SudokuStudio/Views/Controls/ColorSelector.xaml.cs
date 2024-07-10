namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a color selector.
/// </summary>
public sealed partial class ColorSelector : UserControl
{
	[Default]
	private static readonly Color SelectedColorDefaultValue = Colors.Transparent;


	/// <summary>
	/// Initializes a <see cref="ColorSelector"/> instance.
	/// </summary>
	public ColorSelector() => InitializeComponent();


	/// <summary>
	/// Indicates the inner color.
	/// </summary>
	[AutoDependencyProperty]
	public partial Color SelectedColor { get; set; }


	/// <summary>
	/// Indicates the event triggered when the selected color is changed.
	/// </summary>
	public event EventHandler<Color>? ColorChanged;


	/// <summary>
	/// Triggers when the item is being clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void GridView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.ClickedItem is not Color color)
		{
			return;
		}

		SplitButton.Flyout.Hide();

		ColorChanged?.Invoke(this, SelectedColor = color);
	}

	/// <summary>
	/// Triggers when the color picker has changed its color.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
		=> ColorChanged?.Invoke(this, SelectedColor = args.NewColor);

	/// <summary>
	/// Triggers when the "More" button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void HyperlinkButton_Click(object sender, RoutedEventArgs e) => MoreButton.ContextFlyout.ShowAt(MoreButton);
}
