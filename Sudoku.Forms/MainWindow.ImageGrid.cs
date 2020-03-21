using System.Windows.Input;
using Sudoku.Forms.Extensions;
using w = System.Windows;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
		{
			if (sender is w::Controls.Image imageControl)
			{
				var (x, y) = e.GetPosition(imageControl);
				_textBoxInfo.Text = $"{(int)x}, {(int)y}";
			}
		}

		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
