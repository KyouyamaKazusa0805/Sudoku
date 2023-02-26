namespace Sudoku.Gdip;

/// <summary>
/// Provides a serial of methods for a point calculator that interacts with the UI projects.
/// </summary>
public sealed class PointCalculator
{
	/// <summary>
	/// Initializes a <see cref="PointCalculator"/> instance via the specified picture size and offset value.
	/// </summary>
	/// <param name="size">The size of the picture.</param>
	/// <param name="offset">The offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointCalculator(float size, float offset = DefaultOffset) : this(new SizeF(size, size), offset)
	{
	}

	/// <summary>
	/// Initializes a <see cref="PointCalculator"/> instance with the specified picture size instance and offset value.
	/// </summary>
	/// <param name="size">The <see cref="SizeF"/> instance.</param>
	/// <param name="offset">
	/// Indicates the offset to set to allow the output items moving a little bit left or right,
	/// in order to correct the position on drawing. The default value is <c>10F</c>, which is specified
	/// in <see cref="DefaultOffset"/>
	/// </param>
	/// <seealso cref="DefaultOffset"/>
	private PointCalculator(scoped in SizeF size, float offset)
	{
		// Initialize sizes.
		ControlSize = size;

		var (width, height) = size;
		Width = width;
		Height = height;
		Offset = offset;

		var gridWidth = width - Offset * 2;
		var gridHeight = height - Offset * 2;
		GridSize = new(gridWidth, gridHeight);
		CellSize = new(gridWidth / (AnchorsCount / 3), gridHeight / (AnchorsCount / 3));
		CandidateSize = new(gridWidth / AnchorsCount, gridHeight / AnchorsCount);

		// Initialize points.
		const int length = AnchorsCount + 1;
		var (cw, ch) = CandidateSize;
		GridPoints = new PointF[length, length];
		for (var i = 0; i < length; i++)
		{
			for (var j = 0; j < length; j++)
			{
				GridPoints[i, j] = new(cw * i + Offset, ch * j + Offset);
			}
		}
	}


	/// <summary>
	/// Indicates the width of the picture to draw.
	/// </summary>
	public float Width { get; }

	/// <summary>
	/// Indicates the height of the picture to draw.
	/// </summary>
	public float Height { get; }

	/// <summary>
	/// Indicates the offset of the gap between the picture box outline and the sudoku grid outline.
	/// </summary>
	/// <remarks>The default value is <c>10</c>.</remarks>
	public float Offset { get; } = DefaultOffset;

	/// <summary>
	/// Indicates the control size.
	/// </summary>
	public SizeF ControlSize { get; }

	/// <summary>
	/// Indicates the grid size.
	/// </summary>
	public SizeF GridSize { get; }

	/// <summary>
	/// Indicates the cell size.
	/// </summary>
	public SizeF CellSize { get; }

	/// <summary>
	/// Indicates the candidate size.
	/// </summary>
	public SizeF CandidateSize { get; }

	/// <summary>
	/// Indicates the absolutely points in grid cross-lines.
	/// This property will be assigned later (and not <see langword="null"/>).
	/// </summary>
	/// <remarks>Note that the size of this 2D array is always 28 by 28.</remarks>
	public PointF[,] GridPoints { get; }


	/// <summary>
	/// Get the focus cell offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The cell offset. Returns -1 when the current point is invalid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCell(PointF point)
	{
		var (x, y) = point with { X = point.X - Offset, Y = point.Y - Offset };
		if (x < 0 || x > GridSize.Width || y < 0 || y > GridSize.Height)
		{
			return -1;
		}

		var (cw, ch) = CellSize;
		var result = (int)(y / ch) * 9 + (int)(x / cw);
		if (result is not >= 0 or not < 81)
		{
			return -1;
		}

		return result;
	}

	/// <summary>
	/// Get the focus candidate offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The candidate offset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCandidate(PointF point)
	{
		var ((x, y), (cw, ch)) = (point, CandidateSize);
		var (a, b) = ((int)((y - Offset) / ch), (int)((x - Offset) / cw));
		return GetCell(point) * 9 + a % 3 * 3 + b % 3;
	}

	/// <summary>
	/// Get the center mouse point of all candidates.
	/// </summary>
	/// <param name="map">The map of candidates.</param>
	/// <returns>The center mouse point.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in CandidateMap map)
	{
		switch (map)
		{
			case [var min, .., var max]:
			{
				var (cMin, dMin, cMax, dMax) = (min / 9, min % 9, max / 9, max % 9);
				var ((x1, y1), (x2, y2)) = (GetMousePointInCenter(cMin, dMin), GetMousePointInCenter(cMax, dMax));
				return new((x1 + x2) / 2, (y1 + y2) / 2);
			}
			case [var s]:
			{
				return GetMousePointInCenter(s / 9, s % 9);
			}
			default:
			{
				throw new ArgumentException("Cannot get at least 1 candidate in the map.");
			}
		}
	}

