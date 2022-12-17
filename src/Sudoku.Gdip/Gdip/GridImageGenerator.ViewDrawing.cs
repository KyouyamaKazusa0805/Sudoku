namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	/// <summary>
	/// Draw custom view if <see cref="View"/> is not <see langword="null"/>.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	/// <seealso cref="View"/>
	private void DrawView(Graphics g)
	{
		if (View is null)
		{
			return;
		}

		DrawHouses(g);
		DrawCells(g);
		DrawCandidates(g);
		DrawLinks(g);
		DrawUnknownValue(g);
		DrawFigure(g);

		DrawShapeNodes(g);
		DrawGroupedNodes(g);
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawEliminations(Graphics g)
	{
		if (this is not
			{
				Conclusions: { } conclusions,
				Preferences: { EliminationColor: var eColor, CannibalismColor: var cColor },
				View: var view
			} || !conclusions.Any())
		{
			return;
		}

		using var elimBrush = new SolidBrush(eColor);
		using var cannibalBrush = new SolidBrush(cColor);
		using var elimBrushLighter = new SolidBrush(eColor.QuarterAlpha());
		using var canniBrushLighter = new SolidBrush(cColor.QuarterAlpha());
		foreach (var (t, c, d) in conclusions)
		{
			if (t != ConclusionType.Elimination)
			{
				continue;
			}

			var cannibalism = false;
			if (view is null)
			{
				goto Drawing;
			}

			foreach (var candidateNode in view.OfType<CandidateViewNode>())
			{
				var value = candidateNode.Candidate;
				if (value == c * 9 + d)
				{
					cannibalism = true;
					break;
				}
			}

		Drawing:
			g.FillEllipse(
				(cannibalism, view.UnknownOverlaps(c)) switch
				{
					(true, true) => canniBrushLighter,
					(true, false) => cannibalBrush,
					(false, true) => elimBrushLighter,
					_ => elimBrush
				},
				Calculator.GetMouseRectangle(c, d)
			);
		}
	}

	/// <summary>
	/// Draw <see cref="ShapeViewNode"/> instances.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	private void DrawShapeNodes(Graphics g)
	{
		if (this is not { View.ShapeViewNodes: var nodes, Calculator: { CellSize: (var cw, var ch) cellSize } calc })
		{
			return;
		}

		foreach (var node in nodes)
		{
			_ = (This: this, Node: node) switch
			{
				// Consecutive bar
				{
					This.Preferences: { BorderBarWidth: var barWidth, BorderBarFullyOverlapsGridLine: var fullyOverlapping },
					Node: BorderBarViewNode(var c1, var c2) { Identifier: var identifier }
				} => DrawBorderBar(identifier, barWidth, calc, c1, c2, fullyOverlapping, g),

				// Kropki dot
				{
					This.Preferences: { KropkiDotBorderWidth: var borderWidth, KropkiDotSize: var dotSize, BackgroundColor: var backColor },
					Node: KropkiDotViewNode(var c1, var c2) { Identifier: var identifier, IsSolid: var isSolid }
				} => DrawKropkiDot(identifier, backColor, borderWidth, calc, c1, c2, dotSize, isSolid, g),

				// Greater-than sign
				{
					This.Preferences: { GreaterThanSignFont: var fontData, BackgroundColor: var backColor },
					Node: GreaterThanSignViewNode(var c1, var c2, var isRow)
					{
						Identifier: var identifier,
						IsGreaterThan: var isGreaterThan
					}
				} => DrawGreaterThanSign(fontData, backColor, identifier, isGreaterThan, calc, c1, c2, isRow, g),

				// XV sign
				{
					This.Preferences: { XvSignFont: var fontData, BackgroundColor: var backColor },
					Node: XvSignViewNode(var c1, var c2) { Identifier: var identifier, IsX: var isX }
				} => DrawXvSign(fontData, backColor, identifier, isX, calc, c1, c2, g),

				// Number label
				{
					This.Preferences: { NumberLabelFont: var fontData, BackgroundColor: var backColor },
					Node: NumberLabelViewNode(var c1, var c2) { Identifier: var identifier, Label: var label }
				} => DrawNumberLabel(fontData, backColor, identifier, calc, c1, c2, g, label),

				// Battenburg
				{
					This.Preferences.BattenburgSize: var battenburgSize,
					Node: BattenburgViewNode { Identifier: var identifier, Cells: [.., var lastCell] }
				} => DrawBattenburg(identifier, calc, lastCell, cellSize, battenburgSize, g),

				// Quadruple hint
				{
					This.Preferences: { QuadrupleHintFont: var fontData, BackgroundColor: var backColor },
					Node: QuadrupleHintViewNode { Identifier: var identifier, Cells: [.., var lastCell], Hint: var hint }
				} => DrawQuadrupleHint(fontData, backColor, identifier, g, hint, calc, lastCell, cw, ch),

				// Clockface dot
				{
					This.Preferences:
					{
						ClockfaceDotSize: var dotSize,
						ClockfaceDotBorderWidth: var borderWidth,
						BackgroundColor: var backColor
					},
					Node: ClockfaceDotViewNode { Identifier: var identifier, Cells: [.., var lastCell], IsClockwise: var isClockwise }
				} => DrawClockfaceDot(identifier, borderWidth, backColor, calc, lastCell, isClockwise, g, cw, ch, dotSize),

				// Neighbor sign
				{
					This.Preferences: { NeighborSignsWidth: var width, NeighborSignCellPadding: var padding },
					Node: NeighborSignViewNode(var cell, _) { Identifier: var identifier, IsFourDirections: var isFourDirections }
				} => DrawNeighborSign(identifier, width, calc, cell, cw, ch, padding, isFourDirections, g),

				// Wheel
				{
					This.Preferences:
					{
						WheelFont: var fontData,
						WheelWidth: var width,
						WheelTextColor: var textColor,
						BackgroundColor: var backColor
					},
					Node: WheelViewNode(var cell, _) { Identifier: var identifier, DigitString: var digitString }
				} => DrawWheel(backColor, fontData, textColor, identifier, width, calc, cell, cw, ch, digitString, g),

				// Pencilmark
				{
					This.Preferences: { PencilmarkFont: var fontData, PencilmarkTextColor: var textColor },
					Node: PencilMarkViewNode(var cell, _) { Notation: var notation }
				} => DrawPencilmark(fontData, textColor, g, notation, calc, cell, ch),

				// Triangle sum
				{
					This.Preferences.TriangleSumCellPadding: var padding,
					Node: TriangleSumViewNode(var cell, var directions) { Identifier: var identifier, IsComplement: var isComplement }
				} => DrawTriangleSum(identifier, padding, cell, directions, isComplement, g, calc),

				// Star product star
				{
					This.Preferences.StarProductStarFont: var fontData,
					Node: StarProductStarViewNode(var cell, var direction) { Identifier: var identifier }
				} => DrawStarProductStar(fontData, identifier, g, calc, cell, direction, cw, ch),

				// Cell arrow
				{
					Node: CellArrowViewNode(var cell, var direction) { Identifier: var identifier }
				} => DrawCellArrow(identifier, calc, cell, direction, cw, ch, g),

				// Quadruple max arrow
				{
					This.Preferences.QuadrupleMaxArrowSize: var size,
					Node: QuadrupleMaxArrowViewNode { Cells: [.., var lastCell], Identifier: var identifier, ArrowDirection: var direction }
				} => DrawQuadrupleMaxArrow(identifier, calc, lastCell, cw, ch, direction, g, size),

				// Cell-corner triangle
				{
					This.Preferences: { CellCornerTriangleSize: var size, CellCornerTriangleCellPadding: var padding },
					Node: CellCornerTriangleViewNode { Identifier: var identifier, Cell: var cell, Directions: var direction }
				} => DrawCellCornerTriangle(identifier, calc, cell, direction, cw, ch, padding, size, g),

				// Average bar
				{
					This.Preferences.AverageBarWidth: var width,
					Node: AverageBarViewNode { Identifier: var identifier, Cell: var cell, Type: var type }
				} => DrawAverageBar(identifier, width, calc, cell, type, cw, ch, g),

				// Cell-corner arrow
				{
					This.Preferences.CellCornerArrowWidth: var width,
					Node: CellCornerArrowViewNode { Identifier: var identifier, Cell: var cell, Directions: var directions }
				} => DrawCellCornerArrow(identifier, calc, cell, ch, width, directions, g),

				// Embedded skyscraper arrow
				{
					This.Preferences.EmbeddedSkyscraperArrowFont: var fontData,
					Node: EmbeddedSkyscraperArrowViewNode { Identifier: var identifier, Cell: var cell, Directions: var directions }
				} => DrawEmbeddedSkyscraperArrow(fontData, identifier, calc, cell, directions, g, cw, ch)
			};
		}
	}

	/// <summary>
	/// Draw <see cref="GroupedViewNode"/> instances.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	private void DrawGroupedNodes(Graphics g)
	{
		if (this is not { View.GroupedViewNodes: var nodes, Calculator: { CellSize: (var cw, var ch) cs, GridSize: var gs } calc })
		{
			return;
		}

		foreach (var node in nodes)
		{
			_ = (This: this, Node: node) switch
			{
				// Diagonal lines
				{
					This.Preferences.DiagonalLinesWidth: var width,
					Node: DiagonalLinesViewNode { Identifier: var identifier }
				} => DrawDiagonalLines(identifier, width, calc, cs, gs, g),

				// Capsule
				{
					This.Preferences: { CapsulePadding: var padding, CapsuleWidth: var width },
					Node: CapsuleViewNode(var head, _) { Identifier: var identifier, AdjacentType: var adjacentType }
				} => DrawCapsule(head, adjacentType, padding, calc, cs, identifier, width, g),

				// Oblique line
				{
					This.Preferences.ObliqueLineWidth: var width,
					Node: ObliqueLineViewNode(var head, _) { Identifier: var identifier, TailCell: var tail }
				} => DrawObliqueLine(calc, head, tail, identifier, width, cw, ch, g),

				// Windoku
				{ Node: WindokuViewNode { Identifier: var identifier } } => DrawWindoku(identifier, calc, cs, g)
			};
		}
	}
}
