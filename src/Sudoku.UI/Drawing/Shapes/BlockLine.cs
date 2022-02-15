using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a block line.
/// </summary>
#if DEBUG
[DebuggerDisplay($"{{{nameof(DebuggerDisplayView)},nq}}")]
#endif
public sealed class BlockLine : DrawingElement
{
	/// <summary>
	/// The inner line.
	/// </summary>
	private readonly Line _line;

	/// <summary>
	/// Indicates the pane size, which is the backing field of the property <see cref="PaneSize"/>.
	/// </summary>
	/// <seealso cref="PaneSize"/>
	private double _paneSize;

	/// <summary>
	/// Indicates the outside offset, which is the backing field of the property <see cref="OutsideOffset"/>.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	private double _outsideOffset;


	/// <summary>
	/// Initializes a <see cref="BlockLine"/> instance via the specified details.
	/// </summary>
	/// <param name="strokeColor">The stroke color of the block line.</param>
	/// <param name="width">The stroke width of the block line.</param>
	/// <param name="paneSize">Indicates the pane size.</param>
	/// <param name="outsideOffset">Indicates the outside offset.</param>
	/// <param name="order">
	/// The order. The value can only be between 0 and 19. For more details of the parameter,
	/// please see the property <see cref="Order"/>.
	/// </param>
	public BlockLine(Color strokeColor, double width, double paneSize, double outsideOffset, byte order)
	{
		_paneSize = paneSize;
		_outsideOffset = outsideOffset;
		Order = order;
		var ((x1, y1), (x2, y2)) = PointConversions.GetBlockLine(paneSize, outsideOffset, order);
		_line = new Line
		{
			Stroke = new SolidColorBrush(strokeColor),
			StrokeThickness = width,
			X1 = x1,
			Y1 = y1,
			X2 = x2,
			Y2 = y2,
			StrokeLineJoin = PenLineJoin.Round
		};
	}


	/// <summary>
	/// The order of the block line. The value must be between 0 and 19.
	/// <list type="table">
	/// <listheader>
	/// <term>Range</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><![CDATA[>= 0 and < 4]]></term>
	/// <description>The block line is horizontal.</description>
	/// </item>
	/// <item>
	/// <term><![CDATA[>= 4 and < 8]]></term>
	/// <description>The block line is vertical.</description>
	/// </item>
	/// </list>
	/// </summary>
	public byte Order { get; }

	/// <summary>
	/// The stroke width of the block line.
	/// </summary>
	public double Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _line.StrokeThickness;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _line.StrokeThickness = value;
	}

	/// <summary>
	/// The pane size.
	/// </summary>
	public double PaneSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _paneSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var ((x1, y1), (x2, y2)) = PointConversions.GetBlockLine(_paneSize = value, _outsideOffset, Order);
			_line.X1 = x1;
			_line.X2 = x2;
			_line.Y1 = y1;
			_line.Y2 = y2;
		}
	}

	/// <summary>
	/// The outside offset.
	/// </summary>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var ((x1, y1), (x2, y2)) = PointConversions.GetBlockLine(_paneSize, _outsideOffset = value, Order);
			_line.X1 = x1;
			_line.X2 = x2;
			_line.Y1 = y1;
			_line.Y2 = y2;
		}
	}

	/// <summary>
	/// The stroke color of the block line.
	/// </summary>
	public Color StrokeColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((SolidColorBrush)_line.Stroke).Color;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _line.Stroke = new SolidColorBrush(value);
	}

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	private string DebuggerDisplayView
	{
		get
		{
			var (x1, x2, y1, y2) = _line;
			return $"{nameof(BlockLine)} {{ Start = {(x1, y1)}, End = {(x2, y2)} }}";
		}
	}
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is BlockLine comparer
			&& _line.X1.NearlyEquals(comparer._line.X1, 1E-2)
			&& _line.X2.NearlyEquals(comparer._line.X2, 1E-2)
			&& _line.Y1.NearlyEquals(comparer._line.Y1, 1E-2)
			&& _line.Y2.NearlyEquals(comparer._line.Y2, 1E-2);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() =>
		HashCode.Combine(nameof(BlockLine), _line.X1, _line.X2, _line.Y1, _line.Y2);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Line GetControl() => _line;
}
