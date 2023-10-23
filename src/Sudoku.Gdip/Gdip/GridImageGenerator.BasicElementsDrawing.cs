using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.Concepts;
using Sudoku.Runtime.MaskServices;

namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	/// <summary>
	/// Draw footer text.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawFooterText(Graphics g)
	{
		if (this is not { Width: var w, FooterText: { } text, Preferences.FooterTextColor: var color })
		{
			return;
		}

		using var brush = new SolidBrush(color);
		using var data = GetFooterTextRenderingData();
		var (font, extraHeight, alignment) = data;
		g.DrawString(text, font, brush, new RectangleF(0, w, w, extraHeight), alignment);
	}

	/// <summary>
	/// Draw givens, modifiables and candidates, where the values are specified as a grid.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private unsafe void DrawValue(Graphics g)
	{
		if (this is not
			{
				Puzzle: var puzzle,
				Calculator: { CellSize.Width: var cellWidth, CandidateSize.Width: var candidateWidth } calc,
				Preferences:
				{
					GivenColor: var gColor,
					ModifiableColor: var mColor,
					CandidateColor: var cColor,
					GivenFontName: var gFontName,
					ModifiableFontName: var mFontName,
					CandidateFontName: var cFontName,
					ValueScale: var vScale,
					CandidateScale: var cScale,
					GivenFontStyle: var gFontStyle,
					ModifiableFontStyle: var mFontStyle,
					CandidateFontStyle: var cFontStyle,
					ShowCandidates: var showCandidates
				}
			})
		{
			return;
		}

		var vOffsetValue = cellWidth / (PointCalculator.AnchorsCount / 3); // The vertical offset of rendering each value.
		var vOffsetCandidate = candidateWidth / (PointCalculator.AnchorsCount / 3); // The vertical offset of rendering each candidate.
		var halfWidth = cellWidth / 2F;

		using var bGiven = new SolidBrush(gColor);
		using var bModifiable = new SolidBrush(mColor);
		using var bCandidate = new SolidBrush(cColor);
		using var bCandidateLighter = new SolidBrush(cColor.QuarterAlpha());
		using var fGiven = GetFont(gFontName, halfWidth, vScale, gFontStyle);
		using var fModifiable = GetFont(mFontName, halfWidth, vScale, mFontStyle);
		using var fCandidate = GetFont(cFontName, halfWidth, cScale, cFontStyle);

		for (var cell = 0; cell < 81; cell++)
		{
			var mask = puzzle[cell];
			switch (MaskOperations.MaskToCellState(mask))
			{
				case CellState.Undefined when showCandidates:
				{
					// Draw candidates.
					// This block is use when user draw candidates from undefined grid.
					var overlaps = View?.UnknownOverlaps(cell) ?? false;
					foreach (var digit in (Mask)(mask & Grid.MaxCandidatesMask))
					{
						var originalPoint = calc.GetMousePointInCenter(cell, digit);
						var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
						g.DrawValue(digit + 1, fCandidate, overlaps ? bCandidateLighter : bCandidate, point, StringLocating);
					}

					break;
				}
				case CellState.Empty when showCandidates:
				{
					// Draw candidates.
					var overlaps = View?.UnknownOverlaps(cell) ?? false;
					foreach (var digit in (Mask)(mask & Grid.MaxCandidatesMask))
					{
						var originalPoint = calc.GetMousePointInCenter(cell, digit);
						var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
						g.DrawValue(digit + 1, fCandidate, overlaps ? bCandidateLighter : bCandidate, point, StringLocating);
					}

					break;
				}
				case var state and (CellState.Modifiable or CellState.Given):
				{
					// Draw values.
					var originalPoint = calc.GetMousePointInCenter(cell);
					var point = originalPoint with { Y = originalPoint.Y + vOffsetValue };
					g.DrawValue(puzzle.GetDigit(cell) + 1, f(state, fGiven, fModifiable), f(state, bGiven, bModifiable), point, StringLocating);

					break;


					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					static T f<T>(CellState state, T given, T modifiable) => state == CellState.Given ? given : modifiable;
				}
			}
		}
	}

	/// <summary>
	/// Draw the background, where the color is specified in <see cref="DrawingConfigurations.BackgroundColor"/>.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	/// <seealso cref="DrawingConfigurations.BackgroundColor"/>
	private void DrawBackground(Graphics g)
	{
		if (this is not { Preferences.BackgroundColor: var backColor })
		{
			return;
		}

		g.Clear(backColor);
	}

	/// <summary>
	/// Draw grid lines and block lines.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawGridAndBlockLines(Graphics g)
	{
		if (this is not
			{
				Calculator.GridPoints: var pts,
				Preferences:
				{
					GridLineColor: var gridLineColor,
					BlockLineColor: var blockLineColor,
					GridLineWidth: var gridLineWidth,
					BlockLineWidth: var blockLineWidth
				}
			})
		{
			return;
		}

		using var pg = new Pen(gridLineColor, gridLineWidth);
		using var pb = new Pen(blockLineColor, blockLineWidth);

		const int length = PointCalculator.AnchorsCount + 1;
		for (var i = 0; i < length; i += PointCalculator.AnchorsCount / 9)
		{
			g.DrawLine(pg, pts[i, 0], pts[i, PointCalculator.AnchorsCount]);
			g.DrawLine(pg, pts[0, i], pts[PointCalculator.AnchorsCount, i]);
		}

		for (var i = 0; i < length; i += PointCalculator.AnchorsCount / 3)
		{
			g.DrawLine(pb, pts[i, 0], pts[i, PointCalculator.AnchorsCount]);
			g.DrawLine(pb, pts[0, i], pts[PointCalculator.AnchorsCount, i]);
		}
	}
}
