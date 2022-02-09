using Microsoft.UI.Xaml.Shapes;
using Sudoku.UI.Drawing;
using Windows.Foundation;

namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that interacts with a sudoku grid.
/// </summary>
public sealed partial class SudokuPane : UserControl
{
	private double _size;
	private double _outsideOffset;
	private Grid _grid;


	/// <summary>
	/// Initializes a <see cref="SudokuPane"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPane() => InitializeComponent();


	/// <summary>
	/// Gets or sets the size of the pane to the view model.
	/// </summary>
	public double Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _size;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (!_size.NearlyEquals(value, PointCalculator.Epsilon))
			{
				_size = value;

				UpdateBorderLines();
			}
		}
	}

	/// <summary>
	/// Gets or sets the outside offset to the view model.
	/// </summary>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (!_outsideOffset.NearlyEquals(value, PointCalculator.Epsilon))
			{
				_outsideOffset = value;

				UpdateBorderLines();
			}
		}
	}

	/// <summary>
	/// Gets or sets the current grid used.
	/// </summary>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_grid != value)
			{
				_grid = value;

				UpdateGrid();
			}
		}
	}

	/// <summary>
	/// Indicates the point calculator.
	/// </summary>
	public PointCalculator PointCalculator { get; private set; }


	/// <summary>
	/// Update the grid info.
	/// </summary>
	private void UpdateGrid()
	{
	}

	/// <summary>
	/// Update the border lines.
	/// </summary>
	private void UpdateBorderLines()
	{
		PointCalculator = new(Size, OutsideOffset);

		foreach (var control in
			from control in _cCanvasMain.Children.OfType<Line>()
			where control.Tag is string s && s.Contains(SudokuCanvasTags.BorderLines)
			select control)
		{
			string tag = (string)control.Tag!;
			int i = int.Parse(tag.Split('|')[^1]);
			var weight = tag.Contains(SudokuCanvasTags.BlockBorderLines) ? BorderLineType.Block : BorderLineType.Cell;
			if (tag.Contains(SudokuCanvasTags.HorizontalBorderLines))
			{
				var (x1, y1) = PointCalculator.HorizontalBorderLinePoint1(i, weight);
				control.X1 = x1;
				control.Y1 = y1;

				var (x2, y2) = PointCalculator.HorizontalBorderLinePoint2(i, weight);
				control.X2 = x2;
				control.Y2 = y2;
			}
			else if (tag.Contains(SudokuCanvasTags.VerticalBorderLines))
			{
				var (x1, y1) = PointCalculator.VerticalBorderLinePoint1(i, weight);
				control.X1 = x1;
				control.Y1 = y1;

				var (x2, y2) = PointCalculator.VerticalBorderLinePoint2(i, weight);
				control.X2 = x2;
				control.Y2 = y2;
			}
		}
	}
}
