using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Windows;
using System.Windows.Media;
using Sudoku.DocComments;
using Sudoku.Windows.CustomControls;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>ColorPickerWindow.xaml</c>.
	/// </summary>
	public partial class ColorPickerWindow : Window
	{
		/// <summary>
		/// The minimum or maximum width of the window.
		/// </summary>
		private const int WidthMaxValue = 574, WidthMinValue = 342;


		/// <summary>
		/// Initializes a <see cref="ColorPickerWindow"/> instance.
		/// </summary>
		public ColorPickerWindow() => InitializeComponent();


		/// <summary>
		/// Indicates whether the mode is simple mode (only shows basic options).
		/// </summary>
		protected bool SimpleMode { get; set; }


		/// <inheritdoc cref="Events.Click(object?, System.EventArgs)"/>
		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Hide();
		}

		/// <inheritdoc cref="Events.Click(object?, System.EventArgs)"/>
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Hide();
		}

		/// <inheritdoc cref="Events.Click(object?, System.EventArgs)"/>
		private void MinMaxViewButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (SimpleMode)
			{
				SimpleMode = false;
				_buttonMinMaxView.Content = Application.Current.Resources["ColorPickerButtonMinimumView"];
				Width = WidthMaxValue;
			}
			else
			{
				SimpleMode = true;
				_buttonMinMaxView.Content = Application.Current.Resources["ColorPickerButtonMaximumView"];
				Width = WidthMinValue;
			}
		}

		/// <summary>
		/// Toggle simple advanced view.
		/// </summary>
		public void ToggleSimpleAdvancedView()
		{
			if (SimpleMode)
			{
				SimpleMode = false;
				_buttonMinMaxView.Content = Application.Current.Resources["ColorPickerButtonMinimumView"];
				Width = WidthMaxValue;
			}
			else
			{
				SimpleMode = true;
				_buttonMinMaxView.Content = Application.Current.Resources["ColorPickerButtonMaximumView"];
				Width = WidthMinValue;
			}
		}


		/// <summary>
		/// Show the dialog.
		/// </summary>
		/// <param name="color">The color selected.</param>
		/// <param name="flags">The flags that the control should enable.</param>
		/// <param name="customPreviewEventHandler">
		/// The event handler. The default value is <see langword="null"/>.
		/// </param>
		/// <returns>
		/// Return <see langword="true"/> if a user has chosen a color successfully;
		/// otherwise, <see langword="false"/> (i.e. user canceled).
		/// </returns>
		public static bool ShowDialog(
			[NotNullWhen(true)] out Color? color, ColorPickerOptions flags = ColorPickerOptions.None,
			PickingColorEventHandler? customPreviewEventHandler = null)
		{
			if (flags.Flags(ColorPickerOptions.LoadCustomPalette))
			{
				ColorPickerSettings.UsingCustomPalette = true;
			}

			var instance = new ColorPickerWindow();
			color = instance._colorPickerMain.Color;

			if (flags.Flags(ColorPickerOptions.SimpleView))
			{
				instance.ToggleSimpleAdvancedView();
			}

			if (ColorPickerSettings.UsingCustomPalette)
			{
				instance._colorPickerMain.LoadDefaultCustomPalette();
			}

			if (customPreviewEventHandler is not null)
			{
				instance._colorPickerMain.PickingColor += customPreviewEventHandler;
			}

			if (instance.ShowDialog() is true)
			{
				color = instance._colorPickerMain.Color ?? default;
				return true;
			}

			return false;
		}
	}
}
