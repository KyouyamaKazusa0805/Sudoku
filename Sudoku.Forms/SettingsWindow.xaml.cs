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
		/// The base window.
		/// </summary>
		private readonly MainWindow _baseWindow;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public SettingsWindow(MainWindow baseWindow)
		{
			InitializeComponent();

			_baseWindow = baseWindow;

			// Show controls with the specified settings.
			_checkBoxShowCandidates.IsChecked = _baseWindow.Settings.ShowCandidates;
			_textBoxGridLineWidth.Text = _baseWindow.Settings.GridLineWidth.ToString();
			_textBoxBlockLineWidth.Text = _baseWindow.Settings.BlockLineWidth.ToString();
			_textBoxValueScale.Text = _baseWindow.Settings.ValueScale.ToString();
			_textBoxCandidateScale.Text = _baseWindow.Settings.CandidateScale.ToString();
			_labelGivenFontName.Content = _baseWindow.Settings.GivenFontName;
			_labelModifiableFontName.Content = _baseWindow.Settings.ModifiableFontName;
			_labelCandidateFontName.Content = _baseWindow.Settings.CandidateFontName;

			Callback += () => baseWindow._updateControlStatus.Invoke(this, EventArgs.Empty);
		}


		/// <summary>
		/// Indicates an event triggering when the form closed, the data will be
		/// called back to the main form (i.e. <see cref="MainWindow"/>).
		/// </summary>
		public event CallbackEventHandler Callback;


		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			Callback.Invoke();

			base.OnClosing(e);
		}


		private void CheckBoxShowCandidates_Click(object sender, RoutedEventArgs e) =>
			_checkBoxShowCandidates.IsChecked = _baseWindow.Settings.ShowCandidates ^= true;

		private void TextBoxGridLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!float.TryParse(textBox.Text, out float value))
				{
					e.Handled = true;
					return;
				}

				_baseWindow.Settings.GridLineWidth = value;
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

				_baseWindow.Settings.BlockLineWidth = value;
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

				_baseWindow.Settings.ValueScale = value;
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

				_baseWindow.Settings.CandidateScale = value;
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
