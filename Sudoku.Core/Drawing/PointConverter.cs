using System.Drawing;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides converting operations for <see cref="Point"/> and
	/// <see cref="PointF"/> usages in the form.
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
		/// Bound with the initializer <see cref="PointConverter(int, int)"/>
		/// and <see cref="PointConverter(Size)"/>.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <seealso cref="PointConverter(int, int)"/>
		/// <seealso cref="PointConverter(Size)"/>
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
			var (cw, ch) = CellSize;
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
