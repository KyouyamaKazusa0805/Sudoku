namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines an outside rectangle.
/// </summary>
public sealed class OutsideRectangle : DrawingElement
{
	/// <summary>
	/// The inner rectangle.
	/// </summary>
	private readonly Rectangle _rect = null!;

	/// <summary>
	/// Indicates the stroke thickness.
	/// </summary>
	private double _strokeThickness;

	/// <summary>
	/// Indicates the stroke color.
	/// </summary>
	private Color _strokeColor;

	/// <summary>
	/// Indicates the fill color.
	/// </summary>
	private Color _fillColor;


	/// <summary>
	/// The size of the rectangle.
	/// </summary>
	public double RectangleSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _rect.Width;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _rect.Width = _rect.Height = value;
	}

	/// <summary>
	/// The size of the sudoku grid pane.
	/// </summary>
	public required double PaneSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _rect.Width;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _rect ??= new Rectangle()
			.WithWidth(value)
			.WithHeight(value)
			.WithStroke(new SolidColorBrush(_strokeColor))
			.WithStrokeThickness(_strokeThickness)
			.WithFill(new SolidColorBrush(_fillColor));
	}

	/// <summary>
	/// The stroke thickness of the block line.
	/// </summary>
	public required double StrokeThickness
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _strokeThickness;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_strokeThickness = value;
			_rect.StrokeThickness = value;
		}
	}

	/// <summary>
	/// The stroke color of the block line.
	/// </summary>
	public required Color StrokeColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _strokeColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_strokeColor = value;
			_rect.Stroke = new SolidColorBrush(value);
		}
	}

	/// <summary>
	/// The filling color of the outside rectangle.
	/// </summary>
	public required Color FillColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _fillColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_fillColor = value;
			_rect.Fill = new SolidColorBrush(value);
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(OutsideRectangle);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is OutsideRectangle comparer && ReferenceEquals(_rect, comparer._rect);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _rect.Width, _rect.Height);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Rectangle GetControl() => _rect;
}
