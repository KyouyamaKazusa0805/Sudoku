using Windows.Foundation;

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
	public static double BlockSize(double paneSize, double outsideOffset) =>
		GridSize(paneSize, outsideOffset) / 3;

	/// <summary>
	/// Gets the cell size from the specified pane size and the outside offset.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset value. The value must be a positive value.</param>
	/// <returns>The cell size.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CellSize(double paneSize, double outsideOffset) =>
		GridSize(paneSize, outsideOffset) / 9;

	/// <summary>
	/// Gets the candidate size from the specified pane size and the outside offset.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset value. The value must be a positive value.</param>
	/// <returns>The candidate size.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CandidateSize(double paneSize, double outsideOffset) =>
		GridSize(paneSize, outsideOffset) / 27;

	/// <summary>
	/// Gets the start and end point that corresponds to the target block line at the specified index.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="order">The index of the target block line. The value must be between 0 and 3.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Point Start, Point End) GetBlockLine(double paneSize, double outsideOffset, byte order) =>
		GetLine(paneSize, outsideOffset, order, 3);

	/// <summary>
	/// Gets the start and end point that corresponds to the target cell line at the specified index.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="order">The index of the target cell line. The value must be between 0 and 9.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Point Start, Point End) GetCellLine(double paneSize, double outsideOffset, byte order) =>
		GetLine(paneSize, outsideOffset, order, 9);

	/// <summary>
	/// Gets the start and end point that corresponds to the target candidate line at the specified index.
	/// </summary>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="order">The index of the target candidate line. The value must be between 0 and 27.</param>
	/// <returns>The two points indicating the start and end point of the line.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Point Start, Point End) GetCandidateLine(double paneSize, double outsideOffset, byte order) =>
		GetLine(paneSize, outsideOffset, order, 27);

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
			w switch
			{
				3 => BlockSize,
				9 => CellSize,
				27 => CandidateSize,
				_ => default(Func<double, double, double>)!
			}
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
