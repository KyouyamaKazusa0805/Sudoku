using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using Sudoku.Rendering.Nodes;

namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	/// <summary>
	/// Draw figures.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawFigure(Graphics g)
	{
		if (this is not { View.FigureNodes: var figureNodes, Calculator: { CellSize: var (cw, ch) } calc, Preferences.FigurePadding: var padding })
		{
			return;
		}

		foreach (var figureNode in figureNodes)
		{
			switch (figureNode)
			{
				case (TriangleViewNode or DiamondViewNode or StarViewNode) and (var cell) { Identifier: var identifier }:
				{
					using var brush = new SolidBrush(GetColor(identifier));
					var (x, y) = calc.GetMousePointInCenter(cell);

					using var path = (figureNode switch
					{
						TriangleViewNode => triangle,
						DiamondViewNode => diamond,
						StarViewNode => star,
						_ => default(Func<float, float, GraphicsPath>?)!
					})(x, y);
					g.FillPath(brush, path);

					break;


					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					GraphicsPath triangle(float x, float y)
					{
						var top = new PointF(x, y - MathF.Tan(MathF.PI / 3) / 4 * (cw - 2 * padding));
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

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					GraphicsPath star(float x, float y)
					{
						var angles1 = getAngles(-MathF.PI / 2);
						var angles2 = getAngles(-MathF.PI / 2 + MathF.PI / 5);
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
								result[i] = result[i - 1] + 2 * MathF.PI / 5;
							}

							return result;
						}

						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						static PointF getPoint(float x, float y, float length, float angle)
							=> new(x + length * MathF.Cos(angle), y + length * MathF.Sin(angle));

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
					var (x, y) = calc.GetMousePointInCenter(cell);
					(
						figureNode switch
						{
							SquareViewNode => g.FillRectangle,
							CircleViewNode => g.FillEllipse,
							_ => default(Action<Brush, float, float, float, float>?)!
						}
					)(brush, x - cw / 2 + padding, y - ch / 2 + padding, cw - 2 * padding, ch - 2 * padding);

					break;
				}
				case HeartViewNode(var cell) { Identifier: var identifier }:
				{
					// https://mathworld.wolfram.com/HeartCurve.html
					using var brush = new SolidBrush(GetColor(identifier));

					var center = calc.GetMousePointInCenter(cell);

					// Rotating.
					var oldMatrix = g.Transform;
					using var newMatrix = g.Transform.Clone();
					newMatrix.RotateAt(180, center);

					g.Transform = newMatrix;
					g.FillClosedCurve(brush, getPoints());
					g.Transform = oldMatrix;

					break;


					PointF[] getPoints()
					{
						const Count maxTrialTimes = 360;

						var (centerX, centerY) = center;
						var result = new PointF[maxTrialTimes];
						for (var i = 0; i < maxTrialTimes; i++)
						{
							var t = 2 * MathF.PI / maxTrialTimes * i;
							var x = centerX + 16 * MathF.Pow(MathF.Sin(t), 3) / (32 + 2 * padding) * cw;
							var y = centerY + (13 * MathF.Cos(t) - 5 * MathF.Cos(2 * t) - 2 * MathF.Cos(3 * t) - MathF.Cos(4 * t)) / (32 + 2 * padding) * ch;

							result[i] = new(x, y);
						}

						return result;
					}
				}
			}
		}
	}
}
