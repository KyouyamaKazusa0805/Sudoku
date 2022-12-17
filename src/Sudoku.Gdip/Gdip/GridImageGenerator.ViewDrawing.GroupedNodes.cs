namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	private bool DrawDiagonalLines(
		Identifier identifier,
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
		AdjacentCellType adjacentType,
		float padding,
		PointCalculator calc,
		SizeF cs,
		Identifier identifier,
		float width,
		Graphics g
	)
	{
		var nextCell = head + adjacentType switch { AdjacentCellType.Rowish => 1, AdjacentCellType.Columnish => 9 };
		var paddingSize = new SizeF(padding, padding);
		var topLeft = calc.GetMousePointInCenter(head) - cs / 2 + paddingSize;
		var bottomRight = calc.GetMousePointInCenter(nextCell) + cs / 2 - paddingSize;
		var rect = RectangleMarshal.CreateInstance(topLeft, bottomRight);

		using var pen = new Pen(GetColor(identifier), width);

		g.DrawCapsule(pen, rect);

		return true;
	}

	private bool DrawObliqueLine(
		PointCalculator calc,
		int head,
		int tail,
		Identifier identifier,
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

	private bool DrawWindoku(Identifier identifier, PointCalculator calc, SizeF cs, Graphics g)
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
					var rect = RectangleMarshal.CreateInstance(topLeft, bottomRight);

					g.FillRectangle(brush, rect);
				}
			}
		}

		return true;
	}
}
