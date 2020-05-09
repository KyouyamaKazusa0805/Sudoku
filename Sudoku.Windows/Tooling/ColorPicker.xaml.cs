using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using static Sudoku.Windows.Tooling.ColorPickerOptions;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>ColorPicker.xaml</c>.
	/// </summary>
	public partial class ColorPicker : Window
	{
		/// <summary>
		/// The maximum width that the window lies.
		/// </summary>
		protected readonly int _widthMax = 574;

		/// <summary>
		/// The minimum width that the window lies.
		/// </summary>
		protected readonly int _widthMin = 342;


		/// <include file='../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public ColorPicker() => InitializeComponent();


		/// <summary>
		/// Indicates whether the mode is simple mode (only shows basic options).
		/// </summary>
		protected bool SimpleMode { get; set; }


		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Hide();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Hide();
		}

		private void MinMaxViewButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (SimpleMode)
			{
				SimpleMode = false;
				_minMaxViewButton.Content = "<< Simple";
				Width = _widthMax;
			}
			else
			{
				SimpleMode = true;
				_minMaxViewButton.Content = "Advanced >>";
				Width = _widthMin;
			}
		}

		public void ToggleSimpleAdvancedView()
		{
			if (SimpleMode)
			{
				SimpleMode = false;
				_minMaxViewButton.Content = "<< Simple";
				Width = _widthMax;
			}
			else
			{
				SimpleMode = true;
				_minMaxViewButton.Content = "Advanced >>";
				Width = _widthMin;
			}
		}


		/// <summary>
		/// Show the dialog.
		/// </summary>
		/// <param name="color">(<see langword="out"/> parameter) The color selected.</param>
		/// <param name="flags">The flags that the control should enable.</param>
		/// <param name="customPreviewEventHandler">
		/// The event handler. The default value is <see langword="null"/>.
		/// </param>
		/// <returns>
		/// Return <see langword="true"/> if a user has chosen a color successfully;
		/// otherwise, <see langword="false"/> (i.e. user canceled).
		/// </returns>
		public static bool ShowDialog(
			[NotNullWhen(true)] out Color? color, ColorPickerOptions flags = None,
			ColorPickerChangedEventHandler? customPreviewEventHandler = null)
		{
			if ((flags & LoadCustomPalette) == LoadCustomPalette)
			{
				ColorPickerSettings.UsingCustomPalette = true;
			}

			var instance = new ColorPicker();
			color = instance._colorPicker.Color;

			if ((flags & SimpleView) == SimpleView)
			{
				instance.ToggleSimpleAdvancedView();
			}

			if (ColorPickerSettings.UsingCustomPalette)
			{
				instance._colorPicker.LoadDefaultCustomPalette();
			}

			if (!(customPreviewEventHandler is null))
			{
				instance._colorPicker.PickingColor += customPreviewEventHandler;
			}

			if (instance.ShowDialog() is true)
			{
				color = instance._colorPicker.Color ?? default;
				return true;
			}

			return false;
		}
	}
}
