namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a color selector.
/// </summary>
public sealed partial class ColorSelector : UserControl, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the dependency property that binds with the property <see cref="SelectedColor"/>.
	/// </summary>
	/// <seealso cref="SelectedColor"/>
	public static readonly DependencyProperty SelectedColorProperty =
		DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorSelector), new(Colors.Transparent));


	/// <summary>
	/// Indicates the inner color.
	/// </summary>
	[NotifyBackingField]
	[NotifyCallback]
	private Color _selectedColor;


	/// <summary>
	/// Initializes a <see cref="ColorSelector"/> instance.
	/// </summary>
	public ColorSelector() => InitializeComponent();


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	private void SelectedColorSetterAfter(Color value) => CurrentColorBorder.Background = new SolidColorBrush(value);


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

		SelectedColor = color;

		SplitButton.Flyout.Hide();
	}

	/// <summary>
	/// Triggers when the color picker has changed its color.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args) => SelectedColor = args.NewColor;

	/// <summary>
	/// Triggers when the "More" button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void HyperlinkButton_Click(object sender, RoutedEventArgs e) => MoreButton.ContextFlyout.ShowAt(MoreButton);
}
