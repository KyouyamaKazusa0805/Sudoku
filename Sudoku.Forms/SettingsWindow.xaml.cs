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
			_checkBoxShowCandidates.IsChecked = Settings.ShowCandidates;
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

		private void CheckBoxShowCandidates_Click(object sender, RoutedEventArgs e) =>
			_checkBoxShowCandidates.IsChecked = Settings.ShowCandidates ^= true;

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

			Settings.BackgroundColor = dialog.SelectedColor;
			_buttonBackgroundColor.Background = new w::Media.SolidColorBrush(Settings.BackgroundColor.ToWColor());
		}

		private void ButtonGivenColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.GivenColor = dialog.SelectedColor;
			_buttonGivenColor.Background = new w::Media.SolidColorBrush(Settings.GivenColor.ToWColor());
		}

		private void ButtonModifiableColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.ModifiableColor = dialog.SelectedColor;
			_buttonModifiableColor.Background = new w::Media.SolidColorBrush(Settings.ModifiableColor.ToWColor());
		}

		private void ButtonCandidateColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.CandidateColor = dialog.SelectedColor;
			_buttonCandidateColor.Background = new w::Media.SolidColorBrush(Settings.ModifiableColor.ToWColor());
		}
	}
}
