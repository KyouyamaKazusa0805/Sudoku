namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	private void DrawBorderBar(
		Identifier identifier,
		float barWidth,
		PointCalculator calc,
		int c1,
		int c2,
		bool fullyOverlapping,
		Graphics g
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		using var pen = new Pen(brush, barWidth);

		// Draw bars.
		var (start, end) = calc.GetSharedLinePosition(c1, c2, fullyOverlapping);
		g.DrawLine(pen, start, end);
	}

	private void DrawKropkiDot(
		Identifier identifier,
		Color backColor,
		float borderWidth,
		PointCalculator calc,
		int c1,
		int c2,
		float dotSize,
		bool isSolid,
		Graphics g
	)
	{
		using var solidBrush = new SolidBrush(GetColor(identifier));
		using var hollowBrush = new SolidBrush(backColor);
		using var pen = new Pen(solidBrush, borderWidth);

		var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
		var rect = new RectangleF((x1 + x2) / 2 - dotSize / 2, (y1 + y2) / 2 - dotSize / 2, dotSize, dotSize);

		/// Draw Kropki dots.
		/// Please note that method <see cref="Graphics.DrawEllipse"/> and <see cref="Graphics.FillEllipse"/>
		/// starts with the point at top-left position, rather than the center.
		g.DrawEllipse(pen, rect);
		g.FillEllipse(isSolid ? solidBrush : hollowBrush, rect);
	}

	private void DrawGreaterThanSign(
		FontData fontData,
		Color backColor,
		Identifier identifier,
		bool isGreaterThan,
		PointCalculator calc,
		int c1,
		int c2,
		bool isRow,
		Graphics g
	)
	{
		using var font = fontData.CreateFont();
		using var backBrush = new SolidBrush(backColor);
		using var brush = new SolidBrush(GetColor(identifier));
		var text = isGreaterThan ? ">" : "<";
		var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
		var centerPoint = new PointF((x1 + x2) / 2, (y1 + y2) / 2);

		// Draw sign.
		if (isRow)
		{
			var textSize = g.MeasureString(text, font);
			var (tw, th) = textSize;
			var (pointX, pointY) = centerPoint - textSize / 2;

			g.FillRectangle(backBrush, pointX, pointY, tw, th);
			g.DrawString(text, font, brush, centerPoint, StringLocating);
		}
		else
		{
			// If column-ish, we need rotate the text.
			// By using 'g.Transform', we can rotate a text shape on rendering.
			// Please note that 'Matrix' type is a reference type: be careful to assign and replace.

			var matrixOriginal = g.Transform;
			using var matrixRotating = g.Transform.Clone();
			matrixRotating.RotateAt(90, centerPoint);
			g.Transform = matrixRotating;

			var textSize = g.MeasureString(text, font);
			var (tw, th) = textSize;
			var (pointX, pointY) = centerPoint - textSize / 2;

			g.FillRectangle(backBrush, pointX, pointY, tw, th);
			g.DrawString(text, font, brush, centerPoint, StringLocating);

			g.Transform = matrixOriginal;
		}
	}

	private void DrawXvSign(
		FontData fontData,
		Color backColor,
		Identifier identifier,
		bool isX,
		PointCalculator calc,
		int c1,
		int c2,
		Graphics g
	)
	{
		using var font = fontData.CreateFont();
		using var backBrush = new SolidBrush(backColor);
		using var brush = new SolidBrush(GetColor(identifier));
		var text = isX ? "X" : "V";
		var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
		var centerPoint = new PointF((x1 + x2) / 2, (y1 + y2) / 2);
		var textSize = g.MeasureString(text, font);
		var (tw, th) = textSize;
		var (pointX, pointY) = centerPoint - textSize / 2;

		g.FillRectangle(backBrush, pointX, pointY, tw, th);
		g.DrawString(text, font, brush, centerPoint, StringLocating);
	}

	private void DrawNumberLabel(
		FontData fontData,
		Color backColor,
		Identifier identifier,
		PointCalculator calc,
		int c1,
		int c2,
		Graphics g,
		string label
	)
	{
		using var font = fontData.CreateFont();
		using var backBrush = new SolidBrush(backColor);
		using var brush = new SolidBrush(GetColor(identifier));
		var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
		var centerPoint = new PointF((x1 + x2) / 2, (y1 + y2) / 2);
		var textSize = g.MeasureString(label, font);
		var (tw, th) = textSize;
		var (pointX, pointY) = centerPoint - textSize / 2;

		g.FillRectangle(backBrush, pointX, pointY, tw, th);
		g.DrawString(label, font, brush, centerPoint, StringLocating);
	}

	private void DrawBattenburg(
		Identifier identifier,
		PointCalculator calc,
		int lastCell,
		SizeF cellSize,
		float battenburgSize,
		Graphics g
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		using var pen = new Pen(Brushes.Black);

		var (tempX, tempY) = calc.GetMousePointInCenter(lastCell) - cellSize / 2;
		var p1 = new PointF(tempX - battenburgSize / 2, tempY - battenburgSize / 2);
		var p2 = new PointF(tempX, tempY - battenburgSize / 2);
		var p3 = new PointF(tempX - battenburgSize / 2, tempY);
		var p4 = new PointF(tempX, tempY);
		scoped var points = (stackalloc[] { p1, p2, p3, p4 });
		for (var i = 0; i < points.Length; i++)
		{
			var (x, y) = points[i];
			if (i is 0 or 3)
			{
				g.DrawRectangle(pen, x, y, battenburgSize / 2, battenburgSize / 2);
				g.FillRectangle(brush, x, y, battenburgSize / 2, battenburgSize / 2);
			}
			else
			{
				g.DrawRectangle(pen, x, y, battenburgSize / 2, battenburgSize / 2);
			}
		}
	}

	private void DrawQuadrupleHint(
		FontData fontData,
		Color backColor,
		Identifier identifier,
		Graphics g,
		string hint,
		PointCalculator calc,
		int lastCell,
		float cw,
		float ch
	)
	{
		using var font = fontData.CreateFont();
		using var brush = new SolidBrush(backColor);
		using var textColor = new SolidBrush(GetColor(identifier));
		var (tw, th) = g.MeasureString(hint, font);
		var (x, y) = calc.GetMousePointInCenter(lastCell);

		g.FillRectangle(brush, x - cw / 2 - tw / 2, y - ch / 2 - th / 2, tw, th);
		g.DrawString(hint, font, textColor, x - cw / 2, y - ch / 2, StringLocating);
	}

	private void DrawClockfaceDot(
		Identifier identifier,
		float borderWidth,
		Color backColor,
		PointCalculator calc,
		int lastCell,
		bool isClockwise,
		Graphics g,
		float cw,
		float ch,
		float dotSize
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		using var pen = new Pen(brush, borderWidth);
		using var backBrush = new SolidBrush(backColor);

		var (x, y) = calc.GetMousePointInCenter(lastCell);
		if (isClockwise)
		{
			g.DrawEllipse(pen, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
			g.FillEllipse(backBrush, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
		}
		else
		{
			g.DrawEllipse(pen, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
			g.FillEllipse(brush, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
		}
	}

	private void DrawNeighborSign(
		Identifier identifier,
		float width,
		PointCalculator calc,
		int cell,
		float cw,
		float ch,
		float padding,
		bool isFourDirections,
		Graphics g
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		using var pen = new Pen(brush, width);

		var (x, y) = calc.GetMousePointInCenter(cell);
		var topLeft = new PointF(x - cw / 2 + padding, y - ch / 2 + padding);
		var bottomRight = new PointF(x + cw / 2 - padding, y + ch / 2 - padding);
		var rect = RectangleMarshal.CreateInstance(topLeft, bottomRight);

		((Action<Pen, RectangleF>)(isFourDirections ? g.DrawCrossSign : g.DrawEllipse))(pen, rect);
	}

	private void DrawWheel(
		Color backColor,
		FontData fontData,
		Color textColor,
		Identifier identifier,
		float width,
		PointCalculator calc,
		int cell,
		float cw,
		float ch,
		string digitString,
		Graphics g
	)
	{
		using var backBrush = new SolidBrush(backColor);
		using var font = fontData.CreateFont();
		using var textBrush = new SolidBrush(textColor);
		using var pen = new Pen(GetColor(identifier), width);

		var (x, y) = calc.GetMousePointInCenter(cell);
		var topLeft = new PointF(x - cw * SqrtOf2 / 2, y - ch * SqrtOf2 / 2);
		var bottomRight = new PointF(x + cw * SqrtOf2 / 2, y + ch * SqrtOf2 / 2);
		var rect = RectangleMarshal.CreateInstance(topLeft, bottomRight);

		// Draw wheel main circle.
		g.DrawEllipse(pen, rect);

		// Draw strings.
		scoped var positions = (stackalloc PointF[]
		{
			new(x, y - ch * SqrtOf2 / 2),
			new(x + ch * SqrtOf2 / 2, y),
			new(x, y + ch * SqrtOf2 / 2),
			new(x - ch * SqrtOf2 / 2, y)
		});
		for (var i = 0; i < 4; i++)
		{
			var renderingText = digitString[i].ToString();
			var position = positions[i];
			var renderingSize = g.MeasureString(renderingText, font);

			g.FillRectangle(backBrush, new RectangleF(position - renderingSize / 2, renderingSize));
			g.DrawString(renderingText, font, textBrush, position, StringLocating);
		}
	}

	private void DrawPencilmark(
		FontData fontData,
		Color textColor,
		Graphics g,
		string notation,
		PointCalculator calc,
		int cell,
		float ch
	)
	{
		using var font = fontData.CreateFont();
		using var textBrush = new SolidBrush(textColor);
		var renderingSize = g.MeasureString(notation, font);
		var (_, th) = renderingSize;
		var (centerX, centerY) = calc.GetMousePointInCenter(cell);
		var position = new PointF(centerX, centerY - ch / 2 + th / 2);

		// Draw text.
		g.DrawString(notation, font, textBrush, position, StringLocating);
	}

	private void DrawTriangleSum(
		Identifier identifier,
		float padding,
		int cell,
		Direction directions,
		bool isComplement,
		Graphics g,
		PointCalculator calc
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		using var path = createPath(padding, cell, directions, isComplement);

		// Draw shape.
		g.FillPath(brush, path);


		GraphicsPath createPath(float padding, int cell, Direction directions, bool isComplement)
		{
			var (cw, ch) = calc.CellSize;
			var (x, y) = calc.GetMousePointInCenter(cell);
			var p1 = new PointF(x - cw / 2 + padding, y - ch / 2 + padding);
			var p2 = new PointF(x + cw / 2 - padding, y - ch / 2 + padding);
			var p3 = new PointF(x - cw / 2 + padding, y + ch / 2 - padding);
			var p4 = new PointF(x + cw / 2 - padding, y + ch / 2 - padding);

			var path = new GraphicsPath(FillMode.Winding);
			scoped var pathPointsOrdering = (isComplement, directions) switch
			{
				(true, _) => stackalloc[] { p1, p2, p4, p3 },
				(_, Direction.TopLeft) => stackalloc[] { p1, p2, p3 },
				(_, Direction.TopRight) => stackalloc[] { p1, p2, p4 },
				(_, Direction.BottomLeft) => stackalloc[] { p1, p4, p3 },
				(_, Direction.BottomRight) => stackalloc[] { p2, p4, p3 }
			};

			for (var i = 0; i < pathPointsOrdering.Length - 1; i++)
			{
				var p = pathPointsOrdering[i];
				var q = pathPointsOrdering[i + 1];
				path.AddLine(p, q);
			}

			return path;
		}
	}

	private void DrawStarProductStar(
		FontData fontData,
		Identifier identifier,
		Graphics g,
		PointCalculator calc,
		int cell,
		Direction direction,
		float cw,
		float ch
	)
	{
		const string star = "*";

		using var font = fontData.CreateFont();
		using var brush = new SolidBrush(GetColor(identifier));
		var (tw, th) = g.MeasureString(star, font);
		var (x, y) = calc.GetMousePointInCenter(cell);
		var point = direction switch
		{
			Direction.TopLeft => new(x - cw / 2 + tw / 2, y - ch / 2 + th / 2),
			Direction.TopCenter => new(x, y - ch / 2 + th / 2),
			Direction.TopRight => new(x - cw / 2 + tw / 2, y + ch / 2 - th / 2),
			Direction.MiddleLeft => new(x - cw / 2 + tw / 2, y),
			Direction.MiddleRight => new(x + cw / 2 - tw / 2, y + ch / 2 - th / 2),
			Direction.BottomLeft => new(x - cw / 2 + tw / 2, y + ch / 2 - th / 2),
			Direction.BottomCenter => new(x, y - ch / 2 + th / 2),
			Direction.BottomRight => new(x + cw / 2 - tw / 2, y + ch / 2 - th / 2),
			_ => default(PointF)
		};

		g.DrawString(star, font, brush, point, StringLocating);
	}

	private void DrawCellArrow(
		Identifier identifier,
		PointCalculator calc,
		int cell,
		Direction direction,
		float cw,
		float ch,
		Graphics g
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));

		var center = calc.GetMousePointInCenter(cell);
		var rotation = direction.GetRotatingAngle();

		g.DrawHollowArrow(brush, center, cw / 4, cw / 2, ch / 2, rotation);
	}

	private void DrawQuadrupleMaxArrow(
		Identifier identifier,
		PointCalculator calc,
		int lastCell,
		float cw,
		float ch,
		Direction direction,
		Graphics g,
		float size
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		var (centerX, centerY) = calc.GetMousePointInCenter(lastCell);
		var point = new PointF(centerX - cw / 2, centerY - ch / 2);
		var rotation = direction.GetRotatingAngle();

		g.DrawHollowArrow(brush, point, size, size * 2, size * 2, rotation);
	}

	private void DrawCellCornerTriangle(
		Identifier identifier,
		PointCalculator calc,
		int cell,
		Direction direction,
		float cw,
		float ch,
		float padding,
		float size,
		Graphics g
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));
		var (centerX, centerY) = calc.GetMousePointInCenter(cell);
		var points = direction switch
		{
			Direction.TopLeft => new PointF[]
			{
				new(centerX - cw / 2 + padding, centerY - ch / 2 + padding),
				new(centerX - cw / 2 + padding + size, centerY - ch / 2 + padding),
				new(centerX - cw / 2 + padding, centerY - ch / 2 + padding + size)
			},
			Direction.TopRight => new PointF[]
			{
				new(centerX + cw / 2 - padding, centerY - ch / 2 + padding),
				new(centerX + cw / 2 - padding - size, centerY - ch / 2 + padding),
				new(centerX + cw / 2 - padding, centerY - ch / 2 + padding + size)
			},
			Direction.BottomLeft => new PointF[]
			{
				new(centerX - cw / 2 + padding, centerY + ch / 2 - padding),
				new(centerX - cw / 2 + padding + size, centerY + ch / 2 - padding),
				new(centerX - cw / 2 + padding, centerY + ch / 2 - padding - size)
			},
			Direction.BottomRight => new PointF[]
			{
				new(centerX + cw / 2 - padding, centerY + ch / 2 - padding),
				new(centerX + cw / 2 - padding - size, centerY + ch / 2 - padding),
				new(centerX + cw / 2 - padding, centerY + ch / 2 - padding - size)
			},
		};

		using var path = new GraphicsPath();
		path.AddLine(points[0], points[1]);
		path.AddLine(points[1], points[2]);
		path.AddLine(points[2], points[0]);

		g.FillPath(brush, path);
	}

	private void DrawAverageBar(
		Identifier identifier,
		float width,
		PointCalculator calc,
		int cell,
		AdjacentCellType type,
		float cw,
		float ch,
		Graphics g
	)
	{
		using var pen = new Pen(GetColor(identifier), width);
		var (x, y) = calc.GetMousePointInCenter(cell);

		// Draw line.
		foreach (var (p1, p2) in type switch
		{
			AdjacentCellType.Rowish => new (PointF, PointF)[] { (new(x - cw / 2, y), new(x + cw / 2, y)) },
			AdjacentCellType.Columnish => new (PointF, PointF)[] { (new(x, y - ch / 2), new(x, y + ch / 2)) },
			AdjacentCellType.Rowish | AdjacentCellType.Columnish => new (PointF, PointF)[]
			{
				(new(x - cw / 2, y), new(x + cw / 2, y)),
				(new(x, y - ch / 2), new(x, y + ch / 2))
			}
		})
		{
			g.DrawLine(pen, p1, p2);
		}
	}

	private void DrawCellCornerArrow(
		Identifier identifier,
		PointCalculator calc,
		int cell,
		float ch,
		float width,
		Direction directions,
		Graphics g
	)
	{
		using var brush = new SolidBrush(GetColor(identifier));

		var center = calc.GetMousePointInCenter(cell);
		var (centerX, centerY) = center;
		var (x, y) = new PointF(centerX, centerY - ch / 2 + Tan(PI / 3) / 2 * width);
		var p1 = new PointF(x - width / 2, y);
		var p2 = new PointF(x + width / 2, y);
		var p3 = new PointF(x, centerY - ch / 2);

		foreach (var direction in directions.GetAllFlagsDistinct()!)
		{
			using var path = new GraphicsPath();
			path.AddLine(p1, p2);
			path.AddLine(p2, p3);
			path.AddLine(p3, p1);

			// Rotating.
			var oldMatrix = g.Transform;
			using var newMatrix = g.Transform.Clone();
			newMatrix.RotateAt(direction.GetRotatingAngle(), center);

			g.Transform = newMatrix;
			g.FillPath(brush, path);
			g.Transform = oldMatrix;
		}
	}

	private void DrawEmbeddedSkyscraperArrow(
		FontData fontData,
		Identifier identifier,
		PointCalculator calc,
		int cell,
		Direction directions,
		Graphics g,
		float cw,
		float ch
	)
	{
		const string left = "\u2190", up = "\u2191", right = "\u2192", down = "\u2193";

		using var font = fontData.CreateFont();
		using var brush = new SolidBrush(GetColor(identifier));

		var (centerX, centerY) = calc.GetMousePointInCenter(cell);
		foreach (var direction in directions.GetAllFlagsDistinct()!)
		{
			var finalText = direction switch { Direction.Up => up, Direction.Down => down, Direction.Left => left, Direction.Right => right };
			var (tw, th) = g.MeasureString(finalText, font);
			var textCenter = direction switch
			{
				Direction.Up => new PointF(centerX, centerY - ch / 2 + th / 2),
				Direction.Down => new PointF(centerX, centerY + ch / 2 - th / 2),
				Direction.Left => new PointF(centerX - cw / 2 + tw / 2, centerY),
				Direction.Right => new PointF(centerX + cw / 2 - tw / 2, centerY)
			};

			g.DrawString(finalText, font, brush, textCenter, StringLocating);
		}
	}
}
