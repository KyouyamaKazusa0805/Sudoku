namespace Sudoku.UI.Drawing;

/// <summary>
/// Provides a marshal of methods that called while image generating in <see cref="GridImageGenerator"/>.
/// </summary>
/// <seealso cref="GridImageGenerator"/>
internal static class GridImageGenerating
{
	/// <summary>
	/// Append a <see cref="TextBlock"/> control into the <see cref="Grid"/> collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="row">The row that the control is added to.</param>
	/// <param name="column">The column that the control is added to.</param>
	/// <param name="digit">
	/// <para>The digit that the <see cref="TextBlock"/> instance displays.</para>
	/// <para>
	/// Please note that the value will be between 0 and 8, and the method will output
	/// the value and project the range to be between 1 and 9 automatically, so you should pass
	/// a value that is between 0 and 8.
	/// </para>
	/// </param>
	/// <param name="foreground">
	/// The <see cref="Brush"/> instance that represents the foreground of the text
	/// whose value is from argument <paramref name="digit"/>.
	/// </param>
	/// <param name="font">The <see cref="FontFamily"/> instance.</param>
	/// <param name="fontSize">
	/// Indicates the size of the text whose value is from argument <paramref name="digit"/>.
	/// </param>
	/// <param name="horizontalAlignment">
	/// Indicates the alignment that constraints for the row.
	/// The default value is <see cref="HorizontalAlignment.Stretch"/>.
	/// </param>
	/// <param name="verticalAlignment">
	/// Indicates the alignment that constraints for the column.
	/// The default value is <see cref="VerticalAlignment.Stretch"/>.
	/// </param>
	/// <seealso cref="TextBlock"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="Brush"/>
	/// <seealso cref="FontFamily"/>
	/// <seealso cref="HorizontalAlignment"/>
	/// <seealso cref="VerticalAlignment"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTextBlock(
		this Grid @this, int row, int column, int digit, Brush foreground, FontFamily font, double fontSize,
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Stretch,
		VerticalAlignment verticalAlignment = VerticalAlignment.Stretch)
	{
		var tb = new TextBlock
		{
			HorizontalAlignment = horizontalAlignment,
			VerticalAlignment = verticalAlignment,
			Foreground = foreground,
			Text = (digit + 1).ToString(),
			FontFamily = font,
			FontSize = fontSize
		};

		Grid.SetRow(tb, row);
		Grid.SetColumn(tb, column);

		@this.Children.Add(tb);
	}

	/// <summary>
	/// Append the background color that effects the whole <see cref="Canvas"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="fill">The color to be filled.</param>
	/// <param name="pointCalculator">The calculator that can calculated the coordinates.</param>
	/// <seealso cref="Canvas"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="PointCalculator"/>
	public static void AddCanvasBackground(this Grid @this, in Color fill, PointCalculator pointCalculator)
	{
		var brush = new SolidColorBrush(fill);
		var (x, y) = pointCalculator.ControlSize;

		var rectangle = new Rectangle { RadiusX = x / 2, RadiusY = y / 2, Fill = brush };

		Grid.SetRow(rectangle, 0);
		Grid.SetRowSpan(rectangle, PointCalculator.AnchorsCount);
		Grid.SetColumn(rectangle, 0);
		Grid.SetColumnSpan(rectangle, PointCalculator.AnchorsCount);

		@this.Children.Add(rectangle);
	}

	/// <summary>
	/// Add a <see cref="Rectangle"/> instance as a cell, candidate or a region into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="row">The row that the control is added to.</param>
	/// <param name="column">The column that the control is added to.</param>
	/// <param name="rowSpan">Indicates the row span.</param>
	/// <param name="columnSpan">Indicates the column span.</param>
	/// <param name="fill">The color that the <see cref="Rectangle"/> instance filled.</param>
	/// <param name="radiusX">
	/// Indicates the radius X value that used on rounded rectangle. The default value is 0.
	/// </param>
	/// <param name="radiusY">
	/// Indicates the radius Y value that used on rounded rectangle. The default value is 0.
	/// </param>
	/// <seealso cref="Rectangle"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="PointCalculator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRectangle(
		this Grid @this, int row, int column, int rowSpan, int columnSpan, Brush fill,
		double radiusX = 0, double radiusY = 0)
	{
		var rectangle = new Rectangle
		{
			RadiusX = radiusX,
			RadiusY = radiusY,
			Fill = fill
		};

		Grid.SetRow(rectangle, row);
		Grid.SetRowSpan(rectangle, rowSpan);
		Grid.SetColumn(rectangle, column);
		Grid.SetColumnSpan(rectangle, columnSpan);

		@this.Children.Add(rectangle);
	}

	/// <summary>
	/// Add a candidate circle <see cref="Ellipse"/> instance into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="row">The row that the control is added to.</param>
	/// <param name="column">The column that the control is added to.</param>
	/// <param name="size">Indicates the size of the circle.</param>
	/// <param name="fill">Indicates the <see cref="Brush"/> instance as the filling.</param>
	/// <param name="stroke">Indicates the <see cref="Brush"/> instance as the stroke.</param>
	/// <param name="strokeSize">Indictaes the stroke size. The default value is <see cref="double.NaN"/>.</param>
	/// <seealso cref="Ellipse"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="Brush"/>
	/// <exception cref="ArgumentException">
	/// Throws when <paramref name="strokeSize"/> is invalid (<paramref name="stroke"/> isn't
	/// <see langword="null"/> but <paramref name="strokeSize"/> is <see cref="double.NaN"/>);
	/// or <paramref name="stroke"/> is <see langword="null"/> but <paramref name="strokeSize"/>
	/// is <see cref="double.NaN"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddCircle(
		this Grid @this, int row, int column, double size, Brush fill, Brush? stroke = null,
		double strokeSize = double.NaN)
	{
		switch ((Stroke: stroke, StrokeSize: strokeSize))
		{
			case (Stroke: not null, StrokeSize: not double.NaN):
			{
				var circle = new Ellipse
				{
					Width = size,
					Height = size,
					Fill = fill,
					Stroke = stroke,
					StrokeThickness = strokeSize
				};

				Grid.SetRow(circle, row);
				Grid.SetColumn(circle, column);

				@this.Children.Add(circle);

				break;
			}
			case (Stroke: null, StrokeSize: double.NaN):
			{
				var circle = new Ellipse { Width = size, Height = size, Fill = fill };

				Grid.SetRow(circle, row);
				Grid.SetColumn(circle, column);

				@this.Children.Add(circle);

				break;
			}
			default:
			{
				throw new ArgumentException("The specified argument is invalid.", nameof(strokeSize));
			}
		}
	}

	/// <summary>
	/// Add a <see cref="Line"/> instance into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="x1">The X1 value of the line.</param>
	/// <param name="y1">The Y1 value of the line.</param>
	/// <param name="x2">The X2 value of the line.</param>
	/// <param name="y2">The Y2 value of the line.</param>
	/// <param name="stroke">The stroke of the line.</param>
	/// <param name="strokeSize">The stroke thickness of the line.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddLine(
		this Grid @this, double x1, double y1, double x2, double y2, Brush stroke, double strokeSize)
	{
		var line = new Line
		{
			X1 = x1,
			X2 = x2,
			Y1 = y1,
			Y2 = y2,
			Stroke = stroke,
			StrokeThickness = strokeSize
		};

		@this.Children.Add(line);
	}

	/// <summary>
	/// Add a <see cref="Line"/> instance into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="p1">Indicates the first point used in this line.</param>
	/// <param name="p2">Indicates the second point used in this line.</param>
	/// <param name="stroke">The stroke of the line.</param>
	/// <param name="strokeSize">The stroke thickness of the line.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddLine(this Grid @this, in Point p1, in Point p2, Brush stroke, double strokeSize)
	{
		var line = new Line
		{
			X1 = p1.X,
			X2 = p2.X,
			Y1 = p1.Y,
			Y2 = p2.Y,
			Stroke = stroke,
			StrokeThickness = strokeSize
		};

		@this.Children.Add(line);
	}

	/// <summary>
	/// Add cross sign into the <see cref="Grid"/> collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="l1x1">The X1 value of line 1.</param>
	/// <param name="l1y1">The Y1 value of line 1.</param>
	/// <param name="l1x2">The X2 value of line 1.</param>
	/// <param name="l1y2">The Y2 value of line 1.</param>
	/// <param name="l2x1">The X1 value of line 2.</param>
	/// <param name="l2y1">The X2 value of line 2.</param>
	/// <param name="l2x2">The Y1 value of line 2.</param>
	/// <param name="l2y2">The Y2 value of line 2.</param>
	/// <param name="cross">The <see cref="Brush"/> instance as the cross stroke.</param>
	/// <param name="strokeSize">The size of the cross stroke thickness.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddCross(
		this Grid @this, double l1x1, double l1y1, double l1x2, double l1y2,
		double l2x1, double l2y1, double l2x2, double l2y2, Brush cross, double strokeSize)
	{
		var l1 = new Line
		{
			X1 = l1x1,
			X2 = l1x2,
			Y1 = l1y1,
			Y2 = l1y2,
			Stroke = cross,
			StrokeThickness = strokeSize
		};
		var l2 = new Line
		{
			X1 = l2x1,
			X2 = l2x2,
			Y1 = l2y1,
			Y2 = l2y2,
			Stroke = cross,
			StrokeThickness = strokeSize
		};

		@this.Children.Add(l1);
		@this.Children.Add(l2);
	}
}
