namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a color selector control.
/// </summary>
public sealed partial class ColorSelector : UserControl, INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the dependency property that binds with the property <see cref="SelectedColor"/>.
	/// </summary>
	/// <seealso cref="SelectedColor"/>
	public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
		nameof(SelectedColor),
		typeof(Color),
		typeof(ColorSelector),
		new(default(Color))
	);


	/// <summary>
	/// Indicates the inner color.
	/// </summary>
	private Color _selectedColor;


	/// <summary>
	/// Initializes a <see cref="ColorSelector"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ColorSelector() => InitializeComponent();


	/// <summary>
	/// Indicates the selected color. The default value is <see cref="Colors.Black"/>.
	/// </summary>
	/// <seealso cref="Colors.Black"/>
	public Color SelectedColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _selectedColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_selectedColor = value;

			_cCurrentColorBorder.Background = new SolidColorBrush(value);

			PropertyChanged?.Invoke(this, new(nameof(SelectedColor)));
		}
	}


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// Triggers when the item is being clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GridView_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.ClickedItem is not Color color)
		{
			return;
		}

		SelectedColor = color;

		_cSplitButton.Flyout.Hide();
	}
}
