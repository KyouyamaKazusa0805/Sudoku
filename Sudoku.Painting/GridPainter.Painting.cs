using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Painting.Extensions;

namespace Sudoku.Painting
{
	partial record GridPainter
	{
		/// <summary>
		/// Paint background.
		/// </summary>
		/// <param name="controls">The collection that stores <see cref="Shape"/>s.</param>
		partial void PaintBackground(IList<Shape> controls) =>
			controls.Add(new Path
			{
				Data = new RectangleGeometry { Rect = new(default, Translator.ControlSize) },
				Fill = new SolidColorBrush(Preferences.BackgroundColor),
				//Stroke = new SolidColorBrush(Preferences.GridLineColor)
			});

		/// <summary>
		/// Paint grid lines and block lines.
		/// </summary>
		/// <param name="controls">The collection that stores <see cref="Shape"/>s.</param>
		partial void PaintGridAndBlockLines(IList<Shape> controls)
		{
			var gridPoints = Translator.GridPoints;

			// Grid lines.
			for (int i = 0; i < Length; i += Length / 9)
			{
				if (i % (Length / 3) == 0)
				{
					continue;
				}

				var (x1, y1) = gridPoints[i, 0];
				var (x2, y2) = gridPoints[i, Length - 1];
				controls.Add(new Line
				{
					Stroke = new SolidColorBrush(Preferences.GridLineColor),
					Width = Preferences.GridLineWidth,
					X1 = x1,
					X2 = x2,
					Y1 = y1,
					Y2 = y2
				});
			}

			// Block lines.
			for (int i = 0; i < Length; i += Length / 3)
			{
				var (x1, y1) = gridPoints[i, 0];
				var (x2, y2) = gridPoints[i, Length - 1];
				controls.Add(new Line
				{
					Stroke = new SolidColorBrush(Preferences.BlockLineColor),
					Width = Preferences.BlockLineWidth,
					X1 = x1,
					X2 = x2,
					Y1 = y1,
					Y2 = y2
				});
			}
		}

		/// <summary>
		/// Paint the view if need.
		/// </summary>
		/// <param name="controls">The collection that stores <see cref="Shape"/>s.</param>
		/// <param name="offset">The offset.</param>
		partial void PaintViewIfNeed(IList<Shape> controls, double offset)
		{
			if (View is not null)
			{
				PaintViewIfNeedInternal(controls, offset, View);
			}
		}

		/// <summary>
		/// Paint the custom view if need.
		/// </summary>
		/// <param name="controls">The collection that stores <see cref="Shape"/>s.</param>
		/// <param name="offset">The offset.</param>
		partial void PaintCustomViewIfNeed(IList<Shape> controls, double offset)
		{
			if (CustomView is not null)
			{
				PaintViewIfNeedInternal(controls, offset, CustomView);
			}
		}
	}
}
