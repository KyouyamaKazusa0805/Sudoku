#if AUTHOR_FEATURE_CELL_MARKS

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a cell mark.
/// </summary>
internal sealed class CellMark : DrawingElement
{
	/// <summary>
	/// Indicates the default margin value.
	/// </summary>
	private static readonly Thickness DefaultMargin = new(5);


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
	/// Indicates the inner cross mark.
	/// </summary>
	private readonly CrossMark _controlCrossMark;

	/// <summary>
	/// Indicates the inner star mark.
	/// </summary>
	private readonly StarMark _controlStarMark;

	/// <summary>
	/// Indicates the inner triangle mark.
	/// </summary>
	private readonly TriangleMark _controlTriangleMark;

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
	private FrameworkElement _shape;


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
		(ShapeKind, _userPreference, _controlRectangle, _controlCircle, _controlCrossMark, _controlStarMark, _controlTriangleMark) = (
			shapeKind,
			userPreference,
			new()
			{
				Margin = DefaultMargin,
				Fill = new SolidColorBrush(userPreference.AuthorDefined_CellRectangleFillColor),
				Visibility = Visibility.Collapsed
			},
			new()
			{
				Margin = DefaultMargin,
				Fill = new SolidColorBrush(userPreference.AuthorDefined_CellCircleFillColor),
				Visibility = Visibility.Collapsed
			},
			new()
			{
				Margin = DefaultMargin,
				Stroke = new SolidColorBrush(userPreference.AuthorDefined_CrossMarkStrokeColor),
				StrokeThickness = userPreference.AuthorDefined_CrossMarkStrokeThickness,
				Visibility = Visibility.Collapsed
			},
			new()
			{
				Margin = DefaultMargin,
				Fill = new SolidColorBrush(userPreference.AuthorDefined_StarFillColor),
				Visibility = Visibility.Collapsed
			},
			new()
			{
				Margin = DefaultMargin,
				Fill = new SolidColorBrush(userPreference.AuthorDefined_TriangleFillColor),
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
					Array.ForEach(GetControls(), CommonMethods.HideControl);

					break;
				}
				default:
				{
					// Hides the old shape control.
					_shape.Visibility = Visibility.Collapsed;

					// Assigns a new control, and displays it.
					ref var newControl = ref _shape;
					newControl = value switch
					{
						ShapeKind.Rectangle => _controlRectangle,
						ShapeKind.Circle => _controlCircle,
						ShapeKind.CrossMark => _controlCrossMark,
						ShapeKind.Star => _controlStarMark,
						ShapeKind.Triangle => _controlTriangleMark,
						_ => default!
					};
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
	public override FrameworkElement GetControl() => _shape;

	/// <summary>
	/// Try to get all controls used.
	/// </summary>
	/// <returns>All controls.</returns>
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	public FrameworkElement[] GetControls()
	{
		const string prefix = "_control";
		const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
		return (
			from fieldInfo in typeof(CellMark).GetFields(bindingFlags)
			where fieldInfo.Name.StartsWith(prefix) && fieldInfo.FieldType.IsAssignableTo(typeof(FrameworkElement))
			select (FrameworkElement)fieldInfo.GetValue(this)!
		).ToArray();
	}
}

#endif