namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a cell view element.
/// </summary>
public sealed class CellViewElement : ViewElement
{
	/// <summary>
	/// Indicates the cell view node.
	/// </summary>
	private readonly CellViewNode _cellViewNode;

	/// <summary>
	/// Indicates the <see cref="Canvas"/> instance that stores four border lines.
	/// </summary>
	private readonly Canvas _canvas;

	/// <summary>
	/// Indicates the lines extracted from the control <see cref="_canvas"/>.
	/// </summary>
	/// <seealso cref="_canvas"/>
	private readonly Line[] _lines;


	/// <summary>
	/// Initializes a <see cref="CellViewElement"/> instance via the specified cell, pane size and the outside offset.
	/// </summary>
	/// <param name="cellViewNode">The cell view node.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="userPreference">The user preference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellViewElement(CellViewNode cellViewNode, double paneSize, double outsideOffset, IDrawingPreference userPreference)
	{
		(var (identifier, cell), _cellViewNode, double thickness, _canvas) = (
			cellViewNode,
			cellViewNode,
			userPreference.HighlightCellStrokeThickness,
			new()
		);

		var targetColor = identifier.AsColor(userPreference);
		var ((ax1, ax2), (ay1, ay2)) = PointConversions.GetCellLine(paneSize, outsideOffset, (byte)(cell / 9));
		var ((bx1, bx2), (by1, by2)) = PointConversions.GetCellLine(paneSize, outsideOffset, (byte)(cell / 9 + 1));
		var ((cx1, cx2), (cy1, cy2)) = PointConversions.GetCellLine(paneSize, outsideOffset, (byte)(cell % 9 + 10));
		var ((dx1, dx2), (dy1, dy2)) = PointConversions.GetCellLine(paneSize, outsideOffset, (byte)(cell % 9 + 11));
		_lines = new Line[]
		{
			new()
			{
				Stroke = new SolidColorBrush(targetColor),
				StrokeThickness = thickness,
				X1 = ax1,
				X2 = ax2,
				Y1 = ay1,
				Y2 = ay2
			},
			new()
			{
				Stroke = new SolidColorBrush(targetColor),
				StrokeThickness = thickness,
				X1 = bx1,
				X2 = bx2,
				Y1 = by1,
				Y2 = by2
			},
			new()
			{
				Stroke = new SolidColorBrush(targetColor),
				StrokeThickness = thickness,
				X1 = cx1,
				X2 = cx2,
				Y1 = cy1,
				Y2 = cy2
			},
			new()
			{
				Stroke = new SolidColorBrush(targetColor),
				StrokeThickness = thickness,
				X1 = dx1,
				X2 = dx2,
				Y1 = dy1,
				Y2 = dy2
			}
		};

		_canvas.Children.AddRange(_lines);
	}


	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CellViewElement);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewElement? other) =>
		other is CellViewElement comparer && _cellViewNode.Cell == comparer._cellViewNode.Cell;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _cellViewNode.Cell);

	/// <inheritdoc/>
	public override Canvas GetControl() => _canvas;
}
