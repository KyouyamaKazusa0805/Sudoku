namespace Sudoku.UI.Drawing;

/// <summary>
/// Defines a point calculator that interacts with the UI projects.
/// </summary>
public sealed class PointCalculator
{
	/// <summary>
	/// Indicates the default offset value.
	/// </summary>
	public const double DefaultOffset = 10;

	/// <summary>
	/// Indicates the number of anchors hold per region.
	/// </summary>
	/// <remarks>
	/// The sudoku grid painter will draw the outlines and the inner lines, and correct the point
	/// of each digits (candidates also included). Each row or column always contains 27 candidates,
	/// so this value is 27.
	/// </remarks>
	public const int AnchorsCount = 27;


	/// <summary>
	/// Indicates the width of the picture to draw.
	/// </summary>
	public double Width => ControlSize.Width;

	/// <summary>
	/// Indicates the height of the picture to draw.
	/// </summary>
	public double Height => ControlSize.Height;

	/// <summary>
	/// Indicates the offset of the gap between the picture box outline and the sudoku grid outline.
	/// </summary>
	/// <remarks>The default value is <c>10</c>.</remarks>
	public double Offset { get; set; } = DefaultOffset;

	/// <summary>
	/// Indicates the control size.
	/// </summary>
	public Size ControlSize { get; set; }

	/// <summary>
	/// Indicates the grid size.
	/// </summary>
	public Size GridSize => new(ControlSize.Width - Offset / 2, ControlSize.Height - Offset / 2);

	/// <summary>
	/// Indicates the cell size.
	/// </summary>
	public Size CellSize => new(GridSize.Width / (AnchorsCount / 3), GridSize.Height / (AnchorsCount / 3));

	/// <summary>
	/// Indicates the candidate size.
	/// </summary>
	public Size CandidateSize => new(GridSize.Width / AnchorsCount, GridSize.Height / AnchorsCount);

	/// <summary>
	/// Indicates the absolutely points in grid cross-lines.
	/// This property will be assigned later (and not <see langword="null"/>).
	/// </summary>
	/// <remarks>Note that the size of this 2D array is always 28 by 28.</remarks>
	public Point[,] GridPoints
	{
		get
		{
			const int length = AnchorsCount + 1;
			var (cw, ch) = CandidateSize;
			var result = new Point[length, length];
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length; j++)
				{
					GridPoints[i, j] = new(cw * i + Offset, ch * j + Offset);
				}
			}

			return result;
		}
	}


	/// <summary>
	/// Get the focus cell offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The cell offset. Returns -1 when the current point is invalid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetCell(in Point point)
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
	public int GetCandidate(in Point point)
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
	public Point GetMouseCenter(in Candidates map)
	{
		int min = map[0], max = map[^1];
		var (x1, y1) = GetGridRowAndColumn(min / 9, min % 9);
		var (x2, y2) = GetGridRowAndColumn(max / 9, max % 9);
		return new((x1 + x2) / 2, (y1 + y2) / 2);
	}

	///// <summary>
	///// Get the rectangle from all candidates.
	///// </summary>
	///// <param name="map">The candidates.</param>
	///// <returns>The rectangle.</returns>
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public Rect GetMouseRectangle(in Candidates map)
	//{
	//	var (cw, ch) = CandidateSize;
	//	int min = map[0], max = map[^1];
	//	var pt1 = GetGridRowAndColumn(min / 9, min % 9).WithOffset(-cw / 2, -ch / 2);
	//	var pt2 = GetGridRowAndColumn(max / 9, max % 9).WithOffset(cw / 2, ch / 2);
	//	return new(pt1, pt2);
	//}

	///// <summary>
	///// Get the rectangle (4 mouse points) via the specified cell.
	///// </summary>
	///// <param name="cell">The cell.</param>
	///// <returns>The rectangle.</returns>
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public Rect GetMouseRectangleViaCell(int cell)
	//{
	//	var (cw, ch) = CellSize;
	//	var (x, y) = GetGridRowAndColumn(cell);
	//	return new(x - cw / 2, y - ch / 2, cw, ch);
	//}

	///// <summary>
	///// Get the rectangle (4 mouse points) for the specified cell
	///// and digit of a candidate.
	///// </summary>
	///// <param name="cell">The cell.</param>
	///// <param name="digit">The digit.</param>
	///// <returns>The rectangle.</returns>
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public Rect GetMouseRectangle(int cell, int digit)
	//{
	//	var (cw, ch) = CandidateSize;
	//	var (x, y) = GetGridRowAndColumn(cell, digit);
	//	return new(x - cw / 2, y - ch / 2, cw, ch);
	//}

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified region.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <returns>The rectangle.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the region is less than 0 or greater than 26.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rect GetMouseRectangleViaRegion(int region)
	{
		var (l, r) = GetAnchorsViaRegion(region);
		return new(l, r);
	}

	/// <summary>
	/// Gets two points that specifies and represents the anchors of this region.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <returns>The anchor points.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="region"/> is out of range.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Point LeftUp, Point RightDown) GetAnchorsViaRegion(int region) => region switch
	{
		>= 0 and < 9 when (region % 3, region / 3) is (var v1, var v2) => (
			GridPoints[v1 * 9, v2 * 9],
			GridPoints[v1 * 9 + 9, v2 * 9 + 9]
		),
		>= 9 and < 18 when region - 9 is var v => (GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]),
		>= 18 and < 27 when region - 18 is var v => (GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]),
		_ => throw new ArgumentOutOfRangeException(nameof(region))
	};

	/// <summary>
	/// Get the pair of the <see cref="Grid"/> position via its offset.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <returns>The pair of the <see cref="Grid"/> position.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (int Row, int Column) GetGridRowAndColumn(int cell) => (cell % 9 * 3, cell / 9 * 3);

	/// <summary>
	/// Get the pair of the <see cref="Grid"/> position via its offset and the digit.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The pair of the <see cref="Grid"/> position.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (int Row, int Column) GetGridRowAndColumn(int cell, int digit) =>
		(cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3);
}
