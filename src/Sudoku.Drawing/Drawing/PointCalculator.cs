using static Sudoku.Drawing.IPointCalculator;

namespace Sudoku.Drawing;

/// <summary>
/// Defines a point calculator that interacts with the UI projects.
/// </summary>
public sealed class PointCalculator : IPointCalculator
{
	/// <summary>
	/// Initializes a <see cref="PointCalculator"/> instance with
	/// the specified picture size instance of type <see cref="SizeF"/>, and an offset.
	/// </summary>
	/// <param name="size">The <see cref="SizeF"/> instance.</param>
	private PointCalculator(in SizeF size, float offset = DefaultOffset)
	{
		// Initialize sizes.
		ControlSize = size;

		var (width, height) = size;
		Width = width;
		Height = height;
		Offset = offset;

		float gridWidth = width - Offset / 2, gridHeight = height - Offset / 2;
		GridSize = new(gridWidth, gridHeight);
		CellSize = new(gridWidth / (AnchorsCount / 3), gridHeight / (AnchorsCount / 3));
		CandidateSize = new(gridWidth / AnchorsCount, gridHeight / AnchorsCount);

		// Initialize points.
		const int length = AnchorsCount + 1;
		var (cw, ch) = CandidateSize;
		GridPoints = new PointF[length, length];
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
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


	/// <summary>
	/// Get the focus cell offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The cell offset. Returns -1 when the current point is invalid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCell(in PointF point)
	{
		var (x, y) = point.WithOffset(-Offset);
		if (x < 0 || x > GridSize.Width || y < 0 || y > GridSize.Height)
		{
			// Invalid case.
			return -1;
		}

		var (cw, ch) = CellSize;
		int result = (int)(y / ch) * 9 + (int)(x / cw);
		return result is >= 0 and < 81 ? result : -1;
	}

	/// <summary>
	/// Get the focus candidate offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The candidate offset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCandidate(in PointF point)
	{
		var (x, y) = point;
		var (cw, ch) = CandidateSize;
		return GetCell(point) * 9 + (int)((y - Offset) / ch) % 3 * 3 + (int)((x - Offset) / cw) % 3;
	}

	/// <summary>
	/// Get the center mouse point of all candidates.
	/// </summary>
	/// <param name="map">The map of candidates.</param>
	/// <returns>The center mouse point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(in Candidates map)
	{
		int min = map[0], max = map[^1];
		var (x1, y1) = GetMousePointInCenter(min / 9, min % 9);
		var (x2, y2) = GetMousePointInCenter(max / 9, max % 9);
		return new((x1 + x2) / 2, (y1 + y2) / 2);
	}

	/// <summary>
	/// Get the rectangle from all candidates.
	/// </summary>
	/// <param name="map">The candidates.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(in Candidates map)
	{
		var (cw, ch) = CandidateSize;
		int min = map[0], max = map[^1];
		var pt1 = GetMousePointInCenter(min / 9, min % 9).WithOffset(-cw / 2, -ch / 2);
		var pt2 = GetMousePointInCenter(max / 9, max % 9).WithOffset(cw / 2, ch / 2);
		return RectangleMarshal.CreateInstance(pt1, pt2);
	}

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaCell(int cell)
	{
		var (cw, ch) = CellSize;
		var (x, y) = GetMousePointInCenter(cell);
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <summary>
	/// Get the rectangle (4 mouse points) for the specified cell
	/// and digit of a candidate.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(int cell, int digit)
	{
		var (cw, ch) = CandidateSize;
		var (x, y) = GetMousePointInCenter(cell, digit);
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified region.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <returns>The rectangle.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the region is less than 0 or greater than 26.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaRegion(int region)
	{
		var (l, r) = GetAnchorsViaRegion(region);
		return RectangleMarshal.CreateInstance(l, r);
	}

	/// <summary>
	/// Gets two points that specifies and represents the anchors of this region.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <returns>The anchor points.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (PointF LeftUp, PointF RightDown) GetAnchorsViaRegion(int region) => region switch
	{
		>= 0 and < 9 when (region % 3, region / 3) is (var v1, var v2) =>
			(GridPoints[v1 * 9, v2 * 9], GridPoints[v1 * 9 + 9, v2 * 9 + 9]),
		>= 9 and < 18 when region - 9 is var v => (GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]),
		>= 18 and < 27 when region - 18 is var v => (GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]),
		_ => throw new ArgumentOutOfRangeException(nameof(region))
	};

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <returns>The mouse point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell)
	{
		var (cw, ch) = CellSize;
		var (x, y) = GridPoints[cell % 9 * 3, cell / 9 * 3];
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
		var (cw, ch) = CandidateSize;
		var (x, y) = GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3];
		return new(x + cw / 2, y + ch / 2);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IPointCalculator CreateConverter(float size, float offset = 10) =>
		new PointCalculator(new(size, size), offset);
}
