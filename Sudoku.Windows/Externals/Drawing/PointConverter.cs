using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides converting operations for <see cref="Point"/> and
	/// <see cref="PointF"/> usages in the form. For example, this class
	/// will calculate and convert between the drawing coordinates and
	/// the mouse coordinates.
	/// </summary>
	/// <seealso cref="Point"/>
	/// <seealso cref="PointF"/>
	public sealed class PointConverter
	{
		/// <summary>
		/// Indicates the width of the gap between the picture box outline
		/// and the sudoku grid outline.
		/// </summary>
		public const int Offset = 10;


		/// <summary>
		/// Initializes an instance with the width and height.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public PointConverter(int width, int height) : this(new SizeF(width, height))
		{
		}

		/// <summary>
		/// Initializes an instance with the width and height.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public PointConverter(float width, float height) : this(new SizeF(width, height))
		{
		}

		/// <summary>
		/// Initializes an instance with the specified <see cref="SizeF"/>.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <seealso cref="SizeF"/>
		public PointConverter(SizeF size)
		{
			InitializeSizes(size);
			InitializePoints();
		}


		/// <summary>
		/// Indicates the absolutely points in grid cross-lines.
		/// This property will be assigned later (and not <see langword="null"/>).
		/// </summary>
		public PointF[,] GridPoints { get; private set; } = null!;

		/// <summary>
		/// Indicates the panel size.
		/// </summary>
		public SizeF PanelSize { get; private set; }

		/// <summary>
		/// Indicates the control size.
		/// </summary>
		public SizeF ControlSize { get; private set; }

		/// <summary>
		/// Indicates the grid size.
		/// </summary>
		public SizeF GridSize { get; private set; }

		/// <summary>
		/// Indicates the cell size.
		/// </summary>
		public SizeF CellSize { get; private set; }

		/// <summary>
		/// Indicates the candidate size.
		/// </summary>
		public SizeF CandidateSize { get; private set; }


		/// <summary>
		/// Get the focus cell offset via a mouse point.
		/// </summary>
		/// <param name="point">The mouse point.</param>
		/// <returns>The cell offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetCellOffset(PointF point)
		{
			var (x, y) = point;
			var (cw, ch) = CellSize;
			return (int)((y - Offset) / ch) * 9 + (int)((x - Offset) / cw);
		}

		/// <summary>
		/// Get the focus candidate offset via a mouse point.
		/// </summary>
		/// <param name="point">The mouse point.</param>
		/// <returns>The candidate offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetCandidateOffset(PointF point)
		{
			var (x, y) = point;
			var (cw, ch) = CandidateSize;
			return GetCellOffset(point) * 9 + (int)((y - Offset) / ch) % 3 * 3 + (int)((x - Offset) / cw) % 3;
		}

		/// <summary>
		/// Get the center mouse point of all candidates.
		/// </summary>
		/// <param name="map">The map of candidates.</param>
		/// <returns>The center mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PointF GetMouseCenterOfCandidates(FullGridMap map)
		{
			int min = map.SetAt(0);
			int max = map.SetAt(^1);
			var (x1, y1) = GetMousePointInCenter(min / 9, min % 9);
			var (x2, y2) = GetMousePointInCenter(max / 9, max % 9);
			return new PointF((x1 + x2) / 2, (y1 + y2) / 2);
		}

		/// <summary>
		/// Get the rectangle from all candidates.
		/// </summary>
		/// <param name="map">The candidates.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMouseRectangleOfCandidates(FullGridMap map)
		{
			var (cw, ch) = CandidateSize;
			int min = map.SetAt(0);
			int max = map.SetAt(^1);
			var pt1 = GetMousePointInCenter(min / 9, min % 9);
			var pt2 = GetMousePointInCenter(max / 9, max % 9);
			pt1.X -= cw / 2;
			pt1.Y -= ch / 2;
			pt2.X += cw / 2;
			pt2.Y += ch / 2;
			return RectangleEx.CreateInstance(pt1, pt2);
		}

		/// <summary>
		/// Get the rectangle (4 mouse points) for the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMousePointRectangle(int cell)
		{
			var (cw, ch) = CellSize;
			var (x, y) = GetMousePointInCenter(cell);
			return new RectangleF(x - cw / 2, y - ch / 2, cw, ch);
		}

		/// <summary>
		/// Get the rectangle (4 mouse points) for the specified cell
		/// and digit of a candidate.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>The rectangle.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectangleF GetMousePointRectangle(int cell, int digit)
		{
			var (cw, ch) = CandidateSize;
			var (x, y) = GetMousePointInCenter(cell, digit);
			return new RectangleF(x - cw / 2, y - ch / 2, cw, ch);
		}

		/// <summary>
		/// Get the rectangle (4 mouse points) for the specified region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The rectangle.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the region is less than 0 or greater than 26.
		/// </exception>
		public RectangleF GetMousePointRectangleForRegion(int region)
		{
			if (region >= 0 && region < 9)
			{
				return RectangleEx.CreateInstance(
					GridPoints[region % 3 * 9, region / 3 * 9],
					GridPoints[region % 3 * 9 + 9, region / 3 * 9 + 9]);
			}
			else if (region >= 9 && region < 18)
			{
				region -= 9;
				return RectangleEx.CreateInstance(GridPoints[0, region * 3], GridPoints[27, region * 3 + 3]);
			}
			else if (region >= 18 && region < 27)
			{
				region -= 18;
				return RectangleEx.CreateInstance(GridPoints[region * 3, 0], GridPoints[region * 3 + 3, 27]);
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(region));
			}
		}

		/// <summary>
		/// Get the mouse point of the center of a cell via its offset.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>The mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PointF GetMousePointInCenter(int cellOffset)
		{
			var (cw, ch) = CellSize;
			var (x, y) = GridPoints[cellOffset % 9 * 3, cellOffset / 9 * 3];
			return new PointF(x + cw / 2, y + ch / 2);
		}

		/// <summary>
		/// Get the mouse point of the center of a cell via its offset and the digit.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PointF GetMousePointInCenter(int cellOffset, int digit)
		{
			var (cw, ch) = CandidateSize;
			var (x, y) = GridPoints[cellOffset % 9 * 3 + digit % 3, cellOffset / 9 * 3 + digit / 3];
			return new PointF(x + cw / 2, y + ch / 2);
		}

		/// <summary>
		/// Bound with the constructor <see cref="PointConverter(int, int)"/>,
		/// <see cref="PointConverter(float, float)"/> and
		/// <see cref="PointConverter(SizeF)"/>.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <seealso cref="PointConverter(int, int)"/>
		/// <seealso cref="PointConverter(float, float)"/>
		/// <seealso cref="PointConverter(SizeF)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeSizes(SizeF size)
		{
			var (width, height) = ControlSize = size;
			var (gridWidth, gridHeight) = GridSize = new SizeF(width - (Offset << 1), height - (Offset << 1));
			CellSize = new SizeF(gridWidth / 9, gridHeight / 9);
			CandidateSize = new SizeF(gridWidth / 27, gridHeight / 27);
		}

		/// <summary>
		/// Bound with the constructor <see cref="PointConverter(int, int)"/>,
		/// <see cref="PointConverter(float, float)"/> and
		/// <see cref="PointConverter(SizeF)"/>.
		/// </summary>
		/// <seealso cref="PointConverter(int, int)"/>
		/// <seealso cref="PointConverter(float, float)"/>
		/// <seealso cref="PointConverter(SizeF)"/>
		private void InitializePoints()
		{
			const int length = 28;
			var (cw, ch) = CandidateSize;
			GridPoints = new PointF[length, length];
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length; j++)
				{
					GridPoints[i, j] = new PointF(cw * i + Offset, ch * j + Offset);
				}
			}
		}
	}
}
