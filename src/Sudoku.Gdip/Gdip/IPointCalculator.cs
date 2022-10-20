namespace Sudoku.Gdip;

/// <summary>
/// Provides a serial of methods for a point calculator that interacts with the UI projects.
/// </summary>
public interface IPointCalculator
{
	/// <summary>
	/// Indicates the default offset value.
	/// </summary>
	protected internal const float DefaultOffset = 10F;

	/// <summary>
	/// Indicates the number of anchors hold per house.
	/// </summary>
	/// <remarks>
	/// The sudoku grid painter will draw the outlines and the inner lines, and correct the point
	/// of each digits (candidates also included). Each row or column always contains 27 candidates,
	/// so this value is 27.
	/// </remarks>
	protected internal const int AnchorsCount = 27;


	/// <summary>
	/// Indicates the width of the picture to draw.
	/// </summary>
	public abstract float Width { get; }

	/// <summary>
	/// Indicates the height of the picture to draw.
	/// </summary>
	public abstract float Height { get; }

	/// <summary>
	/// Indicates the offset of the gap between the picture box outline and the sudoku grid outline.
	/// </summary>
	public abstract float Offset { get; }

	/// <summary>
	/// Indicates the control size.
	/// </summary>
	public abstract SizeF ControlSize { get; }

	/// <summary>
	/// Indicates the grid size.
	/// </summary>
	public abstract SizeF GridSize { get; }

	/// <summary>
	/// Indicates the cell size.
	/// </summary>
	public abstract SizeF CellSize { get; }

	/// <summary>
	/// Indicates the candidate size.
	/// </summary>
	public abstract SizeF CandidateSize { get; }

	/// <summary>
	/// Indicates the absolutely points in grid cross-lines.
	/// This property will be assigned later (and not <see langword="null"/>).
	/// </summary>
	/// <remarks>Note that the size of this 2D array is always 28 by 28.</remarks>
	public abstract PointF[,] GridPoints { get; }


	/// <summary>
	/// Get the focus cell offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The cell offset. Returns -1 when the current point is invalid.</returns>
	public abstract int GetCell(scoped in PointF point);

	/// <summary>
	/// Get the focus candidate offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The candidate offset.</returns>
	public abstract int GetCandidate(scoped in PointF point);

	/// <summary>
	/// Get the center mouse point of all candidates.
	/// </summary>
	/// <param name="map">The map of candidates.</param>
	/// <returns>The center mouse point.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	public abstract PointF GetMouseCenter(scoped in Candidates map);

	/// <summary>
	/// Gets the center mouse point of the specified locked target.
	/// </summary>
	/// <param name="lockedTarget">The locked target.</param>
	/// <returns>The center mouse point.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	public abstract PointF GetMouseCenter(scoped in LockedTarget lockedTarget);

	/// <summary>
	/// Get the rectangle from all candidates.
	/// </summary>
	/// <param name="map">The candidates.</param>
	/// <returns>The rectangle.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	public abstract RectangleF GetMouseRectangle(scoped in Candidates map);

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The rectangle.</returns>
	public abstract RectangleF GetMouseRectangleViaCell(int cell);

	/// <summary>
	/// Get the rectangle (4 mouse points) for the specified cell and digit of a candidate.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The rectangle.</returns>
	public abstract RectangleF GetMouseRectangle(int cell, int digit);

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The rectangle.</returns>
	public abstract RectangleF GetMouseRectangleViaHouse(int house);

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <returns>The mouse point.</returns>
	public abstract PointF GetMousePointInCenter(int cell);

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset and the digit.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The mouse point.</returns>
	public abstract PointF GetMousePointInCenter(int cell, int digit);

	/// <summary>
	/// Gets two points that specifies and represents the anchors of this house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The anchor points.</returns>
	public abstract (PointF LeftUp, PointF RightDown) GetAnchorsViaHouse(int house);


