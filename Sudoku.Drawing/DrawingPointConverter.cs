using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides an instance for converting drawing points (i.e. <see cref="PointF"/>)
	/// to an element in sudoku grid.
	/// </summary>
	public readonly struct DrawingPointConverter
	{
		/// <summary>
		/// Indicates the width of the gap between the picture box outline
		/// and the sudoku grid outline.
		/// </summary>
		private const int Offset = 10;


		/// <summary>
		/// Initializes an instance with the specified width and height.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public DrawingPointConverter(int width, int height) : this(new(width, height))
		{
		}

		/// <summary>
		/// Initializes an instance with the specified width and height.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public DrawingPointConverter(float width, float height) : this(new(width, height))
		{
		}

		/// <summary>
		/// Initializes an instance with the specified <see cref="SizeF"/> instance.
		/// </summary>
		/// <param name="size">(<see langword="in"/> parameter) The <see cref="SizeF"/> instance.</param>
		public DrawingPointConverter(in SizeF size)
		{
			// Initialize sizes.
			{
				var (width, height) = ControlSize = size;
				var (gridWidth, gridHeight) = GridSize = new(width - (Offset << 1), height - (Offset << 1));
				CellSize = new(gridWidth / 9, gridHeight / 9);
				CandidateSize = new(gridWidth / 27, gridHeight / 27);
			}

			// Initialize points.
			{
				const int length = 28;
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
		}


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
		/// <remarks>
		/// Note that the size of this 2D array is always 28 by 28.
		/// </remarks>
		internal PointF[,] GridPoints { get; }


		/// <summary>
		/// Get the focus cell offset via a mouse point.
		/// </summary>
		/// <param name="point">(<see langword="in"/> parameter) The mouse point.</param>
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
		/// <param name="point">(<see langword="in"/> parameter) The mouse point.</param>
		/// <returns>The candidate offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetCandidate(in PointF point)
		{
			var (x, y) = point;
			var (cw, ch) = CandidateSize;
			return GetCell(point) * 9 + (int)((y - Offset) / ch) % 3 * 3 + (int)((x - Offset) / cw) % 3;
		}

		/// <summary>
		/// Get the mouse point of the center of a cell via its offset.
		/// </summary>
		/// <param name="cell">The cell offset.</param>
		/// <returns>The mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PointF GetMouseCenter(int cell)
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
		public PointF GetMouseCenter(int cell, int digit)
		{
			var (cw, ch) = CandidateSize;
			var (x, y) = GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3];
			return new(x + cw / 2, y + ch / 2);
		}

		/// <summary>
		/// Get the center mouse point of all candidates.
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The map of candidates.</param>
		/// <returns>The center mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PointF GetMouseCenter(in Candidates map)
		{
			int min = map[0], max = map[^1];
			var (x1, y1) = GetMouseCenter(min / 9, min % 9);
			var (x2, y2) = GetMouseCenter(max / 9, max % 9);
			return new((x1 + x2) / 2, (y1 + y2) / 2);
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
			var (x, y) = GetMouseCenter(cell);
			return new(x - cw / 2, y - ch / 2, cw, ch);
		}

		/// <summary>
		/// Get the rectangle (4 mouse points) for the specified candidate.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMouseRectangleViaCandidate(int candidate) =>
			GetMouseRectangleViaCandidate(candidate / 9, candidate % 9);

		/// <summary>
		/// Get the rectangle (4 mouse points) for the specified cell
		/// and digit of a candidate.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMouseRectangleViaCandidate(int cell, int digit)
		{
			var (cw, ch) = CandidateSize;
			var (x, y) = GetMouseCenter(cell, digit);
			return new(x - cw / 2, y - ch / 2, cw, ch);
		}

		/// <summary>
		/// Get the rectangle from all candidates.
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The candidates.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMouseRectangleViaCandidates(in Candidates map)
		{
			var (cw, ch) = CandidateSize;
			int min = map[0], max = map[^1];
			var pt1 = GetMouseCenter(min / 9, min % 9).WithOffset(-cw / 2, -ch / 2);
			var pt2 = GetMouseCenter(max / 9, max % 9).WithOffset(cw / 2, ch / 2);
			return RectangleEx.CreateInstance(pt1, pt2);
		}

		/// <summary>
		/// Get the rectangle (4 mouse points) via the specified region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMouseRectangleViaRegion(int region)
		{
			switch (region)
			{
				case >= 0 and < 9:
				{
					int v1 = region % 3, v2 = region / 3;
					return RectangleEx.CreateInstance(
						GridPoints[v1 * 9, v2 * 9],
						GridPoints[v1 * 9 + 9, v2 * 9 + 9]
					);
				}
				case >= 9 and < 18:
				{
					int v = region - 9;
					return RectangleEx.CreateInstance(GridPoints[0, v * 3], GridPoints[27, v * 3 + 3]);
				}
				case >= 18 and < 27:
				{
					int v = region - 18;
					return RectangleEx.CreateInstance(GridPoints[v * 3, 0], GridPoints[v * 3 + 3, 27]);
				}
				default:
				{
					throw new ArgumentOutOfRangeException(nameof(region));
				}
			}
		}
	}
}
