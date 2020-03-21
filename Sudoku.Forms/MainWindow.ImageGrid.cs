using System.Windows.Input;
using w = System.Windows;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
		{
			// This method is only used for testing the coordinates.
			//if (sender is w::Controls.Image imageControl)
			//{
			//	var (x, y) = e.GetPosition(imageControl);
			//	_textBoxInfo.Text = $"{(int)x}, {(int)y}";
			//}
		}

		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is w::Controls.Image imageControl)
			{
				int cell = _pointConverter.GetCellOffset(
					ToDrawingPoint(e.GetPosition(imageControl)));

				_textBoxInfo.Text = $"r{cell / 9 + 1}c{cell % 9 + 1}";
			}
		}
	}
}
