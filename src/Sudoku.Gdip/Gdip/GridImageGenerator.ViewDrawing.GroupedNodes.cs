namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	private bool DrawDiagonalLines(
		ColorIdentifier identifier,
		float width,
		PointCalculator calc,
		SizeF cs,
		SizeF gs,
		Graphics g
	)
	{
		using var pen = new Pen(GetColor(identifier), width);

		var rect = new RectangleF(calc.GetMousePointInCenter(0) - cs, gs);
		g.DrawCrossSign(pen, rect);

		return true;
	}

	private bool DrawCapsule(
		int head,
		bool isHorizontal,
		float padding,
		PointCalculator calc,
		SizeF cs,
		ColorIdentifier identifier,
		float width,
		Graphics g
	)
	{
		var nextCell = head + (isHorizontal ? 1 : 9);
		var paddingSize = new SizeF(padding, padding);
		var topLeft = calc.GetMousePointInCenter(head) - cs / 2 + paddingSize;
		var bottomRight = calc.GetMousePointInCenter(nextCell) + cs / 2 - paddingSize;
		var rect = RectangleCreator.Create(topLeft, bottomRight);
		using var pen = new Pen(GetColor(identifier), width);

		g.DrawCapsule(pen, rect);

		return true;
	}

	private bool DrawObliqueLine(
		PointCalculator calc,
		int head,
		int tail,
		ColorIdentifier identifier,
		float width,
		float cw,
		float ch,
		Graphics g
	)
	{
		var (x1, y1) = calc.GetMousePointInCenter(head);
		var (x2, y2) = calc.GetMousePointInCenter(tail);
		var slope = x1 == x2 ? float.NaN : (y2 - y1) / (x2 - x1);

		using var pen = new Pen(GetColor(identifier), width);

		if (slope < 0)
		{
			x1 -= cw / 2;
			y1 -= ch / 2;
			x2 += cw / 2;
			y2 += ch / 2;
		}
		else
		{
			x1 -= cw / 2;
			y1 += ch / 2;
			x2 += cw / 2;
			y2 -= ch / 2;
		}

		g.DrawLine(pen, x1, y1, x2, y2);

		return true;
	}

	private bool DrawWindoku(ColorIdentifier identifier, PointCalculator calc, SizeF cs, Graphics g)
	{
		using var brush = new SolidBrush(GetColor(identifier));

		for (var row = 0; row < 9; row++)
		{
			for (var column = 0; column < 9; column++)
			{
				if ((row, column) is (0 or 4 or 8, 0 or 4 or 8))
				{
					var center = calc.GetMousePointInCenter(row * 9 + column);
					var topLeft = center - cs / 2;
					var bottomRight = center + cs / 2;
					var rect = RectangleCreator.Create(topLeft, bottomRight);

					g.FillRectangle(brush, rect);
				}
			}
		}

		return true;
	}

	private bool DrawPyramid(ColorIdentifier identifier, PointCalculator calc, SizeF cs, Graphics g)
	{
		using var brush = new SolidBrush(GetColor(identifier));

		for (var row = 0; row < 9; row++)
		{
			for (var column = 0; column < 9; column++)
			{
				if (Constants.PyramidStatusTable[row, column])
				{
					var center = calc.GetMousePointInCenter(row * 9 + column);
					var topLeft = center - cs / 2;
					var bottomRight = center + cs / 2;
					var rect = RectangleCreator.Create(topLeft, bottomRight);

					g.FillRectangle(brush, rect);
				}
			}
		}

		return true;
	}

	private bool DrawLever(
		float cw,
		float ch,
		float padding,
		ColorIdentifier identifier,
		int left,
		int right,
		int center,
		float barWidth,
		float pivotWidth,
		Graphics g
	)
	{
		var color = GetColor(identifier);
		var l = center % 9 * ch + ch;
		var r = center / 9 * cw + (cw - padding);
		var leverCenterPoint = new PointF(l, r);
		var leverLeftPoint = new PointF(left % 9 * ch + padding, left / 9 * cw + (cw - padding));
		var leverRightPoint = new PointF(right % 9 * ch + (ch - padding), right / 9 * cw + (cw - padding));

		// The lower points of the lever pivot.
		var ttl = new PointF(l - padding, r + padding / 2 + 2 * padding);
		var ttr = new PointF(l + padding, r + padding / 2 + 2 * padding);

		using var brush = new SolidBrush(color);
		using var pivotPen = new Pen(Color.FromArgb(192, color), pivotWidth);
		using var linePen = new Pen(Color.FromArgb(192, color), barWidth);
		using var pivotFillBrush = new SolidBrush(Color.FromArgb(128, color));

		// Draw bars.
		g.DrawLine(linePen, leverCenterPoint, leverLeftPoint);
		g.DrawLine(linePen, leverCenterPoint, leverRightPoint);

		// Draw pivot.
		g.DrawPolygon(pivotPen, new[] { leverCenterPoint, ttl, ttr });
		g.FillPolygon(pivotFillBrush, new[] { leverCenterPoint, ttl, ttr });

		return true;
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="constant"]'/>
file static class Constants
{
	/// <summary>
	/// The pyramid status table.
	/// </summary>
	public static readonly bool[,] PyramidStatusTable =
	{
		{ false, false, false,  true,  true,  true,  true,  true, false },
		{  true, false, false, false,  true,  true,  true, false, false },
		{  true,  true, false, false, false,  true, false, false, false },
		{  true,  true,  true, false, false, false, false, false,  true },
		{  true,  true, false, false, false, false, false,  true,  true },
		{  true, false, false, false, false, false,  true,  true,  true },
		{ false, false, false,  true, false, false, false,  true,  true },
		{ false, false,  true,  true,  true, false, false, false,  true },
		{ false,  true,  true,  true,  true,  true, false, false, false }
	};
}
