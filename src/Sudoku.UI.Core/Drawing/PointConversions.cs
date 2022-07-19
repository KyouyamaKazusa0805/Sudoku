namespace Sudoku.UI.Drawing;

/// <summary>
/// Stores a set of methods on handling point conversions.
/// </summary>
internal static class PointConversions
{
	/// <summary>
	/// Gets the grid size from the specified pane size and the outside offset.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset value. The value must be a positive value.</param>
	/// <returns>The grid size.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double GridSize(double paneSize, double outsideOffset) => paneSize - 2 * outsideOffset;

	/// <summary>
	/// Gets the block size from the specified pane size and the outside offset.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset value. The value must be a positive value.</param>
	/// <returns>The block size.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double BlockSize(double paneSize, double outsideOffset) => GridSize(paneSize, outsideOffset) / 3;

	/// <summary>
	/// Gets the cell size from the specified pane size and the outside offset.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset value. The value must be a positive value.</param>
	/// <returns>The cell size.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CellSize(double paneSize, double outsideOffset) => GridSize(paneSize, outsideOffset) / 9;

	/// <summary>
	/// Gets the candidate size from the specified pane size and the outside offset.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset value. The value must be a positive value.</param>
	/// <returns>The candidate size.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CandidateSize(double paneSize, double outsideOffset) => GridSize(paneSize, outsideOffset) / 27;

	/// <summary>
	/// Gets the top left cursor point of the specified locked target.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="lockedTarget">The locked target.</param>
	/// <returns>The top left cursor point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point GetMouseTopLeft(double paneSize, double outsideOffset, LockedTarget lockedTarget)
	{
		double cs = CandidateSize(paneSize, outsideOffset);
		int cell = lockedTarget.Cells[0];
		int digit = lockedTarget.Digit;
		var pt = GetMousePointInCenter(paneSize, outsideOffset, cell, digit);
		return pt with { X = pt.X - cs / 2, Y = pt.Y - cs / 2 };
	}

	/// <summary>
	/// Gets the bottom right cursor point of the specified locked target.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="lockedTarget">The locked target.</param>
	/// <returns>The bottom right cursor point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point GetMouseBottomRight(double paneSize, double outsideOffset, LockedTarget lockedTarget)
	{
		double cs = CandidateSize(paneSize, outsideOffset);
		int cell = lockedTarget.Cells[^1];
		int digit = lockedTarget.Digit;
		var pt = GetMousePointInCenter(paneSize, outsideOffset, cell, digit);
		return pt with { X = pt.X + cs / 2, Y = pt.Y + cs / 2 };
	}

