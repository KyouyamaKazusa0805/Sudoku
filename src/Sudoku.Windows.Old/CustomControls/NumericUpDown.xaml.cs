namespace Sudoku.Windows.CustomControls;

/// <summary>
/// Interaction logic for <c>NumericUpDown.xaml</c>.
/// </summary>
public partial class NumericUpDown : UserControl
{
	/// <summary>
	/// Indicates the current value.
	/// </summary>
	private decimal _currentValue;


	/// <summary>
	/// Initializes a default <see cref="NumericUpDown"/> instance.
	/// </summary>
	public NumericUpDown() => InitializeComponent();


	/// <summary>
	/// Indicates the current value.
	/// </summary>
	/// <value>The value to set.</value>
	public decimal CurrentValue
	{
		get => _currentValue;

		set
		{
			if (value >= MinValue && value <= MaxValue)
			{
				_textBoxInner.Text = (_currentValue = value).ToString();
				_buttonUp.IsEnabled = value != MaxValue;
				_buttonDown.IsEnabled = value != MinValue;

				ValueChanged?.Invoke(this, new());
			}
		}
	}

	/// <summary>
	/// Indicates the minimum value the control supports.
	/// </summary>
	public decimal MinValue { get; set; } = 0;

	/// <summary>
	/// Indicates the maximum value the control supports.
	/// </summary>
	public decimal MaxValue { get; set; } = 100M;

	/// <summary>
	/// Indicates the increasing unit.
	/// </summary>
	public decimal IncreasingUnit { get; set; } = 1M;


	/// <summary>
	/// Indicates the event triggering when the value changed.
	/// </summary>
	public event RoutedEventHandler? ValueChanged;


	private void TextBoxInner_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is TextBox textBox)
		{
			if (decimal.TryParse(textBox.Text, out decimal value))
			{
				CurrentValue = value;
			}
			else
			{
				textBox.Text = CurrentValue.ToString();
			}
		}
	}

	private void ButtonUp_Click(object sender, RoutedEventArgs e) =>
		_textBoxInner.Text = (CurrentValue += IncreasingUnit).ToString();

	private void ButtonDown_Click(object sender, RoutedEventArgs e) =>
		_textBoxInner.Text = (CurrentValue -= IncreasingUnit).ToString();
}