	/// <summary>
	/// Creates a <see cref="IPointCalculator"/> instance with the specified size, and the offset.
	/// </summary>
	/// <param name="size">The size.</param>
	/// <param name="offset">The offset. The default value is <c>10</c>.</param>
	/// <returns>An <see cref="IPointCalculator"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sealed IPointCalculator Create(float size, float offset = 10) => new PointCalculator(new(size, size), offset);
}

/// <summary>
/// Defines a point calculator that interacts with the UI projects.
/// </summary>
file sealed class PointCalculator : IPointCalculator
{
	/// <summary>
	/// Initializes a <see cref="PointCalculator"/> instance with
	/// the specified picture size instance of type <see cref="SizeF"/>, and an offset.
	/// </summary>
	/// <param name="size">The <see cref="SizeF"/> instance.</param>
	/// <param name="offset">
	/// Indicates the offset to set to allow the output items moving a little bit left or right,
	/// in order to correct the position on drawing. The default value is <c>10F</c>, which is specified
	/// in <see cref="DefaultOffset"/>
	/// </param>
	/// <seealso cref="DefaultOffset"/>
	internal PointCalculator(scoped in SizeF size, float offset = DefaultOffset)
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


	/// <inheritdoc/>
	public float Width { get; }

	/// <inheritdoc/>
	public float Height { get; }

	/// <inheritdoc/>
	/// <remarks>The default value is <c>10</c>.</remarks>
	public float Offset { get; } = DefaultOffset;

	/// <inheritdoc/>
	public SizeF ControlSize { get; }

	/// <inheritdoc/>
	public SizeF GridSize { get; }

	/// <inheritdoc/>
	public SizeF CellSize { get; }

	/// <inheritdoc/>
	public SizeF CandidateSize { get; }

	/// <inheritdoc/>
	public PointF[,] GridPoints { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCell(scoped in PointF point)
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCandidate(scoped in PointF point)
	{
		var ((x, y), (cw, ch)) = (point, CandidateSize);
		var (a, b) = ((int)((y - Offset) / ch), (int)((x - Offset) / cw));
		return GetCell(point) * 9 + a % 3 * 3 + b % 3;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in Candidates map)
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
		};
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(scoped in Candidates map)
	{
		switch (map, CandidateSize)
		{
			case ([var min, .., var max], var (cw, ch)):
			{
				var (cMin, dMin, cMax, dMax) = (min / 9, min % 9, max / 9, max % 9);
				var (a, b) = (GetMousePointInCenter(cMin, dMin), GetMousePointInCenter(cMax, dMax));
				var (p, q) = (a with { X = a.X - cw / 2, Y = a.Y - ch / 2 }, b with { X = b.X + cw / 2, Y = b.Y + ch / 2 });
				return RectangleMarshal.CreateInstance(p, q);
			}
			default:
			{
				throw new ArgumentException("Cannot get at least 1 candidate in the map.");
			}
		};
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaCell(int cell)
	{
		var ((cw, ch), (x, y)) = (CellSize, GetMousePointInCenter(cell));
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(int cell, int digit)
	{
		var ((cw, ch), (x, y)) = (CandidateSize, GetMousePointInCenter(cell, digit));
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaHouse(int house)
	{
		var (l, r) = GetAnchorsViaHouse(house);
		return RectangleMarshal.CreateInstance(l, r);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (PointF LeftUp, PointF RightDown) GetAnchorsViaHouse(int house)
		=> house switch
		{
			>= 0 and < 9 when (house % 3, house / 3) is var (v1, v2) => (GridPoints[v1 * 9, v2 * 9], GridPoints[v1 * 9 + 9, v2 * 9 + 9]),
			>= 9 and < 18 when house - 9 is var v => (GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]),
			>= 18 and < 27 when house - 18 is var v => (GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]),
			_ => throw new ArgumentOutOfRangeException(nameof(house))
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell)
	{
		var ((cw, ch), (x, y)) = (CellSize, GridPoints[cell % 9 * 3, cell / 9 * 3]);
		return new(x + cw / 2, y + ch / 2);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell, int digit)
	{
		var ((cw, ch), (x, y)) = (CandidateSize, GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3]);
		return new(x + cw / 2, y + ch / 2);
	}
}
