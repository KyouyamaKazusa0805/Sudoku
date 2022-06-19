#if AUTHOR_DEFINED

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a cell mark.
/// </summary>
internal sealed class CellMark : DrawingElement
{
	/// <summary>
	/// Indicates the inner rectangle.
	/// </summary>
	private readonly Rectangle _rectangle;

	/// <summary>
	/// Indicates the inner circle.
	/// </summary>
	private readonly Ellipse _circle;

	/// <summary>
	/// Indicates the user preference.
	/// </summary>
	private readonly IDrawingPreference _userPreference;

	/// <summary>
	/// Indicates the shape kind.
	/// </summary>
	private ShapeKind _shapeKind;

	/// <summary>
	/// Indicates the target shape.
	/// </summary>
	private Shape _shape = null!;


	/// <summary>
	/// Initializes a <see cref="CellMark"/> instance via the specified user preference.
	/// </summary>
	/// <param name="userPreference">The user preference instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellMark(IDrawingPreference userPreference) : this(ShapeKind.None, userPreference)
	{
	}

	/// <summary>
	/// Initializes a <see cref="CellMark"/> instance via the specified shape kind.
	/// </summary>
	/// <param name="shapeKind">The shape kind.</param>
	/// <param name="userPreference">The user preference instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellMark(ShapeKind shapeKind, IDrawingPreference userPreference)
		=> (ShapeKind, _userPreference, _rectangle, _circle) = (
			shapeKind,
			userPreference,
			new()
			{
				Margin = new(5),
				Fill = new SolidColorBrush(userPreference.AuthorDefinedCellRectangleFillColor),
				Visibility = Visibility.Collapsed
			},
			new()
			{
				Margin = new(5),
				Fill = new SolidColorBrush(userPreference.AuthorDefinedCellCircleFillColor),
				Visibility = Visibility.Collapsed
			}
		);


	/// <summary>
	/// Indicates the shape kind used.
	/// </summary>
	public ShapeKind ShapeKind
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _shapeKind;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_shapeKind == value)
			{
				return;
			}

			if (!Enum.IsDefined(value))
			{
				throw new InvalidOperationException("The specified value is not defined.");
			}

			_shapeKind = value;

			switch (value)
			{
				case ShapeKind.None:
				{
					_shape = _rectangle;
					_shape.Visibility = Visibility.Collapsed;

					break;
				}
				default:
				{
					// Hides the old shape control.
					_shape.Visibility = Visibility.Collapsed;

					// Assigns a new control, and displays it.
					ref var newControl = ref _shape;
					newControl = value switch { ShapeKind.Rectangle => _rectangle, ShapeKind.Circle => _circle, _ => default! };
					newControl.Visibility = Visibility.Visible;

					break;
				}
			}
		}
	}


	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CellMark);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CellMark comparer && _shapeKind == comparer._shapeKind;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _shapeKind);

	/// <inheritdoc/>
	public override Shape GetControl() => _shape;
}

#endif