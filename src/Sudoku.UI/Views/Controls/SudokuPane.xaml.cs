using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Models;
using Sudoku.UI.ViewModels;
using Windows.Foundation;
using Tags = Sudoku.UI.SudokuCanvasTags;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPane() => InitializeComponent();


	/// <summary>
	/// Gets or sets the size of the pane to the view model.
	/// </summary>
	/// <value>The size of the pane.</value>
	public double Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _vm.Size;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _vm.Size = value;
	}

	/// <summary>
	/// Gets or sets the outside offset to the view model.
	/// </summary>
	/// <value>The outside offset.</value>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _vm.OutsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _vm.OutsideOffset = value;
	}


	/// <summary>
	/// Initializes the grid and updates the controls.
	/// </summary>
	/// <param name="outsideBorderThickness">The stroke of the outside border.</param>
	/// <param name="blockBorderThickness">The stroke of the block border.</param>
	/// <param name="cellBorderThickness">The stroke of the cell border.</param>
	private void InitializeGrid(
		double outsideBorderThickness, double blockBorderThickness, double cellBorderThickness)
	{
		var defaultScale = Vector3.Zero;
		var borderBrush = new SolidColorBrush(Colors.Black);
		var scaleTransition = new Vector3Transition { Duration = TimeSpan.FromSeconds(1) };

		// Initializes the outside border if worth.
		if (outsideBorderThickness != 0 && _vm.OutsideOffset != 0)
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

			_cCanvasMain.Children.Add(rect);
		}

		// Initializes block border lines.
		for (int i = 0; i < 4; i++)
		{
			var l1 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = blockBorderThickness,
				X1 = _vm.HorizontalBorderLinePoint1(i, 3).X,
				Y1 = _vm.HorizontalBorderLinePoint1(i, 3).Y,
				X2 = _vm.HorizontalBorderLinePoint2(i, 3).X,
				Y2 = _vm.HorizontalBorderLinePoint2(i, 3).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.BlockBorderLines}|{Tags.HorizontalBorderLines}|{i}"
			};
			var l2 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = blockBorderThickness,
				X1 = _vm.VerticalBorderLinePoint1(i, 3).X,
				Y1 = _vm.VerticalBorderLinePoint1(i, 3).Y,
				X2 = _vm.VerticalBorderLinePoint2(i, 3).X,
				Y2 = _vm.VerticalBorderLinePoint2(i, 3).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.BlockBorderLines}|{Tags.VerticalBorderLines}|{i}"
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
				X1 = _vm.HorizontalBorderLinePoint1(i, 9).X,
				Y1 = _vm.HorizontalBorderLinePoint1(i, 9).Y,
				X2 = _vm.HorizontalBorderLinePoint2(i, 9).X,
				Y2 = _vm.HorizontalBorderLinePoint2(i, 9).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.CellBorderLines}|{Tags.HorizontalBorderLines}|{i}"
			};
			var l2 = new Line
			{
				Stroke = borderBrush,
				StrokeThickness = cellBorderThickness,
				X1 = _vm.VerticalBorderLinePoint1(i, 9).X,
				Y1 = _vm.VerticalBorderLinePoint1(i, 9).Y,
				X2 = _vm.VerticalBorderLinePoint2(i, 9).X,
				Y2 = _vm.VerticalBorderLinePoint2(i, 9).Y,
				Scale = defaultScale,
				ScaleTransition = scaleTransition,
				StrokeLineJoin = PenLineJoin.Round,
				Tag = $"{Tags.BorderLines}|{Tags.CellBorderLines}|{Tags.VerticalBorderLines}|{i}"
			};
			Canvas.SetZIndex(l1, 1);
			Canvas.SetZIndex(l2, 1);

			_cCanvasMain.Children.Add(l1);
			_cCanvasMain.Children.Add(l2);
		}

		// TODO: Initializes candidate border lines if worth.
	}

	/// <summary>
	/// Update the border lines.
	/// </summary>
	private void UpdateBorderLines()
	{
		foreach (var control in
			from control in _cCanvasMain.Children.OfType<Line>()
			where control.Tag is string s && s.Contains(Tags.BorderLines)
			select control)
		{
			string tag = (string)control.Tag!;
			int i = int.Parse(tag.Split('|')[^1]);
			int weight = tag.Contains(Tags.BlockBorderLines) ? 3 : 9;
			if (tag.Contains(Tags.HorizontalBorderLines))
			{
				var (x1, y1) = _vm.HorizontalBorderLinePoint1(i, weight);
				control.X1 = x1;
				control.Y1 = y1;

				var (x2, y2) = _vm.HorizontalBorderLinePoint2(i, weight);
				control.X2 = x2;
				control.Y2 = y2;
			}
			else if (tag.Contains(Tags.VerticalBorderLines))
			{
				var (x1, y1) = _vm.VerticalBorderLinePoint1(i, weight);
				control.X1 = x1;
				control.Y1 = y1;

				var (x2, y2) = _vm.VerticalBorderLinePoint2(i, weight);
				control.X2 = x2;
				control.Y2 = y2;
			}
		}
	}

	/// <summary>
	/// Sets the scale of all border lines.
	/// </summary>
	/// <param name="horizontalScale">The horizontal scale.</param>
	/// <param name="verticalScale">The vertical scale.</param>
	/// <returns>The task that loads the animation on grid border lines.</returns>
	private async Task SetBorderLineScaleAsync(Vector3 horizontalScale, Vector3 verticalScale)
	{
		foreach (var control in
			from control in _cCanvasMain.Children.OfType<FrameworkElement>()
			where control.Tag is string s && s.Contains(Tags.BorderLines)
			select control)
		{
			string tag = (string)control.Tag!;

			// Try to change the scale to trigger the animation.
			control.Scale = tag.Contains(Tags.HorizontalBorderLines) ? horizontalScale : verticalScale;
			await Task.Delay(100);
		}
	}

	/// <summary>
	/// Triggers when the property <see cref="SudokuPaneViewModel.Grid"/> is changed.
	/// </summary>
	/// <param name="sender">The object to trigegr the event.</param>
	/// <param name="e">The event arguments provided.</param>
	/// <seealso cref="SudokuPaneViewModel.Grid"/>
	private void SudokuPane_GridChanged(object sender, GridChangedEventArgs e)
	{

	}

	/// <summary>
	/// Triggers when the property <see cref="OutsideOffset"/> is changed.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_OutsideOffsetChanged([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		UpdateBorderLines();

	/// <summary>
	/// Triggers when the property <see cref="Size"/> is changed.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_SizeChanged([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		UpdateBorderLines();

	/// <summary>
	/// Triggers when the current control is loaded.
	/// </summary>
	/// <param name="sender">The object to trigger the event. The instance is always itself.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SudokuPane_LoadedAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		// Initializes the grid lines.
		InitializeGrid(outsideBorderThickness: 1, blockBorderThickness: 4, cellBorderThickness: 1);

		// Then apply the animation for the initialization.
		await SetBorderLineScaleAsync(new(1, 1, 1), new(1, 1, 1));
	}
}
