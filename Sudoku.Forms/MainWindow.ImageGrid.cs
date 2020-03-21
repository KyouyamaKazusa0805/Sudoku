using System.Windows.Input;
using Sudoku.Forms.Drawing.Layers;
using ImageControl = System.Windows.Controls.Image;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
		{
			// This method is only used for testing the coordinates.
			//if (sender is ImageControl imageControl)
			//{
			//	var (x, y) = e.GetPosition(imageControl);
			//	_textBoxInfo.Text = $"{(int)x}, {(int)y}";
			//}
		}

		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is ImageControl imageControl)
			{
				_focusedCells.Add(
					_pointConverter.GetCellOffset(
						ToDrawingPoint(e.GetPosition(imageControl))));

				_layerCollection.Add(
					new FocusLayer(
						_pointConverter, _focusedCells, _settings.FocusedCellColor));

				UpdateImageGrid();
			}
		}

		private void ImageGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is ImageControl imageControl)
			{
				_focusedCells.Remove(
					_pointConverter.GetCellOffset(
						ToDrawingPoint(e.GetPosition(imageControl))));

				if (_focusedCells.Count == 0)
				{
					_layerCollection.RemoveAll(typeof(FocusLayer).Name);
				}

				UpdateImageGrid();
			}
		}
	}
}
