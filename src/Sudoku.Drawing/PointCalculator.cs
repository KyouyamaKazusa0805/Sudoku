namespace Sudoku.Drawing;

using static IPointCalculator;

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
		=> point with { X = point.X - Offset, Y = point.Y - Offset } switch
		{
			var (x, y) => (x >= 0 && x <= GridSize.Width && y >= 0 && y <= GridSize.Height) switch
			{
				true => CellSize switch
				{
					var (cw, ch) => (int)(y / ch) * 9 + (int)(x / cw) switch
					{
						var result and >= 0 and < 81 => result,
						_ => -1
					}
				},
				_ => -1
			}
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCandidate(scoped in PointF point)
		=> (point, CandidateSize) switch
		{
			var ((x, y), (cw, ch)) => ((int)((y - Offset) / ch), (int)((x - Offset) / cw)) switch
			{
				var (a, b) => GetCell(point) * 9 + a % 3 * 3 + b % 3
			}
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in Candidates map)
		=> map switch
		{
			[var min, .., var max] => (min / 9, min % 9, max / 9, max % 9) switch
			{
				var (cMin, dMin, cMax, dMax) => (GetMousePointInCenter(cMin, dMin), GetMousePointInCenter(cMax, dMax)) switch
				{
					var ((x1, y1), (x2, y2)) => new((x1 + x2) / 2, (y1 + y2) / 2)
				}
			},
			[var s] => GetMousePointInCenter(s / 9, s % 9),
			_ => throw new ArgumentException("Cannot get at least 1 candidate in the map.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMouseCenter(scoped in LockedTarget lockedTarget)
		=> lockedTarget switch
		{
			{ Cells: var cells, Digit: var digit } => cells switch
			{
				[var a, .., var b] => (a * 9 + digit, b * 9 + digit) switch
				{
					var (min, max) => (GetMousePointInCenter(min / 9, min % 9), GetMousePointInCenter(max / 9, max % 9)) switch
					{
						var ((x1, y1), (x2, y2)) => new((x1 + x2) / 2, (y1 + y2) / 2)
					}
				},
				[var s] => (s * 9 + digit) switch { var c => GetMousePointInCenter(c / 9, c % 9) },
				_ => throw new ArgumentException("Cannot get at least 1 cell in the map.")
			}
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(scoped in Candidates map)
		=> (map, CandidateSize) switch
		{
			([var min, .., var max], var (cw, ch)) => (min / 9, min % 9, max / 9, max % 9) switch
			{
				var (cMin, dMin, cMax, dMax) => (GetMousePointInCenter(cMin, dMin), GetMousePointInCenter(cMax, dMax)) switch
				{
					var (a, b) => (
						a with { X = a.X - cw / 2, Y = a.Y - ch / 2 },
						b with { X = b.X + cw / 2, Y = b.Y + ch / 2 }
					) switch
					{
						var (p, q) => RectangleMarshal.CreateInstance(p, q)
					}
				}
			},
			_ => throw new ArgumentException("Cannot get at least 1 candidate in the map.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaCell(int cell)
		=> (this, GetMousePointInCenter(cell)) switch
		{
			({ CellSize: var (cw, ch) }, var (x, y)) => new(x - cw / 2, y - ch / 2, cw, ch)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangle(int cell, int digit)
		=> (this, GetMousePointInCenter(cell, digit)) switch
		{
			({ CandidateSize: var (cw, ch) }, var (x, y)) => new(x - cw / 2, y - ch / 2, cw, ch)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectangleF GetMouseRectangleViaHouse(int house)
		=> GetAnchorsViaHouse(house) switch { var (l, r) => RectangleMarshal.CreateInstance(l, r) };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (PointF LeftUp, PointF RightDown) GetAnchorsViaHouse(int house)
		=> house switch
		{
			>= 0 and < 9 => (house % 3, house / 3) switch
			{
				var (v1, v2) => (GridPoints[v1 * 9, v2 * 9], GridPoints[v1 * 9 + 9, v2 * 9 + 9])
			},
			>= 9 and < 18 => (house - 9) switch { var v => (GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]) },
			>= 18 and < 27 => (house - 18) switch { var v => (GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]) },
			_ => throw new ArgumentOutOfRangeException(nameof(house))
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell)
		=> (this, GridPoints[cell % 9 * 3, cell / 9 * 3]) switch
		{
			({ CellSize: var (cw, ch) }, var (x, y)) => new(x + cw / 2, y + ch / 2)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PointF GetMousePointInCenter(int cell, int digit)
		=> (this, GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3]) switch
		{
			({ CandidateSize: var (cw, ch) }, var (x, y)) => new(x + cw / 2, y + ch / 2)
		};


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IPointCalculator CreateConverter(float size, float offset = 10)
		=> new PointCalculator(new(size, size), offset);
}
