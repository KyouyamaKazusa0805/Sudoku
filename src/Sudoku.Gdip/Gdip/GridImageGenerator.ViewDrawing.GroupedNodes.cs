namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	private void DrawDiagonalLines(
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
	}

	private void DrawCapsule(
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
	}

	private void DrawObliqueLine()
	{
#if true
		throw new NotImplementedException();
#else
		var (x1, y1) = calc.GetMousePointInCenter(head);
		var (x2, y2) = calc.GetMousePointInCenter(tail);
		var slope = x1 == x2 ? float.NaN : (y2 - y1) / (x2 - x1);

		using var pen = new Pen(GetColor(identifier), width);

		break;
#endif
	}
}
