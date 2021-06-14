using System;
using System.Windows;
using System.Windows.Controls;
using Sudoku.DocComments;

namespace Sudoku.Windows.CustomControls
{
	/// <summary>
	/// Interaction logic for <c>ColorComponentSlider.xaml</c>.
	/// </summary>
	public partial class ColorComponentSlider : UserControl
	{
		/// <summary>
		/// Indicates the updating values.
		/// </summary>
		protected bool _updatingValues = false;


		/// <summary>
		/// Initializes a default <see cref="ColorComponentSlider"/> instance.
		/// </summary>
		public ColorComponentSlider() => InitializeComponent();


		/// <summary>
		/// The format string.
		/// </summary>
		public string FormatString { get; set; } = "F2";


		/// <summary>
		/// The event triggering while value changed.
		/// </summary>
		public event ValueChangedEventHandler? ValueChanged;


		/// <inheritdoc cref="Events.ValueChanged(object?, EventArgs)"/>
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double value = _slider.Value;
			if (!_updatingValues)
			{
				_updatingValues = true;
				_textBox.Text = value.ToString(FormatString);
				ValueChanged?.Invoke(value);
				_updatingValues = false;
			}
		}

		/// <inheritdoc cref="Events.TextChanged(object?, EventArgs)"/>
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!_updatingValues && double.TryParse(_textBox.Text, out double parsedValue))
			{
				_updatingValues = true;
				_slider.Value = parsedValue;
				ValueChanged?.Invoke(parsedValue);
				_updatingValues = false;
			}
		}
	}
}
