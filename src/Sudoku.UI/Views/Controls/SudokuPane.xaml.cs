using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane() => InitializeComponent();


	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SudokuPane_LoadedAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		InitializeGrid(_cCanvasMain, 1, 4, 1);
		await applyAnimationAsync();


		async Task applyAnimationAsync()
		{
			foreach (var control in
				from control in _cCanvasMain.Children.OfType<FrameworkElement>()
				where control.Tag is "Border lines"
				select control)
			{
				control.Scale = new(1, 1, 1);
				await Task.Delay(100);
			}
		}
	}


	/// <summary>
	/// Initializes the grid and updates the controls.
	/// </summary>
	/// <param name="canvas">The control where the items will be drawn onto.</param>
	/// <param name="outsideBorderThickness">The stroke of the outside border.</param>
	/// <param name="blockBorderThickness">The stroke of the block border.</param>
	/// <param name="cellBorderThickness">The stroke of the cell border.</param>
	private static void InitializeGrid(
		Canvas canvas, double outsideBorderThickness, double blockBorderThickness, double cellBorderThickness)
	{
		const double offset = 10;
		var borderBrush = new SolidColorBrush(Colors.Black);
		var scaleTransition = new Vector3Transition() { Duration = TimeSpan.FromSeconds(1) };

		// Initializes the outside border if worth.
		if (outsideBorderThickness != 0)
		{
			var rect = new Rectangle
			{
				Stroke = borderBrush,
				StrokeThickness = outsideBorderThickness,
				Scale = new(),
				ScaleTransition = scaleTransition,
				Tag = "Border lines"
			};
			Canvas.SetZIndex(rect, 0);

			canvas.Children.Add(rect);
		}

		// Initializes block border lines.
		double gridWidth = canvas.ActualWidth - 2 * offset;
		double gridHeight = canvas.ActualHeight - 2 * offset;
		for (int i = 0; i < 4; i++)
		{
			var l1 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = blockBorderThickness,
				X1 = offset + i * gridHeight / 3,
				Y1 = offset,
				X2 = offset + i * gridHeight / 3,
				Y2 = canvas.ActualWidth - offset,
				Scale = new(),
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = "Border lines"
			};
			var l2 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = blockBorderThickness,
				X1 = offset,
				Y1 = offset + i * gridWidth / 3,
				X2 = canvas.ActualHeight - offset,
				Y2 = offset + i * gridWidth / 3,
				Scale = new(),
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = "Border lines"
			};
			Canvas.SetZIndex(l1, 1);
			Canvas.SetZIndex(l2, 1);

			canvas.Children.Add(l1);
			canvas.Children.Add(l2);
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
				X1 = offset + i * gridHeight / 9,
				Y1 = offset,
				X2 = offset + i * gridHeight / 9,
				Y2 = canvas.ActualWidth - offset,
				Scale = new(),
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = "Border lines"
			};
			var l2 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = cellBorderThickness,
				X1 = offset,
				Y1 = offset + i * gridWidth / 9,
				X2 = canvas.ActualHeight - offset,
				Y2 = offset + i * gridWidth / 9,
				Scale = new(),
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = "Border lines"
			};
			Canvas.SetZIndex(l1, 1);
			Canvas.SetZIndex(l2, 1);

			canvas.Children.Add(l1);
			canvas.Children.Add(l2);
		}
	}
}
