using System.Windows;
using System.Windows.Controls;
using Sudoku.Forms.Drawing.Extensions;
using Sudoku.Forms.Tooling;
using w = System.Windows;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		/// <summary>
		/// Indicates the result settings.
		/// </summary>
		public Settings Settings { get; }


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public SettingsWindow(MainWindow baseWindow)
		{
			InitializeComponent();

			Settings = baseWindow.Settings;

			// Show controls with the specified settings.
			_checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting;
			_textBoxGridLineWidth.Text = Settings.GridLineWidth.ToString();
			_textBoxBlockLineWidth.Text = Settings.BlockLineWidth.ToString();
			_textBoxValueScale.Text = Settings.ValueScale.ToString();
			_textBoxCandidateScale.Text = Settings.CandidateScale.ToString();
			_labelGivenFontName.Content = Settings.GivenFontName;
			_labelGivenFontName.FontFamily = new w::Media.FontFamily(Settings.GivenFontName);
			_labelModifiableFontName.Content = Settings.ModifiableFontName;
			_labelModifiableFontName.FontFamily = new w::Media.FontFamily(Settings.ModifiableFontName);
			_labelCandidateFontName.Content = Settings.CandidateFontName;
			_labelCandidateFontName.FontFamily = new w::Media.FontFamily(Settings.CandidateFontName);
			_buttonBackgroundColor.Background = new w::Media.SolidColorBrush(Settings.BackgroundColor.ToWColor());
			_buttonGivenColor.Background = new w::Media.SolidColorBrush(Settings.GivenColor.ToWColor());
			_buttonModifiableColor.Background = new w::Media.SolidColorBrush(Settings.ModifiableColor.ToWColor());
			_buttonCandidateColor.Background = new w::Media.SolidColorBrush(Settings.CandidateColor.ToWColor());
			_buttonFocusColor.Background = new w::Media.SolidColorBrush(Settings.FocusedCellColor.ToWColor());
			_buttonGridLineColor.Background = new w::Media.SolidColorBrush(Settings.GridLineColor.ToWColor());
			_buttonBlockLineColor.Background = new w::Media.SolidColorBrush(Settings.BlockLineColor.ToWColor());
			_buttonColor1.Background = new w::Media.SolidColorBrush(Settings.Color1.ToWColor());
			_buttonColor2.Background = new w::Media.SolidColorBrush(Settings.Color2.ToWColor());
			_buttonColor3.Background = new w::Media.SolidColorBrush(Settings.Color3.ToWColor());
			_buttonColor4.Background = new w::Media.SolidColorBrush(Settings.Color4.ToWColor());
			_buttonColor5.Background = new w::Media.SolidColorBrush(Settings.Color5.ToWColor());
			_buttonColor6.Background = new w::Media.SolidColorBrush(Settings.Color6.ToWColor());
			_buttonColor7.Background = new w::Media.SolidColorBrush(Settings.Color7.ToWColor());
			_buttonColor8.Background = new w::Media.SolidColorBrush(Settings.Color8.ToWColor());
			_buttonColor9.Background = new w::Media.SolidColorBrush(Settings.Color9.ToWColor());
			_buttonColor10.Background = new w::Media.SolidColorBrush(Settings.Color10.ToWColor());
			_buttonColor11.Background = new w::Media.SolidColorBrush(Settings.Color11.ToWColor());
			_buttonColor12.Background = new w::Media.SolidColorBrush(Settings.Color12.ToWColor());
			_buttonColor13.Background = new w::Media.SolidColorBrush(Settings.Color13.ToWColor());
			_buttonColor14.Background = new w::Media.SolidColorBrush(Settings.Color14.ToWColor());
			_buttonColor15.Background = new w::Media.SolidColorBrush(Settings.Color15.ToWColor());
		}


		/// <summary>
		/// To handle the color settings.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="e">The event.</param>
		/// <param name="settings">The setting target instance.</param>
		/// <param name="colorIndex">The index.</param>
		private void HandleColor(object sender, RoutedEventArgs e, Settings settings, int colorIndex)
		{
			var dialog = new ColorDialog();

			if (!(sender is Button button) || !(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			typeof(Settings).GetProperty($"Color{colorIndex}")!.SetValue(settings, dialog.SelectedColor);
			button.Foreground = new w::Media.SolidColorBrush(dialog.SelectedColor.ToWColor());
		}


		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			Close();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;

			Close();
		}

		private void CheckBoxAskWhileQuitting_Click(object sender, RoutedEventArgs e) =>
			_checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting ^= true;

		private void TextBoxGridLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!float.TryParse(textBox.Text, out float value))
				{
					e.Handled = true;
					return;
				}

				Settings.GridLineWidth = value;
			}
		}

		private void TextBoxBlockLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!float.TryParse(textBox.Text, out float value))
				{
					e.Handled = true;
					return;
				}

				Settings.BlockLineWidth = value;
			}
		}

		private void TextBoxValueScale_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!decimal.TryParse(textBox.Text, out decimal value))
				{
					e.Handled = true;
					return;
				}

				Settings.ValueScale = value;
			}
		}

		private void TextBoxCandidateScale_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!decimal.TryParse(textBox.Text, out decimal value))
				{
					e.Handled = true;
					return;
				}

				Settings.CandidateScale = value;
			}
		}

		private void ButtonGivenFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_labelGivenFontName.FontFamily = new w::Media.FontFamily(
				Settings.GivenFontName = dialog.SelectedFont.Name);
		}

		private void ButtonModifiableFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_labelModifiableFontName.FontFamily = new w::Media.FontFamily(
				Settings.ModifiableFontName = dialog.SelectedFont.Name);
		}

		private void ButtonCandidateFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_labelCandidateFontName.FontFamily = new w::Media.FontFamily(
				Settings.CandidateFontName = dialog.SelectedFont.Name);
		}

		private void ButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonBackgroundColor.Background = new w::Media.SolidColorBrush(
				(Settings.BackgroundColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonGivenColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonGivenColor.Background = new w::Media.SolidColorBrush(
				(Settings.GivenColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonModifiableColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonModifiableColor.Background = new w::Media.SolidColorBrush(
				(Settings.ModifiableColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonCandidateColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonCandidateColor.Background = new w::Media.SolidColorBrush(
				(Settings.CandidateColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonFocusColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonCandidateColor.Background = new w::Media.SolidColorBrush(
				(Settings.FocusedCellColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonGridLineColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonCandidateColor.Background = new w::Media.SolidColorBrush(
				(Settings.GridLineColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonBlockLineColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonCandidateColor.Background = new w::Media.SolidColorBrush(
				(Settings.BlockLineColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonChainColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonChainColor.Background = new w::Media.SolidColorBrush(
				(Settings.ChainColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonColor1_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 1);

		private void ButtonColor2_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 2);

		private void ButtonColor3_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 3);

		private void ButtonColor4_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 4);

		private void ButtonColor5_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 5);

		private void ButtonColor6_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 6);

		private void ButtonColor7_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 7);

		private void ButtonColor8_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 8);

		private void ButtonColor9_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 9);

		private void ButtonColor10_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 10);

		private void ButtonColor11_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 11);

		private void ButtonColor12_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 12);

		private void ButtonColor13_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 13);

		private void ButtonColor14_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 14);

		private void ButtonColor15_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 15);
	}
}