	/// <summary>
	/// Gets the center mouse point of the specified locked target.
	/// </summary>
	/// <param name="lockedTarget">The locked target.</param>
	/// <returns>The center mouse point.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in LockedTarget lockedTarget)
	{
		switch (lockedTarget)
		{
			case { Cells: [var a, .., var b], Digit: var digit }:
			{
				var (min, max) = (a * 9 + digit, b * 9 + digit);
				var ((x1, y1), (x2, y2)) = (GetMousePointInCenter(min / 9, min % 9), GetMousePointInCenter(max / 9, max % 9));
				return new((x1 + x2) / 2, (y1 + y2) / 2);
			}
			case { Cells: [var s], Digit: var digit }:
			{
				var c = s * 9 + digit;
				return GetMousePointInCenter(c / 9, c % 9);
			}
			default:
			{
				throw new ArgumentException("Cannot get at least 1 cell in the map.");
			}
		}
	}

	/// <summary>
	/// Get the rectangle from all candidates.
	/// </summary>
	/// <param name="map">The candidates.</param>
	/// <returns>The rectangle.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(scoped in CandidateMap map)
	{
		switch (map, CandidateSize)
		{
			case ([var min, .., var max], var (cw, ch)):
			{
				var (cMin, dMin, cMax, dMax) = (min / 9, min % 9, max / 9, max % 9);
				var (a, b) = (GetMousePointInCenter(cMin, dMin), GetMousePointInCenter(cMax, dMax));
				var (p, q) = (a with { X = a.X - cw / 2, Y = a.Y - ch / 2 }, b with { X = b.X + cw / 2, Y = b.Y + ch / 2 });
				return RectangleCreator.Create(p, q);
			}
			default:
			{
				throw new ArgumentException("Cannot get at least 1 candidate in the map.");
			}
		}
	}

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaCell(int cell)
	{
		var ((cw, ch), (x, y)) = (CellSize, GetMousePointInCenter(cell));
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <summary>
	/// Get the rectangle (4 mouse points) for the specified cell and digit of a candidate.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(int cell, int digit)
	{
		var ((cw, ch), (x, y)) = (CandidateSize, GetMousePointInCenter(cell, digit));
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaHouse(int house)
	{
		var (l, r) = GetAnchorsViaHouse(house);
		return RectangleCreator.Create(l, r);
	}

	/// <summary>
	/// Gets two points that specifies and represents the anchors of this house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The anchor points.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (PointF TopLeft, PointF BottomRight) GetAnchorsViaHouse(int house)
		=> house switch
		{
			>= 0 and < 9 when (house % 3, house / 3) is var (v1, v2) => (GridPoints[v1 * 9, v2 * 9], GridPoints[v1 * 9 + 9, v2 * 9 + 9]),
			>= 9 and < 18 when house - 9 is var v => (GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]),
			>= 18 and < 27 when house - 18 is var v => (GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]),
			_ => throw new ArgumentOutOfRangeException(nameof(house))
		};

	/// <summary>
	/// Gets two points specifies and represents a line as shared border grid lines between two adjacent cells.
	/// </summary>
	/// <param name="cell1">The first cell.</param>
	/// <param name="cell2">The second cell that is adjacent with <paramref name="cell1"/> by row or column.</param>
	/// <returns>The two points representing with a line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (PointF Start, PointF End) GetSharedLinePosition(int cell1, int cell2) => GetSharedLinePosition(cell1, cell2, false);

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <returns>The mouse point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell)
	{
		var ((cw, ch), (x, y)) = (CellSize, GridPoints[cell % 9 * 3, cell / 9 * 3]);
		return new(x + cw / 2, y + ch / 2);
	}

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset and the digit.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The mouse point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell, int digit)
	{
		var ((cw, ch), (x, y)) = (CandidateSize, GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3]);
		return new(x + cw / 2, y + ch / 2);
	}

	/// <inheritdoc cref="GetSharedLinePosition(int, int)"/>
	/// <summary>
	/// <inheritdoc path="/summary"/>
	/// </summary>
	/// <param name="cell1"><inheritdoc path="/param[@name='cell1']"/></param>
	/// <param name="cell2"><inheritdoc path="/param[@name='cell2']"/></param>
	/// <param name="borderBarFullyOverlapsGridLine">
	/// <inheritdoc cref="DrawingConfigurations.BorderBarFullyOverlapsGridLine" path="/summary"/>
	/// </param>
	/// <returns><inheritdoc path="/returns"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal (PointF Start, PointF End) GetSharedLinePosition(int cell1, int cell2, bool borderBarFullyOverlapsGridLine)
	{
		var ((x, y), (cw, ch)) = GetMouseRectangleViaCell(cell2);
		return (cell2 - cell1 == 1, borderBarFullyOverlapsGridLine) switch
		{
			(true, false) => (new(x, y + ch * .2F), new(x, y + ch * .8F)),
			(false, false) => (new(x + cw * .2F, y), new(x + cw * .8F, y)),
			(true, true) => (new(x, y), new(x, y + ch)),
			(false, true) => (new(x, y), new(x + cw, y))
		};
	}
}
