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
	/// <param name="offset">
	/// Indicates the offset to set to allow the output items moving a little bit left or right,
	/// in order to correct the position on drawing. The default value is <c>10F</c>, which is specified
	/// in <see cref="DefaultOffset"/>
	/// </param>
	/// <seealso cref="DefaultOffset"/>
	private PointCalculator(scoped in SizeF size, float offset = DefaultOffset)
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


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCell(scoped in PointF point)
		=> point.WithOffset(-Offset) is var (x, y)
		&& x >= 0 && x <= GridSize.Width && y >= 0 && y <= GridSize.Height
		&& CellSize is var (cw, ch)
		&& (int)(y / ch) * 9 + (int)(x / cw) is var result and >= 0 and < 81
			? result
			: -1;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCandidate(scoped in PointF point)
	{
		var (x, y) = point;
		var (cw, ch) = CandidateSize;
		return GetCell(point) * 9 + (int)((y - Offset) / ch) % 3 * 3 + (int)((x - Offset) / cw) % 3;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in Candidates map)
		=> map switch
		{
			[var min, .., var max]
			when GetMousePointInCenter(min / 9, min % 9) is var (x1, y1)
			&& GetMousePointInCenter(max / 9, max % 9) is var (x2, y2)
				=> new((x1 + x2) / 2, (y1 + y2) / 2),
			[var s] => GetMousePointInCenter(s / 9, s % 9),
			_ => throw new ArgumentException("Cannot get at least 1 candidate in the map.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in LockedTarget lockedTarget)
	{
		_ = lockedTarget is { Cells: var cells, Digit: var digit };
		return cells switch
		{
			[var a, .., var b]
			when a * 9 + digit is var min
			&& b * 9 + digit is var max
			&& GetMousePointInCenter(min / 9, min % 9) is var (x1, y1)
			&& GetMousePointInCenter(max / 9, max % 9) is var (x2, y2)
				=> new((x1 + x2) / 2, (y1 + y2) / 2),
			[var s] when s * 9 + digit is var c => GetMousePointInCenter(c / 9, c % 9),
			_ => throw new ArgumentException("Cannot get at least 1 cell in the map.")
		};
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(scoped in Candidates map)
		=> (map, CandidateSize) switch
		{
			([var min, .., var max], var (cw, ch))
				=> RectangleMarshal.CreateInstance(
					GetMousePointInCenter(min / 9, min % 9).WithOffset(-cw / 2, -ch / 2),
					GetMousePointInCenter(max / 9, max % 9).WithOffset(cw / 2, ch / 2)
				),
			_ => throw new ArgumentException("Cannot get at least 1 candidate in the map.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaCell(int cell)
	{
		var (cw, ch) = CellSize;
		var (x, y) = GetMousePointInCenter(cell);
		return new(x - cw / 2, y - ch / 2, cw, ch);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(int cell, int digit)
	{
		var (cw, ch) = CandidateSize;
		var (x, y) = GetMousePointInCenter(cell, digit);
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
			>= 0 and < 9 when (house % 3, house / 3) is (var v1, var v2)
				=> (GridPoints[v1 * 9, v2 * 9], GridPoints[v1 * 9 + 9, v2 * 9 + 9]),
			>= 9 and < 18 when house - 9 is var v => (GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]),
			>= 18 and < 27 when house - 18 is var v => (GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]),
			_ => throw new ArgumentOutOfRangeException(nameof(house))
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell)
	{
		var (cw, ch) = CellSize;
		var (x, y) = GridPoints[cell % 9 * 3, cell / 9 * 3];
		return new(x + cw / 2, y + ch / 2);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell, int digit)
	{
		var (cw, ch) = CandidateSize;
		var (x, y) = GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3];
		return new(x + cw / 2, y + ch / 2);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IPointCalculator CreateConverter(float size, float offset = 10)
		=> new PointCalculator(new(size, size), offset);
}
