namespace Sudoku.Drawing;

/// <summary>
/// Provides a serial of methods for a point calculator that interacts with the UI projects.
/// </summary>
public interface IPointCalculator
{
	/// <summary>
	/// Indicates the default offset value.
	/// </summary>
	protected internal const float DefaultOffset = 10F;

	/// <summary>
	/// Indicates the number of anchors hold per house.
	/// </summary>
	/// <remarks>
	/// The sudoku grid painter will draw the outlines and the inner lines, and correct the point
	/// of each digits (candidates also included). Each row or column always contains 27 candidates,
	/// so this value is 27.
	/// </remarks>
	protected internal const int AnchorsCount = 27;


	/// <summary>
	/// Indicates the width of the picture to draw.
	/// </summary>
	public abstract float Width { get; }

	/// <summary>
	/// Indicates the height of the picture to draw.
	/// </summary>
	public abstract float Height { get; }

	/// <summary>
	/// Indicates the offset of the gap between the picture box outline and the sudoku grid outline.
	/// </summary>
	public abstract float Offset { get; }

	/// <summary>
	/// Indicates the control size.
	/// </summary>
	public abstract SizeF ControlSize { get; }

	/// <summary>
	/// Indicates the grid size.
	/// </summary>
	public abstract SizeF GridSize { get; }

	/// <summary>
	/// Indicates the cell size.
	/// </summary>
	public abstract SizeF CellSize { get; }

	/// <summary>
	/// Indicates the candidate size.
	/// </summary>
	public abstract SizeF CandidateSize { get; }

	/// <summary>
	/// Indicates the absolutely points in grid cross-lines.
	/// This property will be assigned later (and not <see langword="null"/>).
	/// </summary>
	/// <remarks>Note that the size of this 2D array is always 28 by 28.</remarks>
	public abstract PointF[,] GridPoints { get; }


	/// <summary>
	/// Get the focus cell offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The cell offset. Returns -1 when the current point is invalid.</returns>
	public abstract int GetCell(scoped in PointF point);

	/// <summary>
	/// Get the focus candidate offset via a mouse point.
	/// </summary>
	/// <param name="point">The mouse point.</param>
	/// <returns>The candidate offset.</returns>
	public abstract int GetCandidate(scoped in PointF point);

	/// <summary>
	/// Get the center mouse point of all candidates.
	/// </summary>
	/// <param name="map">The map of candidates.</param>
	/// <returns>The center mouse point.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	public abstract PointF GetMouseCenter(scoped in Candidates map);

	/// <summary>
	/// Gets the center mouse point of the specified locked target.
	/// </summary>
	/// <param name="lockedTarget">The locked target.</param>
	/// <returns>The center mouse point.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	public abstract PointF GetMouseCenter(scoped in LockedTarget lockedTarget);

	/// <summary>
	/// Get the rectangle from all candidates.
	/// </summary>
	/// <param name="map">The candidates.</param>
	/// <returns>The rectangle.</returns>
	/// <exception cref="ArgumentException">Throws when the argument is invalid.</exception>
	public abstract RectangleF GetMouseRectangle(scoped in Candidates map);

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The rectangle.</returns>
	public abstract RectangleF GetMouseRectangleViaCell(int cell);

	/// <summary>
	/// Get the rectangle (4 mouse points) for the specified cell and digit of a candidate.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The rectangle.</returns>
	public abstract RectangleF GetMouseRectangle(int cell, int digit);

	/// <summary>
	/// Get the rectangle (4 mouse points) via the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The rectangle.</returns>
	public abstract RectangleF GetMouseRectangleViaRegion(int house);

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <returns>The mouse point.</returns>
	public abstract PointF GetMousePointInCenter(int cell);

	/// <summary>
	/// Get the mouse point of the center of a cell via its offset and the digit.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The mouse point.</returns>
	public abstract PointF GetMousePointInCenter(int cell, int digit);

	/// <summary>
	/// Gets two points that specifies and represents the anchors of this house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The anchor points.</returns>
	public abstract (PointF LeftUp, PointF RightDown) GetAnchorsViaRegion(int house);


	/// <summary>
	/// Creates a <see cref="IPointCalculator"/> instance with the specified size, and the offset.
	/// </summary>
	/// <param name="size">The size.</param>
	/// <param name="offset">The offset. The default value is <c>10</c>.</param>
	/// <returns>An <see cref="IPointCalculator"/> instance.</returns>
	public static abstract IPointCalculator CreateConverter(float size, float offset = 10);
}
