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
			return (int)(y / ch) * 9 + (int)(x / cw);
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
			return GetCellOffset(point) * 9 + (int)(y / ch) % 3 * 3 + (int)(x / cw) % 3;
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
			var (x, y) = GridPoints[cellOffset % 9, cellOffset / 9];
			return new PointF(x + Offset + cw * 2, y + Offset + ch * 2);
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
			var (x, y) = GridPoints[cellOffset / 9, cellOffset % 9];
			return new PointF(
				digit % 3 * cw + Offset + x + cw * 2,
				digit / 3 * ch + Offset + y + ch * 2);
		}

		/// <summary>
		/// Bound with the initializer <see cref="PointConverter(int, int)"/>,
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
			var (cellWidth, cellHeight) = CellSize = new SizeF(gridWidth / 9, gridHeight / 9);
			CandidateSize = new SizeF(gridWidth / 27, gridHeight / 27);
			PanelSize = new SizeF(cellWidth * 27 + (Offset << 1), cellHeight * 27 + (Offset << 1));
		}

		/// <summary>
		/// Bound with the initializer <see cref="PointConverter(int, int)"/>,
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
