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
	/// <param name="fontStyle">
	/// Indicates the font style. The default value is <see cref="FontStyle.Normal"/>.
	/// </param>
	/// <seealso cref="TextBlock"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="Brush"/>
	/// <seealso cref="FontFamily"/>
	/// <seealso cref="HorizontalAlignment"/>
	/// <seealso cref="VerticalAlignment"/>
	/// <seealso cref="FontStyle"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddText(
		this Grid @this,
		int row,
		int column,
		int digit,
		Brush foreground,
		FontFamily font,
		double fontSize,
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Stretch,
		VerticalAlignment verticalAlignment = VerticalAlignment.Stretch,
		FontStyle fontStyle = FontStyle.Normal
	)
	{
		var tb = new TextBlock
		{
			HorizontalAlignment = horizontalAlignment,
			VerticalAlignment = verticalAlignment,
			Foreground = foreground,
			Text = (digit + 1).ToString(),
			FontFamily = font,
			FontSize = fontSize,
			FontStyle = fontStyle
		};

		Grid.SetRow(tb, row);
		Grid.SetColumn(tb, column);

		@this.Children.Add(tb);
	}

	/// <summary>
	/// Append a <see cref="TextBlock"/> control into the <see cref="Grid"/> collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="row">The row that the control is added to.</param>
	/// <param name="column">The column that the control is added to.</param>
	/// <param name="character">The character.</param>
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
	/// <param name="fontStyle">
	/// Indicates the font style. The default value is <see cref="FontStyle.Normal"/>.
	/// </param>
	/// <seealso cref="TextBlock"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="Brush"/>
	/// <seealso cref="FontFamily"/>
	/// <seealso cref="HorizontalAlignment"/>
	/// <seealso cref="VerticalAlignment"/>
	/// <seealso cref="FontStyle"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddText<TNotNull>(
		this Grid @this,
		int row,
		int column,
		TNotNull character,
		Brush foreground,
		FontFamily font,
		double fontSize,
		HorizontalAlignment horizontalAlignment = HorizontalAlignment.Stretch,
		VerticalAlignment verticalAlignment = VerticalAlignment.Stretch,
		FontStyle fontStyle = FontStyle.Normal
	) where TNotNull : notnull
	{
		var tb = new TextBlock
		{
			HorizontalAlignment = horizontalAlignment,
			VerticalAlignment = verticalAlignment,
			Foreground = foreground,
			Text = character.ToString(),
			FontFamily = font,
			FontSize = fontSize,
			FontStyle = fontStyle
		};

		Grid.SetRow(tb, row);
		Grid.SetColumn(tb, column);

		@this.Children.Add(tb);
	}

	/// <summary>
	/// Append the background color that effects the whole <see cref="Grid"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="width">Indicates the width of the background.</param>
	/// <param name="height">Indicates the height of the background.</param>
	/// <param name="fill">The <see cref="Brush"/> instance as the filled background.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddBackground(this Grid @this, double width, double height, Brush fill)
	{
		var rectangle = new Rectangle { Width = width, Height = height, Fill = fill };

		Grid.SetRow(rectangle, 0);
		Grid.SetRowSpan(rectangle, PointCalculator.AnchorsCount);
		Grid.SetColumn(rectangle, 0);
		Grid.SetColumnSpan(rectangle, PointCalculator.AnchorsCount);

		@this.Children.Add(rectangle);
	}

	/// <summary>
	/// Append the background color that effects the whole <see cref="Grid"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="size">Indicates the size.</param>
	/// <param name="fill">The <see cref="Brush"/> instance as the filled background.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddBackground(this Grid @this, in Size size, Brush fill) =>
		@this.AddBackground(size.Width, size.Height, fill);

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
		this Grid @this,
		int row,
		int column,
		int rowSpan,
		int columnSpan,
		Brush fill,
		double radiusX = 0,
		double radiusY = 0
	)
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
	/// <param name="strokeThickness">Indictaes the stroke size. The default value is <see cref="double.NaN"/>.</param>
	/// <seealso cref="Ellipse"/>
	/// <seealso cref="Grid"/>
	/// <seealso cref="Brush"/>
	/// <exception cref="ArgumentException">
	/// Throws when <paramref name="strokeThickness"/> is invalid (<paramref name="stroke"/> isn't
	/// <see langword="null"/> but <paramref name="strokeThickness"/> is <see cref="double.NaN"/>);
	/// or <paramref name="stroke"/> is <see langword="null"/> but <paramref name="strokeThickness"/>
	/// is <see cref="double.NaN"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddCircle(
		this Grid @this,
		int row,
		int column,
		double size,
		Brush fill,
		Brush? stroke = null,
		double strokeThickness = double.NaN
	)
	{
		switch ((Stroke: stroke, StrokeSize: strokeThickness))
		{
			case (Stroke: not null, StrokeSize: not double.NaN):
			{
				var circle = new Ellipse
				{
					Width = size,
					Height = size,
					Fill = fill,
					Stroke = stroke,
					StrokeThickness = strokeThickness
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
				throw new ArgumentException("The specified argument is invalid.", nameof(strokeThickness));
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
	/// <param name="strokeThickness">The stroke thickness of the line.</param>
	/// <param name="lineCap">Indicates the line cap. The default value is <see cref="PenLineCap.Flat"/>.</param>
	/// <param name="dashArray">Indicates the dash array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddLine(
		this Grid @this, double x1, double y1, double x2, double y2, Brush stroke, double strokeThickness,
		PenLineCap lineCap = PenLineCap.Flat, DoubleCollection? dashArray = null) =>
		@this.AddLine(new(x1, y1), new(x2, y2), stroke, strokeThickness, lineCap, dashArray);

	/// <summary>
	/// Add a <see cref="Line"/> instance into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="p1">Indicates the first point used in this line.</param>
	/// <param name="p2">Indicates the second point used in this line.</param>
	/// <param name="stroke">The stroke of the line.</param>
	/// <param name="strokeSize">The stroke thickness of the line.</param>
	/// <param name="lineCap">Indicates the line cap. The default value is <see cref="PenLineCap.Flat"/>.</param>
	/// <param name="dashArray">Indicates the dash array. The default value is <see langword="null"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddLine(
		this Grid @this,
		in Point p1,
		in Point p2,
		Brush stroke,
		double strokeSize,
		PenLineCap lineCap = PenLineCap.Flat,
		DoubleCollection? dashArray = null
	)
	{
		var line = new Line
		{
			X1 = p1.X,
			X2 = p2.X,
			Y1 = p1.Y,
			Y2 = p2.Y,
			Stroke = stroke,
			StrokeThickness = strokeSize,
			StrokeEndLineCap = lineCap,
			StrokeDashArray = dashArray ?? new()
		};

		@this.Children.Add(line);
	}

	/// <summary>
	/// Add a <see cref="Path"/> that constructed by a <see cref="BezierSegment"/> into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="startX">The X value of the start point.</param>
	/// <param name="startY">The Y value of the start point.</param>
	/// <param name="p1X">The X value of the point 1.</param>
	/// <param name="p1Y">The Y value of the point 1.</param>
	/// <param name="p2X">The X value of the point 2.</param>
	/// <param name="p2Y">The Y value of the point 2.</param>
	/// <param name="endX">The X value of the end point.</param>
	/// <param name="endY">The Y value of the end point.</param>
	/// <param name="strokeThickness">Indicates the stroke thickness.</param>
	/// <param name="lineCap">The line cap. The default value is <see cref="PenLineCap.Flat"/>.</param>
	/// <param name="dashArray">The dash array. The default is <see langword="null"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddBezier(
		this Grid @this, double startX, double startY, double p1X, double p1Y, double p2X, double p2Y,
		double endX, double endY, double strokeThickness, PenLineCap lineCap = PenLineCap.Flat,
		DoubleCollection? dashArray = null) =>
		@this.AddBezier(
			new(startX, startY), new(p1X, p1Y), new(p2X, p2Y), new(endX, endY),
			strokeThickness, lineCap, dashArray
		);

	/// <summary>
	/// Add a <see cref="Path"/> that constructed by a <see cref="BezierSegment"/> into the collection.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="start">The start point.</param>
	/// <param name="p1">The point 1 that is used for constructing of a bezier.</param>
	/// <param name="p2">The point 2 that is used for constructing of a bezier.</param>
	/// <param name="end">The end point.</param>
	/// <param name="strokeThickness">Indicates the stroke thickness.</param>
	/// <param name="lineCap">The line cap. The default value is <see cref="PenLineCap.Flat"/>.</param>
	/// <param name="dashArray">The dash array. The default is <see langword="null"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddBezier(
		this Grid @this,
		in Point start,
		in Point p1,
		in Point p2,
		in Point end,
		double strokeThickness,
		PenLineCap lineCap = PenLineCap.Flat,
		DoubleCollection? dashArray = null
	)
	{
		var pathFigure = new PathFigure { StartPoint = start };
		var pathData = new PathGeometry();
		pathFigure.Segments.Add(new BezierSegment { Point1 = p1, Point2 = p2, Point3 = end });
		pathData.Figures.Add(pathFigure);

		@this.Children.Add(new Path
		{
			StrokeThickness = strokeThickness,
			Data = pathData,
			StrokeEndLineCap = lineCap,
			StrokeDashArray = dashArray
		});
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
		this Grid @this,
		double l1x1,
		double l1y1,
		double l1x2,
		double l1y2,
		double l2x1,
		double l2y1,
		double l2x2,
		double l2y2,
		Brush cross,
		double strokeSize
	)
	{
		@this.AddLine(l1x1, l1y1, l1x2, l1y2, cross, strokeSize);
		@this.AddLine(l2x1, l2y1, l2x2, l2y2, cross, strokeSize);
	}
}