	/// <summary>
	/// Gets the center cursor point of the specified locked target.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="lockedTarget">The locked target.</param>
	/// <returns>The center cursor point.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point GetMouseCenter(double paneSize, double outsideOffset, LockedTarget lockedTarget)
	{
		return lockedTarget switch
		{
			{ Cells: [var a, .., var b], Digit: var digit } when f(a, b, digit) is var ((x1, y1), (x2, y2))
				=> new((x1 + x2) / 2, (y1 + y2) / 2),
			{ Cells: [var s], Digit: var digit } when s * 9 + digit is var c
				=> GetMousePointInCenter(paneSize, outsideOffset, c / 9, c % 9),
			_ => throw new ArgumentException("Cannot get at least 1 cell in the map.")
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		(Point, Point) f(int a, int b, int digit)
		{
			int min = a * 9 + digit;
			int max = b * 9 + digit;
			return (
				GetMousePointInCenter(paneSize, outsideOffset, min / 9, min % 9),
				GetMousePointInCenter(paneSize, outsideOffset, max / 9, max % 9)
			);
		}
	}

	/// <summary>
	/// Gets the center mouse cursor point of the specified cell and digit.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The point.</returns>
	public static Point GetMousePointInCenter(double paneSize, double outsideOffset, int cell, int digit)
	{
		double cs = CandidateSize(paneSize, outsideOffset);
		var (x, y) = GridPoints(paneSize, outsideOffset)[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3];
		return new(x + cs / 2, y + cs / 2);
	}

	/// <summary>
	/// Gets the start and end point that corresponds to the target block line at the specified index.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="order">The index of the target block line. The value must be between 0 and 3.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Point Start, Point End) GetBlockLine(double paneSize, double outsideOffset, byte order)
		=> GetLine(paneSize, outsideOffset, order, 3);

	/// <summary>
	/// Gets the start and end point that corresponds to the target cell line at the specified index.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="order">The index of the target cell line. The value must be between 0 and 9.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Point Start, Point End) GetCellLine(double paneSize, double outsideOffset, byte order)
		=> GetLine(paneSize, outsideOffset, order, 9);

	/// <summary>
	/// Gets the start and end point that corresponds to the target candidate line at the specified index.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="order">The index of the target candidate line. The value must be between 0 and 27.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Point Start, Point End) GetCandidateLine(double paneSize, double outsideOffset, byte order)
		=> GetLine(paneSize, outsideOffset, order, 27);

	/// <summary>
	/// Gets the cell index via the <see cref="Point"/> value through the mouse interaction.
	/// </summary>
	/// <param name="point">The point value.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <returns>
	/// The cell index. If the argument <paramref name="point"/> is invalid,
	/// the return value will be -1.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCell(scoped in Point point, double paneSize, double outsideOffset)
		=> point with { X = point.X - outsideOffset, Y = point.Y - outsideOffset } is var (x, y)
		&& GridSize(paneSize, outsideOffset) is var gs
		&& x >= 0 && x <= gs && y >= 0 && y <= gs
		&& CellSize(paneSize, outsideOffset) is var cs
		&& (int)(y / cs) * 9 + (int)(x / cs) is var result and >= 0 and < 81
			? result
			: -1;

	/// <summary>
	/// Gets the candidate index via the <see cref="Point"/> value through the mouse interaction.
	/// </summary>
	/// <param name="point">The point value.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <returns>
	/// The candidate index. If the argument <paramref name="point"/> is invalid,
	/// the return value will be -1.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCandidate(scoped in Point point, double paneSize, double outsideOffset)
		=> point is var (x, y)
		&& CandidateSize(paneSize, outsideOffset) is var cs
		&& (int)((y - outsideOffset) / cs) % 3 * 3 + (int)((x - outsideOffset) / cs) % 3 is var candidateOffset
		&& GetCell(point, paneSize, outsideOffset) is var cellOffset and not -1
			? cellOffset * 9 + candidateOffset
			: -1;

	/// <summary>
	/// Indicates the grid points.
	/// </summary>
	private static Point[,] GridPoints(double paneSize, double outsideOffset)
	{
		const int length = 28;
		double cs = CandidateSize(paneSize, outsideOffset);
		var result = new Point[length, length];
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				result[i, j] = new(cs * i + outsideOffset, cs * j + outsideOffset);
			}
		}

		return result;
	}

	/// <summary>
	/// Gets the start and end point that corresponds to the target line at the specified index,
	/// with the specified line weight.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="i">The index of the target line.</param>
	/// <param name="w">The weight of the line type. The value must be 3, 9 or 27.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (Point, Point) GetLine(double paneSize, double outsideOffset, byte i, int w)
	{
		bool isHorizontal = i <= w;
		double targetSize = (
			(Func<double, double, double>)(w switch { 3 => BlockSize, 9 => CellSize, 27 => CandidateSize })
		)(paneSize, outsideOffset);

		return (
			isHorizontal
				? new(outsideOffset, outsideOffset + i * targetSize)
				: new(outsideOffset + i % (w + 1) * targetSize, outsideOffset),
			isHorizontal
				? new(paneSize - outsideOffset, outsideOffset + i * targetSize)
				: new(outsideOffset + i % (w + 1) * targetSize, paneSize - outsideOffset)
		);
	}
}
