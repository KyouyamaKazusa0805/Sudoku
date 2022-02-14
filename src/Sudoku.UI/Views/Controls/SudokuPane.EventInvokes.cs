using Microsoft.UI;
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
	private void SudokuPane_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		// Initializes the grid lines.
		initializeGrid(1, 4, 1);


		void initializeGrid(double outsideBorderThickness, double blockBorderThickness, double cellBorderThickness)
		{
			var duration = TimeSpan.FromSeconds(1);
			PointCalculator = new(Size, OutsideOffset);

			// Initializes the outside border if worth.
			if (outsideBorderThickness != 0 && OutsideOffset != 0)
			{
				var rect = DrawingElementCreator.OutsideRectangle(
					Colors.Black,
					outsideBorderThickness,
					duration
				);

				_cCanvasMain.Children.Add(rect);
			}

			// Initializes block border lines.
			for (int i = 0; i < 4; i++)
			{
				var l1 = DrawingElementCreator.BlockBorderLine(
					Colors.Black,
					blockBorderThickness,
					duration,
					PointCalculator,
					i,
					false
				);
				var l2 = DrawingElementCreator.BlockBorderLine(
					Colors.Black,
					blockBorderThickness,
					duration,
					PointCalculator,
					i,
					true
				);

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

				var l1 = DrawingElementCreator.CellBorderLine(
					Colors.Black,
					cellBorderThickness,
					duration,
					PointCalculator,
					i,
					false
				);
				var l2 = DrawingElementCreator.CellBorderLine(
					Colors.Black,
					cellBorderThickness,
					duration,
					PointCalculator,
					i,
					true
				);

				_cCanvasMain.Children.Add(l1);
				_cCanvasMain.Children.Add(l2);
			}

			// TODO: Initializes candidate border lines if worth.
		}
	}
}
