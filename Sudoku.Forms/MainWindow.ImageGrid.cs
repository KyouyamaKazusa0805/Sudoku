using System;
using System.Drawing;
using System.Windows.Input;
using Sudoku.Forms.Drawing.Layers;
using Sudoku.Forms.Extensions;
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
				_focusedCells.Add(
					_pointConverter.GetCellOffset(
						ToDrawingPoint(e.GetPosition(imageControl))));

				_layerCollection.Add(
					new FocusLayer(
						_pointConverter, _focusedCells, _settings.FocusedCellColor));

				var bitmap = new Bitmap((int)_imageGrid.Width, (int)_imageGrid.Height);
				_layerCollection.IntegrateTo(bitmap);
				_imageGrid.Source = bitmap.ToImageSource();

				GC.Collect();
			}
		}

		private void ImageGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is w::Controls.Image imageControl)
			{
				_focusedCells.Remove(
					_pointConverter.GetCellOffset(
						ToDrawingPoint(e.GetPosition(imageControl))));

				if (_focusedCells.Count == 0)
				{
					_layerCollection.RemoveAll(typeof(FocusLayer).Name);
				}

				var bitmap = new Bitmap((int)_imageGrid.Width, (int)_imageGrid.Height);
				_layerCollection.IntegrateTo(bitmap);
				_imageGrid.Source = bitmap.ToImageSource();

				GC.Collect();
			}
		}
	}
}
