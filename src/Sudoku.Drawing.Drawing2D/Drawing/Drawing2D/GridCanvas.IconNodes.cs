namespace Sudoku.Drawing.Drawing2D;

public partial class GridCanvas
{
	/// <summary>
	/// Draw icon view nodes onto the canvas.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	public partial void DrawIconViewNodes(ReadOnlySpan<IconViewNode> nodes)
	{
		var (cw, ch) = _calculator.CellSize;
		var padding = Settings.FigurePadding;
		foreach (var figureNode in nodes)
		{
			switch (figureNode)
			{
				case (TriangleViewNode or DiamondViewNode or StarViewNode) and (var cell) { Identifier: var identifier }:
				{
					using var brush = new SolidBrush(GetColor(identifier));
					var (x, y) = _calculator.GetMousePointInCenter(cell);
					Func<float, float, GraphicsPath> pathCreator = figureNode switch
					{
						TriangleViewNode => triangle,
						DiamondViewNode => diamond,
						StarViewNode => star
					};
					using var path = pathCreator(x, y);
					_g.FillPath(brush, path);
					break;


					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					GraphicsPath triangle(float x, float y)
					{
						var top = new PointF(x, y - Tan(PI / 3) / 4 * (cw - 2 * padding));
						var left = new PointF(x - (cw - 2 * padding) / 2, y - ch / 2 + ch - padding);
						var right = new PointF(x + (cw - 2 * padding) / 2, y - ch / 2 + ch - padding);
						var path = new GraphicsPath();
						path.AddLine(top, left);
						path.AddLine(left, right);
						path.AddLine(right, top);
						return path;
					}

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					GraphicsPath diamond(float x, float y)
					{
						var p1 = new PointF(x, y - ch / 2 + padding);
						var p2 = new PointF(x - cw / 2 + padding, y);
						var p3 = new PointF(x + cw / 2 - padding, y);
						var p4 = new PointF(x, y + ch / 2 - padding);
						var path = new GraphicsPath();
						path.AddLine(p1, p3);
						path.AddLine(p3, p4);
						path.AddLine(p4, p2);
						path.AddLine(p2, p1);
						return path;
					}

					GraphicsPath star(float x, float y)
					{
						var angles1 = getAngles(-PI / 2);
						var angles2 = getAngles(-PI / 2 + PI / 5);
						var points1 = getPoints(x, y, cw / 2 - padding, angles1);
						var points2 = getPoints(x, y, (ch / 2 - padding) / 2, angles2);
						var points = new PointF[points1.Length + points2.Length];
						for (var (i, j) = (0, 0); i < points.Length; i += 2, j++)
						{
							points[i] = points1[j];
							points[i + 1] = points2[j];
						}

						var path = new GraphicsPath();
						for (var i = 0; i < points.Length - 1; i++)
						{
							path.AddLine(points[i], points[i + 1]);
						}
						path.AddLine(points[^1], points[0]);
						return path;


						static float[] getAngles(float startAngle)
						{
							var result = (float[])[startAngle, default, default, default, default];
							for (var i = 1; i < 5; i++)
							{
								result[i] = result[i - 1] + 2 * PI / 5;
							}
							return result;
						}

						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						static PointF getPoint(float x, float y, float length, float angle)
							=> new(x + length * Cos(angle), y + length * Sin(angle));

						static PointF[] getPoints(float x, float y, float length, params float[] angles)
						{
							var result = new PointF[angles.Length];
							for (var i = 0; i < result.Length; i++)
							{
								result[i] = getPoint(x, y, length, angles[i]);
							}
							return result;
						}
					}
				}
				case (SquareViewNode or CircleViewNode) and (var cell) { Identifier: var identifier }:
				{
					using var brush = new SolidBrush(GetColor(identifier));
					var (x, y) = _calculator.GetMousePointInCenter(cell);
					Action<Brush, float, float, float, float> drawingAction = figureNode switch
					{
						SquareViewNode => _g.FillRectangle,
						CircleViewNode => _g.FillEllipse
					};
					drawingAction(brush, x - cw / 2 + padding, y - ch / 2 + padding, cw - 2 * padding, ch - 2 * padding);
					break;
				}
				case CrossViewNode(var cell) { Identifier: var identifier }:
				{
					using var pen = new Pen(GetColor(identifier), 6);
					var rectangle = _calculator.GetMouseRectangleViaCell(cell);
					_g.DrawCrossSign(pen, rectangle);
					break;
				}
				case HeartViewNode(var cell) { Identifier: var identifier }:
				{
					using var brush = new SolidBrush(GetColor(identifier));
					var center = _calculator.GetMousePointInCenter(cell);
					var oldMatrix = _g.Transform;
					using var newMatrix = _g.Transform.Clone();
					newMatrix.RotateAt(180, center);
					_g.Transform = newMatrix;
					_g.FillClosedCurve(brush, getPoints());
					_g.Transform = oldMatrix;
					break;


					PointF[] getPoints()
					{
						// See: https://mathworld.wolfram.com/HeartCurve.html
						const int maxTrialTimes = 360;

						var (centerX, centerY) = center;
						var result = new PointF[maxTrialTimes];
						for (var i = 0; i < maxTrialTimes; i++)
						{
							var t = 2 * PI / maxTrialTimes * i;
							var x = centerX + 16 * Pow(Sin(t), 3) / (32 + 2 * padding) * cw;
							var y = centerY + (13 * Cos(t) - 5 * Cos(2 * t) - 2 * Cos(3 * t) - Cos(4 * t)) / (32 + 2 * padding) * ch;
							result[i] = new(x, y);
						}
						return result;
					}
				}
			}
		}
	}
}
