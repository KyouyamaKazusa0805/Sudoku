namespace Sudoku.Drawing;

public partial class GridCanvas
{
	/// <summary>
	/// Clear the canvas, removing all drawn elements.
	/// </summary>
	/// <remarks>
	/// This will invalidate drawing items, and draw background, border lines and footer text.
	/// </remarks>
	public partial void Clear()
	{
		DrawBackground();
		DrawBorderLines();
		DrawFooterText();
	}

	/// <summary>
	/// Draw the background.
	/// </summary>
	public partial void DrawBackground() => _g.Clear(_settings.BackgroundColor);

	/// <summary>
	/// Draw border lines onto the canvas.
	/// </summary>
	public partial void DrawBorderLines()
	{
		const int anchorsCountTotal = PointCalculator.AnchorsCount + 1;
		var gridPoints = _calculator.GridPoints;
		using var pg = new Pen(_settings.GridLineColor, _settings.GridLineWidth);
		using var pb = new Pen(_settings.BlockLineColor, _settings.BlockLineWidth);
		for (var i = 0; i < anchorsCountTotal; i += PointCalculator.AnchorsCount / 9)
		{
			_g.DrawLine(pg, gridPoints[i, 0], gridPoints[i, PointCalculator.AnchorsCount]);
			_g.DrawLine(pg, gridPoints[0, i], gridPoints[PointCalculator.AnchorsCount, i]);
		}
		for (var i = 0; i < anchorsCountTotal; i += PointCalculator.AnchorsCount / 3)
		{
			_g.DrawLine(pb, gridPoints[i, 0], gridPoints[i, PointCalculator.AnchorsCount]);
			_g.DrawLine(pb, gridPoints[0, i], gridPoints[PointCalculator.AnchorsCount, i]);
		}
	}

	/// <summary>
	/// Draw footer text onto the canvas.
	/// </summary>
	public partial void DrawFooterText()
	{
		var w = _calculator.Width;
		using var brush = new SolidBrush(_settings.FooterTextColor);
		using var font = GetFooterTextFont(w, _settings);
		var extraHeight = _g.MeasureString(_footerText, font).Height;
		_g.DrawString(_footerText, font, brush, new RectangleF(0, w, w, extraHeight), _stringAligner);
	}

	/// <summary>
	/// Draw grid onto the canvas.
	/// </summary>
	/// <param name="grid">The grid to be drawn.</param>
	public partial void DrawGrid(ref readonly Grid grid)
	{
		if (_settings is not
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
			})
		{
			return;
		}

		var cellWidth = _calculator.CellSize.Width;
		var candidateWidth = _calculator.CandidateSize.Width;
		var vOffsetValue = cellWidth / (PointCalculator.AnchorsCount / 3); // The vertical offset of drawing each value.
		var vOffsetCandidate = candidateWidth / (PointCalculator.AnchorsCount / 3); // The vertical offset of drawing each candidate.
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
			var mask = grid[cell];
			switch (MaskOperations.MaskToCellState(mask))
			{
				case CellState.Undefined when showCandidates: // Draw candidates.
				{
					// This block is use when user draw candidates from undefined grid.
					foreach (var digit in (Mask)(mask & Grid.MaxCandidatesMask))
					{
						var originalPoint = _calculator.GetMousePointInCenter(cell, digit);
						var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
						_g.DrawValue(digit + 1, fCandidate, bCandidate, point, _stringAligner);
					}
					break;
				}
				case CellState.Empty when showCandidates: // Draw candidates.
				{
					foreach (var digit in (Mask)(mask & Grid.MaxCandidatesMask))
					{
						var originalPoint = _calculator.GetMousePointInCenter(cell, digit);
						var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
						_g.DrawValue(digit + 1, fCandidate, bCandidate, point, _stringAligner);
					}
					break;
				}
				case var state and (CellState.Modifiable or CellState.Given):
				{
					// Draw values.
					var originalPoint = _calculator.GetMousePointInCenter(cell);
					var point = originalPoint with { Y = originalPoint.Y + vOffsetValue };
					_g.DrawValue(grid.GetDigit(cell) + 1, f(state, fGiven, fModifiable), f(state, bGiven, bModifiable), point, _stringAligner);
					break;


					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					static T f<T>(CellState state, T given, T modifiable) => state == CellState.Given ? given : modifiable;
				}
			}
		}
	}
}
