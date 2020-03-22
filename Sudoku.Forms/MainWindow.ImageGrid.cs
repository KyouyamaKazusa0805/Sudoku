using System.Windows.Input;
using Sudoku.Drawing.Extensions;
using Sudoku.Forms.Drawing.Layers;
using ImageControl = System.Windows.Controls.Image;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is ImageControl imageControl)
			{
				_focusedCells.Add(
					_pointConverter.GetCellOffset(
						e.GetPosition(imageControl).ToDPointF()));

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
						e.GetPosition(imageControl).ToDPointF()));

				if (_focusedCells.Count == 0)
				{
					_layerCollection.RemoveAll(typeof(FocusLayer).Name);
				}

				UpdateImageGrid();
			}
		}
	}
}
