using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines a creator that creates the drawing elements.
/// </summary>
internal static class DrawingElementCreator
{
	/// <summary>
	/// Creates a <see cref="Rectangle"/> instance that displays as the outside border.
	/// </summary>
	/// <param name="color">The color of the stroke.</param>
	/// <param name="thickness">The thickness of the border.</param>
	/// <param name="duration">The duration of the scale transition.</param>
	/// <returns>The instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle OutsideRectangle(Color color, double thickness, TimeSpan duration) =>
		new()
		{
			Stroke = new SolidColorBrush(color),
			StrokeThickness = thickness,
			Scale = Vector3.Zero,
			ScaleTransition = new() { Duration = duration },
			Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.OutsideBorderLines}"
		};

	/// <summary>
	/// Creates a <see cref="Line"/> instance that displays as the block border lines.
	/// </summary>
	/// <param name="color">The color of the stroke.</param>
	/// <param name="thickness">The thickness of the border.</param>
	/// <param name="duration">The duration of the scale transition.</param>
	/// <param name="pc">The conversion instance.</param>
	/// <param name="order">The order of the line. The valid range is <c>[0, 4)</c>.</param>
	/// <param name="horizontal">
	/// Indicates whether the current mode is creating a horizontal line. <see langword="true"/> is for the
	/// horizontal lines and <see langword="false"/> is for vertical lines.
	/// </param>
	/// <returns>The instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line BlockBorderLine(
		Color color, double thickness, TimeSpan duration, PointCalculator pc, int order, bool horizontal)
	{
		Func<int, BorderLineType, Point>
			a = horizontal ? pc.HorizontalBorderLinePoint1 : pc.VerticalBorderLinePoint1,
			b = horizontal ? pc.HorizontalBorderLinePoint2 : pc.VerticalBorderLinePoint2;

		var (x1, y1) = a(order, BorderLineType.Block);
		var (x2, y2) = b(order, BorderLineType.Block);
		return new()
		{
			Stroke = new SolidColorBrush(color),
			StrokeThickness = thickness,
			X1 = x1,
			Y1 = y1,
			X2 = x2,
			Y2 = y2,
			Scale = Vector3.Zero,
			ScaleTransition = new() { Duration = duration },
			StrokeLineJoin = PenLineJoin.Round,
			Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.BlockBorderLines}|{(horizontal ? SudokuCanvasTags.HorizontalBorderLines : SudokuCanvasTags.VerticalBorderLines)}|{order}"
		};
	}

	/// <summary>
	/// Creates a <see cref="Line"/> instance that displays as the cell border lines.
	/// </summary>
	/// <param name="color">The color of the stroke.</param>
	/// <param name="thickness">The thickness of the border.</param>
	/// <param name="duration">The duration of the scale transition.</param>
	/// <param name="pc">The conversion instance.</param>
	/// <param name="order">The order of the line. The valid range is <c>[0, 4)</c>.</param>
	/// <param name="horizontal">
	/// Indicates whether the current mode is creating a horizontal line. <see langword="true"/> is for the
	/// horizontal lines and <see langword="false"/> is for vertical lines.
	/// </param>
	/// <returns>The instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Line CellBorderLine(
		Color color, double thickness, TimeSpan duration, PointCalculator pc, int order, bool horizontal)
	{
		Func<int, BorderLineType, Point>
			a = horizontal ? pc.HorizontalBorderLinePoint1 : pc.VerticalBorderLinePoint1,
			b = horizontal ? pc.HorizontalBorderLinePoint2 : pc.VerticalBorderLinePoint2;

		var (x1, y1) = a(order, BorderLineType.Cell);
		var (x2, y2) = b(order, BorderLineType.Cell);
		return new()
		{
			Stroke = new SolidColorBrush(color),
			StrokeThickness = thickness,
			X1 = x1,
			Y1 = y1,
			X2 = x2,
			Y2 = y2,
			Scale = Vector3.Zero,
			ScaleTransition = new() { Duration = duration },
			StrokeLineJoin = PenLineJoin.Round,
			Tag = $"{SudokuCanvasTags.BorderLines}|{SudokuCanvasTags.CellBorderLines}|{(horizontal ? SudokuCanvasTags.HorizontalBorderLines : SudokuCanvasTags.VerticalBorderLines)}|{order}"
		};
	}
}
