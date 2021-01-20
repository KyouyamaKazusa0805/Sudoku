using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sudoku.DocComments;
using Sudoku.Windows.Extensions;

namespace Sudoku.Windows.CustomControls
{
	/// <summary>
	/// Interaction logic for <c>Swatch.xaml</c>.
	/// </summary>
	public partial class Swatch : UserControl
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public Swatch() => InitializeComponent();


		/// <summary>
		/// Indicates whether the current color can be editable.
		/// </summary>
		public bool Editable { get; set; }

		/// <summary>
		/// Indicates the current color chosen.
		/// </summary>
		public Color CurrentColor { get; set; } = Colors.White;


		/// <summary>
		/// Indicates the color picker control instance.
		/// </summary>
		public static ColorPicker? ColorPickerControl { get; set; }


		/// <summary>
		/// The event triggering while picking colors.
		/// </summary>
		public event PickingColorEventHandler? PickingColor;


		/// <inheritdoc cref="Events.MouseDown(object?, System.EventArgs)"/>
		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is not Border border)
			{
				e.Handled = true;
				return;
			}

			if (Editable && Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				border.Background = new SolidColorBrush(CurrentColor);

				if (border.DataContext is ColorSwatchItem data)
				{
					data.Color = CurrentColor;
					data.HexString = CurrentColor.ToHexString();
				}

				ColorPickerControl?.CustomColorsChanged();
			}
			else
			{
				PickingColor?.Invoke(((SolidColorBrush)border.Background).Color);
			}
		}

		/// <summary>
		/// Get all colors.
		/// </summary>
		/// <returns>The colors.</returns>
		internal ICollection<ColorSwatchItem> GetColors() =>
			_swatchListBox.ItemsSource as List<ColorSwatchItem> ?? new();
	}
}
