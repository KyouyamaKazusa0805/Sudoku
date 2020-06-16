using System.Windows;
using System.Windows.Controls;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>NumericUpDown.xaml</c>.
	/// </summary>
	public partial class NumericUpDown : UserControl
	{
		/// <summary>
		/// Indicates the current value.
		/// </summary>
		private int _currentValue;


		/// <include file='../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public NumericUpDown() => InitializeComponent();


		/// <summary>
		/// Indicates the current value.
		/// </summary>
		/// <value>The value to set.</value>
		public int CurrentValue
		{
			get => _currentValue;
			set
			{
				if (value >= MinValue && value <= MaxValue)
				{
					_textBoxInner.Text = (_currentValue = value).ToString();
					_buttonUp.IsEnabled = value != MaxValue;
					_buttonDown.IsEnabled = value != MinValue;

					ValueChanged?.Invoke(this, new RoutedEventArgs());
				}
			}
		}

		/// <summary>
		/// Indicates the minimum value the control supports.
		/// </summary>
		public int MinValue { get; set; } = 0;

		/// <summary>
		/// Indicates the maximum value the control supports.
		/// </summary>
		public int MaxValue { get; set; } = 100;


		/// <summary>
		/// Indicates the event triggering when the value changed.
		/// </summary>
		public event RoutedEventHandler? ValueChanged;


		private void TextBoxInner_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (int.TryParse(textBox.Text, out int value))
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
			_textBoxInner.Text = (++CurrentValue).ToString();

		private void ButtonDown_Click(object sender, RoutedEventArgs e) =>
			_textBoxInner.Text = (--CurrentValue).ToString();
	}
}
