using System;
using System.Drawing;
using System.Runtime.CompilerServices;
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
		public const int Offset = 3;


		/// <summary>
		/// Initializes an instance with the width and height.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public PointConverter(int width, int height) : this(new Size(width, height))
		{
		}

		/// <summary>
		/// Initializes an instance with the specified <see cref="Size"/>.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <seealso cref="Size"/>
		public PointConverter(Size size)
		{
			InitializeSizes(size);
			InitializePoints();
		}


		/// <summary>
		/// Indicates the absolutely points in grid cross-lines.
		/// This property will be assigned later (and not <see langword="null"/>).
		/// </summary>
		public Point[,] GridPoints { get; private set; } = null!;

		/// <summary>
		/// Indicates the panel size.
		/// </summary>
		public Size PanelSize { get; private set; }

		/// <summary>
		/// Indicates the control size.
		/// </summary>
		public Size ControlSize { get; private set; }

		/// <summary>
		/// Indicates the grid size.
		/// </summary>
		public Size GridSize { get; private set; }

		/// <summary>
		/// Indicates the cell size.
		/// </summary>
		public Size CellSize { get; private set; }

		/// <summary>
		/// Indicates the candidate size.
		/// </summary>
		public Size CandidateSize { get; private set; }


		/// <summary>
		/// Get the focus cell offset via a mouse point.
		/// </summary>
		/// <param name="point">The mouse point.</param>
		/// <returns>The cell offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetCellOffset(Point point)
		{
			var (x, y) = point;
			var (cw, ch) = CellSize;
			return y / ch * 9 + x / cw;
		}

		/// <summary>
		/// Get the focus candidate offset via a mouse point.
		/// </summary>
		/// <param name="point">The mouse point.</param>
		/// <returns>The candidate offset.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetCandidateOffset(Point point)
		{
			var (x, y) = point;
			var (cw, ch) = CandidateSize;
			return GetCellOffset(point) * 9 + y / ch % 3 * 3 + x / cw % 3;
		}

		/// <summary>
		/// Get the mouse point of the center of a cell via its offset.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <returns>The mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Point GetMousePointInCenter(int cellOffset)
		{
			var (cw, ch) = CellSize;
			var (x, y) = GridPoints[cellOffset % 9, cellOffset / 9];
			return new Point(x + Offset + (cw >> 1), y + Offset + (ch >> 1));
		}

		/// <summary>
		/// Get the mouse point of the center of a cell via its offset and the digit.
		/// </summary>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The mouse point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Point GetMousePointInCenter(int cellOffset, int digit)
		{
			var (cw, ch) = CandidateSize;
			var (x, y) = GridPoints[cellOffset / 9, cellOffset % 9];
			return new Point(
				digit % 3 * cw + Offset + x + (cw >> 1),
				digit / 3 * ch + Offset + y + (ch >> 1));
		}

		/// <summary>
		/// Bound with the initializer <see cref="PointConverter(int, int)"/>
		/// and <see cref="PointConverter(Size)"/>.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <seealso cref="PointConverter(int, int)"/>
		/// <seealso cref="PointConverter(Size)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeSizes(Size size)
		{
			var (width, height) = ControlSize = size;
			var (gridWidth, gridHeight) = GridSize = new Size(width - (Offset << 1), height - (Offset << 1));
			var (cellWidth, cellHeight) = CellSize = new Size(gridWidth / 9, gridHeight / 9);
			CandidateSize = new Size(gridWidth / 27, gridHeight / 27);
			PanelSize = new Size(cellWidth * 27 + (Offset << 1), cellHeight * 27 + (Offset << 1));
		}

		/// <summary>
		/// Bound with the initializer <see cref="PointConverter(int, int)"/>
		/// and <see cref="PointConverter(Size)"/>.
		/// </summary>
		/// <seealso cref="PointConverter(int, int)"/>
		/// <seealso cref="PointConverter(Size)"/>
		private void InitializePoints()
		{
			const int length = 28;
			var (cw, ch) = CandidateSize;
			GridPoints = new Point[length, length];
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length; j++)
				{
					GridPoints[i, j] = new Point(cw * i + Offset, ch * j + Offset);
				}
			}
		}
	}
}
