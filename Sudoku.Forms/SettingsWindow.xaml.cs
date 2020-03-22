using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
			_labelModifiableFontName.Content = Settings.ModifiableFontName;
			_labelCandidateFontName.Content = Settings.CandidateFontName;
		}


		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			DialogResult = true;

			base.OnClosing(e);
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
			// TODO: Design a font dialog.
		}

		private void ButtonModifiableFontName_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Design a font dialog.
		}

		private void ButtonCandidateFontName_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Design a font dialog.
		}
	}
}
