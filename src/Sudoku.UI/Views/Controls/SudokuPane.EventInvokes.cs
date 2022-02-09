using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Drawing;

namespace Sudoku.UI.Views.Controls;

partial class SudokuPane
{
	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SudokuPane_LoadedAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		// Initializes the grid lines.
		initializeGrid(outsideBorderThickness: 1, blockBorderThickness: 4, cellBorderThickness: 1);

		// Then apply the animation for the initialization.
		await setBorderLineScaleAsync();


		void initializeGrid(
			double outsideBorderThickness, double blockBorderThickness, double cellBorderThickness)
		{
			PointCalculator = new(Size, OutsideOffset);

			var defaultScale = Vector3.Zero;
			var borderBrush = new SolidColorBrush(Colors.Black);
			var scaleTransition = new Vector3Transition { Duration = TimeSpan.FromSeconds(1) };

			// Initializes the outside border if worth.
			if (outsideBorderThickness != 0 && OutsideOffset != 0)
			{
				var rect = new Rectangle
				{
					Stroke = borderBrush,
					StrokeThickness = outsideBorderThickness,
					Scale = defaultScale,
					ScaleTransition = scaleTransition,
					Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.OutsideBorderLines}"
				};
				Canvas.SetZIndex(rect, 0);

				_cCanvasMain.Children.Add(rect);
			}

			// Initializes block border lines.
			for (int i = 0; i < 4; i++)
			{
				var l1 = new Line
				{
					Stroke = borderBrush,
					StrokeThickness = blockBorderThickness,
					X1 = PointCalculator.HorizontalBorderLinePoint1(i, BorderLineType.Block).X,
					Y1 = PointCalculator.HorizontalBorderLinePoint1(i, BorderLineType.Block).Y,
					X2 = PointCalculator.HorizontalBorderLinePoint2(i, BorderLineType.Block).X,
					Y2 = PointCalculator.HorizontalBorderLinePoint2(i, BorderLineType.Block).Y,
					Scale = defaultScale,
					ScaleTransition = scaleTransition,
					StrokeLineJoin = PenLineJoin.Round,
					Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.BlockBorderLines}|{SudokuCanvasTags.HorizontalBorderLines}|{i}"
				};
				var l2 = new Line
				{
					Stroke = borderBrush,
					StrokeThickness = blockBorderThickness,
					X1 = PointCalculator.VerticalBorderLinePoint1(i, BorderLineType.Block).X,
					Y1 = PointCalculator.VerticalBorderLinePoint1(i, BorderLineType.Block).Y,
					X2 = PointCalculator.VerticalBorderLinePoint2(i, BorderLineType.Block).X,
					Y2 = PointCalculator.VerticalBorderLinePoint2(i, BorderLineType.Block).Y,
					Scale = defaultScale,
					ScaleTransition = scaleTransition,
					StrokeLineJoin = PenLineJoin.Round,
					Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.BlockBorderLines}|{SudokuCanvasTags.VerticalBorderLines}|{i}"
				};
				Canvas.SetZIndex(l1, 1);
				Canvas.SetZIndex(l2, 1);

				_cCanvasMain.Children.Add(l1);
				_cCanvasMain.Children.Add(l2);
			}

			// Initializes cell border lines.
			for (int i = 0; i < 10; i++)
			{
				if (i is 0 or 3 or 6 or 9)
				{
					// Skip overlapping lines.
					continue;
				}

				var l1 = new Line
				{
					Stroke = borderBrush,
					StrokeThickness = cellBorderThickness,
					X1 = PointCalculator.HorizontalBorderLinePoint1(i, BorderLineType.Cell).X,
					Y1 = PointCalculator.HorizontalBorderLinePoint1(i, BorderLineType.Cell).Y,
					X2 = PointCalculator.HorizontalBorderLinePoint2(i, BorderLineType.Cell).X,
					Y2 = PointCalculator.HorizontalBorderLinePoint2(i, BorderLineType.Cell).Y,
					Scale = defaultScale,
					ScaleTransition = scaleTransition,
					StrokeLineJoin = PenLineJoin.Round,
					Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.CellBorderLines}|{SudokuCanvasTags.HorizontalBorderLines}|{i}"
				};
				var l2 = new Line
				{
					Stroke = borderBrush,
					StrokeThickness = cellBorderThickness,
					X1 = PointCalculator.VerticalBorderLinePoint1(i, BorderLineType.Cell).X,
					Y1 = PointCalculator.VerticalBorderLinePoint1(i, BorderLineType.Cell).Y,
					X2 = PointCalculator.VerticalBorderLinePoint2(i, BorderLineType.Cell).X,
					Y2 = PointCalculator.VerticalBorderLinePoint2(i, BorderLineType.Cell).Y,
					Scale = defaultScale,
					ScaleTransition = scaleTransition,
					StrokeLineJoin = PenLineJoin.Round,
					Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.CellBorderLines}|{SudokuCanvasTags.VerticalBorderLines}|{i}"
				};
				Canvas.SetZIndex(l1, 1);
				Canvas.SetZIndex(l2, 1);

				_cCanvasMain.Children.Add(l1);
				_cCanvasMain.Children.Add(l2);
			}

			// TODO: Initializes candidate border lines if worth.
		}

		async Task setBorderLineScaleAsync()
		{
			foreach (var control in
				from control in _cCanvasMain.Children.OfType<FrameworkElement>()
				where control.Tag is string s && s.Contains(SudokuCanvasTags.BorderLines)
				select control)
			{
				string tag = (string)control.Tag!;

				// Try to change the scale to trigger the animation.
				control.Scale = new(1, 1, 1);
				await Task.Delay(100);
			}
		}
	}
}
