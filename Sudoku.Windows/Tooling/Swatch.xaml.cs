using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sudoku.Windows.Tooling
{
	public delegate void PickingColorHandlerEventHandler(Color color);

	/// <summary>
	/// Interaction logic for <c>Swatch.xaml</c>.
	/// </summary>
	public partial class Swatch : UserControl
	{
		/// <include file='../../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
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
		public static ColorPickerControl? ColorPickerControl { get; set; }


		/// <summary>
		/// The event triggering while picking colors.
		/// </summary>
		public event PickingColorHandlerEventHandler? PickingColor;


		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!(sender is Border border))
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

				if (!(ColorPickerControl is null))
				{
					ColorPickerControl.CustomColorsChanged();
				}
			}
			else
			{
				PickingColor?.Invoke(((SolidColorBrush)border.Background).Color);
			}
		}

		internal ICollection<ColorSwatchItem> GetColors() =>
			SwatchListBox.ItemsSource as List<ColorSwatchItem> ?? new List<ColorSwatchItem>();
	}
}
