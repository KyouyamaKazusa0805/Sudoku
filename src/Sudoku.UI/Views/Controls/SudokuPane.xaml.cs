using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Tags = Sudoku.UI.SudokuCanvasTags;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	/// <summary>
	/// Defines the property that handles the outside offset <see cref="OutsideOffset"/>.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	public static readonly DependencyProperty OutsideOffsetProperty =
		DependencyProperty.Register(nameof(OutsideOffset), typeof(double), typeof(SudokuPane), new(0));


	/// <summary>
	/// Indicates the outside offset.
	/// </summary>
	/// <value>The value to assign.</value>
	/// <returns>The outside offset value.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the value is below 0.</exception>
	public double OutsideOffset
	{
		get => (double)GetValue(OutsideOffsetProperty);

		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			SetValue(OutsideOffsetProperty, value);

			OutsideOffsetChanged?.Invoke(this, new());
		}
	}


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	public SudokuPane()
	{
		InitializeComponent();

		OutsideOffsetChanged += SudokuPane_OutsideOffsetChanged;
	}


	/// <summary>
	/// Defines an event that triggers when the property value <see cref="OutsideOffset"/> is changed.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	public event RoutedEventHandler? OutsideOffsetChanged;


	/// <summary>
	/// Initializes the grid and updates the controls.
	/// </summary>
	/// <param name="canvas">The control where the items will be drawn onto.</param>
	/// <param name="outsideBorderThickness">The stroke of the outside border.</param>
	/// <param name="blockBorderThickness">The stroke of the block border.</param>
	/// <param name="cellBorderThickness">The stroke of the cell border.</param>
	private void InitializeGrid(
		Canvas canvas, double outsideBorderThickness, double blockBorderThickness, double cellBorderThickness)
	{
		var defaultScale = Vector3.Zero;
		var borderBrush = new SolidColorBrush(Colors.Black);
		var scaleTransition = new Vector3Transition() { Duration = TimeSpan.FromSeconds(1) };

		// Initializes the outside border if worth.
		if (outsideBorderThickness != 0 && OutsideOffset != 0)
		{
			var rect = new Rectangle
			{
				Stroke = borderBrush,
				StrokeThickness = outsideBorderThickness,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				Tag = $"{Tags.BorderLines}|{Tags.OutsideBorderLines}"
			};
			Canvas.SetZIndex(rect, 0);

			canvas.Children.Add(rect);
		}

		// Initializes block border lines.
		for (int i = 0; i < 4; i++)
		{
			var l1 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = blockBorderThickness,
				X1 = HorizontalBorderLinePoint1(canvas, i, 3).X,
				Y1 = HorizontalBorderLinePoint1(canvas, i, 3).Y,
				X2 = HorizontalBorderLinePoint2(canvas, i, 3).X,
				Y2 = HorizontalBorderLinePoint2(canvas, i, 3).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.BlockBorderLines}|{Tags.HorizontalBorderLines}"
			};
			var l2 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = blockBorderThickness,
				X1 = VerticalBorderLinePoint1(canvas, i, 3).X,
				Y1 = VerticalBorderLinePoint1(canvas, i, 3).Y,
				X2 = VerticalBorderLinePoint2(canvas, i, 3).X,
				Y2 = VerticalBorderLinePoint2(canvas, i, 3).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.BlockBorderLines}|{Tags.VerticalBorderLines}"
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
				X1 = HorizontalBorderLinePoint1(canvas, i, 9).X,
				Y1 = HorizontalBorderLinePoint1(canvas, i, 9).Y,
				X2 = HorizontalBorderLinePoint2(canvas, i, 9).X,
				Y2 = HorizontalBorderLinePoint2(canvas, i, 9).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.CellBorderLines}|{Tags.HorizontalBorderLines}"
			};
			var l2 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = cellBorderThickness,
				X1 = VerticalBorderLinePoint1(canvas, i, 9).X,
				Y1 = VerticalBorderLinePoint1(canvas, i, 9).Y,
				X2 = VerticalBorderLinePoint2(canvas, i, 9).X,
				Y2 = VerticalBorderLinePoint2(canvas, i, 9).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.CellBorderLines}|{Tags.VerticalBorderLines}"
			};
			Canvas.SetZIndex(l1, 1);
			Canvas.SetZIndex(l2, 1);

			canvas.Children.Add(l1);
			canvas.Children.Add(l2);
		}

		// TODO: Initializes candidate border lines if worth.
	}

	/// <summary>
	/// Gets the first point value of the horizontal border line.
	/// </summary>
	/// <param name="canvas">The <see cref="Canvas"/> instance.</param>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The first point value of the horizontal border line.</returns>
	private Point HorizontalBorderLinePoint1(Canvas canvas, int i, int weight) =>
		new()
		{
			X = OutsideOffset + i * (canvas.ActualHeight - 2 * OutsideOffset) / weight,
			Y = OutsideOffset
		};

	/// <summary>
	/// Gets the second point value of the horizontal border line.
	/// </summary>
	/// <param name="canvas">The <see cref="Canvas"/> instance.</param>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The second point value of the horizontal border line.</returns>
	private Point HorizontalBorderLinePoint2(Canvas canvas, int i, int weight) =>
		new()
		{
			X = OutsideOffset + i * (canvas.ActualHeight - 2 * OutsideOffset) / weight,
			Y = canvas.ActualWidth - OutsideOffset
		};

	/// <summary>
	/// Gets the first point value of the vertical border line.
	/// </summary>
	/// <param name="canvas">The <see cref="Canvas"/> instance.</param>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The first point value of the horizontal border line.</returns>
	private Point VerticalBorderLinePoint1(Canvas canvas, int i, int weight) =>
		new()
		{
			X = OutsideOffset,
			Y = OutsideOffset + i * (canvas.ActualWidth - 2 * OutsideOffset) / weight
		};

	/// <summary>
	/// Gets the second point value of the vertical border line.
	/// </summary>
	/// <param name="canvas">The <see cref="Canvas"/> instance.</param>
	/// <param name="i">The index of the line.</param>
	/// <param name="weight">The weight of the division operation. The value can only be 3, 9 or 27.</param>
	/// <returns>The second point value of the horizontal border line.</returns>
	private Point VerticalBorderLinePoint2(Canvas canvas, int i, int weight) =>
		new()
		{
			X = canvas.ActualHeight - OutsideOffset,
			Y = OutsideOffset + i * (canvas.ActualWidth - 2 * OutsideOffset) / weight
		};

	/// <summary>
	/// Applies the loaded animation.
	/// </summary>
	/// <returns>The task that loads the animation on grid border lines.</returns>
	private async Task ApplyAnimationAsync()
	{
		foreach (var control in
			from control in _cCanvasMain.Children.OfType<FrameworkElement>()
			where control.Tag is string s && s.Contains(Tags.BorderLines)
			select control)
		{
			control.Scale = new(1, 1, 1); // Try to change the scale to trigger the animation.
			await Task.Delay(100);
		}
	}

	/// <summary>
	/// Triggers when the property <see cref="OutsideOffset"/> is changed.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_OutsideOffsetChanged([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		foreach (var control in
			from control in _cCanvasMain.Children.OfType<Line>()
			where control.Tag is string s && s.Contains(Tags.BorderLines)
			select control)
		{
			string tag = (string)control.Tag!;
			if (tag.Contains(Tags.HorizontalBorderLines))
			{
				int weight = tag.Contains(Tags.BlockBorderLines) ? 3 : 9;
				for (int i = 0; i < (weight == 3 ? 4 : 10); i++)
				{
					var (x1, y1) = HorizontalBorderLinePoint1(_cCanvasMain, i, weight);
					control.X1 = x1;
					control.Y1 = y1;

					var (x2, y2) = HorizontalBorderLinePoint2(_cCanvasMain, i, weight);
					control.X2 = x2;
					control.Y2 = y2;
				}
			}
			else if (tag.Contains(Tags.VerticalBorderLines))
			{
				int weight = tag.Contains(Tags.BlockBorderLines) ? 3 : 9;
				for (int i = 0; i < (weight == 3 ? 4 : 10); i++)
				{
					var (x1, y1) = VerticalBorderLinePoint1(_cCanvasMain, i, weight);
					control.X1 = x1;
					control.Y1 = y1;

					var (x2, y2) = VerticalBorderLinePoint2(_cCanvasMain, i, weight);
					control.X2 = x2;
					control.Y2 = y2;
				}
			}
		}
	}

	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SudokuPane_LoadedAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		// Initializes the grid lines.
		InitializeGrid(_cCanvasMain, 1, 4, 1);

		// Then apply the animation for the initialization.
		await ApplyAnimationAsync();
	}
}
