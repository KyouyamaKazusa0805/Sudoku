#if AUTHOR_FEATURE_CELL_MARKS

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a cell mark.
/// </summary>
internal sealed class CellMark : DrawingElement
{
	/// <summary>
	/// Indicates the inner rectangle.
	/// </summary>
	/// <remarks><b>
	/// The prefix <c>_control</c> is appended on purpose, which is used for
	/// checking in method <see cref="GetControls"/>.
	/// </b></remarks>
	/// <seealso cref="GetControls"/>
	private readonly Rectangle _controlRectangle;

	/// <summary>
	/// Indicates the inner circle.
	/// </summary>
	/// <remarks><b>
	/// The prefix <c>_control</c> is appended on purpose, which is used for
	/// checking in method <see cref="GetControls"/>.
	/// </b></remarks>
	/// <seealso cref="GetControls"/>
	private readonly Ellipse _controlCircle;

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
	{
		(ShapeKind, _userPreference, _controlRectangle, _controlCircle) = (
			shapeKind,
			userPreference,
			new()
			{
				Margin = new(5),
				Fill = new SolidColorBrush(userPreference.AuthorDefined_CellRectangleFillColor),
				Visibility = Visibility.Collapsed
			},
			new()
			{
				Margin = new(5),
				Fill = new SolidColorBrush(userPreference.AuthorDefined_CellCircleFillColor),
				Visibility = Visibility.Collapsed
			}
		);

		_shape = _controlRectangle;
	}


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
			if (_shapeKind == value || !Enum.IsDefined(value))
			{
				return;
			}

			_shapeKind = value;
			switch (value)
			{
				case ShapeKind.None:
				{
					// Hides all possible controls.
					Array.ForEach(GetControls(), static control => control.Visibility = Visibility.Collapsed);

					break;
				}
				default:
				{
					// Hides the old shape control.
					_shape.Visibility = Visibility.Collapsed;

					// Assigns a new control, and displays it.
					ref var newControl = ref _shape;
					newControl = value switch { ShapeKind.Rectangle => _controlRectangle, ShapeKind.Circle => _controlCircle, _ => default! };
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

	/// <summary>
	/// Try to get all controls used.
	/// </summary>
	/// <returns>All controls.</returns>
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	public Shape[] GetControls()
	{
		const string prefix = "_control";
		const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
		return (
			from fieldInfo in typeof(CellMark).GetFields(bindingFlags)
			where fieldInfo.Name.StartsWith(prefix) && fieldInfo.FieldType.IsAssignableTo(typeof(Shape))
			select (Shape)fieldInfo.GetValue(this)!
		).ToArray();
	}
}

#endif