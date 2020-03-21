using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Save failed due to:{Environment.NewLine}{ex.Message}.", "Warning");
			}
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMe().Show();

		private void MenuItemFileQuit_Executed(object sender, ExecutedRoutedEventArgs e) =>
			Close();
	}
}
