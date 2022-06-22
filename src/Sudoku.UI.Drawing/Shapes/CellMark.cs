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
	/// Indicates the default margin value that is applied to a built-in control.
	/// </summary>
	private static readonly Thickness BuiltinShapeDefaultMargin = new(10);


	/// <summary>
	/// Indicates the inner rectangle.
	/// </summary>
	/// <remarks><b>
	/// The prefix <c>_control</c> is appended on purpose, which is used for
	/// checking in method <see cref="GetControls"/>.
	/// </b></remarks>
	/// <seealso cref="GetControls"/>
	[Shape(ShapeKind.Rectangle)]
	private readonly Rectangle _controlRectangle;

	/// <summary>
	/// Indicates the inner circle.
	/// </summary>
	/// <remarks><b>
	/// The prefix <c>_control</c> is appended on purpose, which is used for
	/// checking in method <see cref="GetControls"/>.
	/// </b></remarks>
	/// <seealso cref="GetControls"/>
	[Shape(ShapeKind.Circle)]
	private readonly Ellipse _controlCircle;

	/// <summary>
	/// Indicates the inner cross mark.
	/// </summary>
	[Shape(ShapeKind.CrossMark)]
	private readonly CrossMark _controlCrossMark;

	/// <summary>
	/// Indicates the inner star mark.
	/// </summary>
	[Shape(ShapeKind.Star)]
	private readonly StarMark _controlStarMark;

	/// <summary>
	/// Indicates the inner triangle mark.
	/// </summary>
	[Shape(ShapeKind.Triangle)]
	private readonly TriangleMark _controlTriangleMark;

	/// <summary>
	/// Indicates the inner diamond mark.
	/// </summary>
	[Shape(ShapeKind.Diamond)]
	private readonly DiamondMark _controlDiamondMark;

	/// <summary>
	/// Indicates the user preference.
	/// </summary>
	private readonly IDrawingPreference _userPreference;

	/// <summary>
	/// Indicates whether the cell mark is shown.
	/// </summary>
	private bool _showMark;

	/// <summary>
	/// Indicates the shape kind.
	/// </summary>
	private ShapeKind _shapeKind;

	/// <summary>
	/// Indicates the previous state of the visibility case.
	/// </summary>
	private Visibility? _previousVisibilityState;

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
		ShapeKind = shapeKind;
		_userPreference = userPreference;
		_controlRectangle = new()
		{
			Margin = BuiltinShapeDefaultMargin,
			Fill = new SolidColorBrush(userPreference.AuthorDefined_CellRectangleFillColor),
			Visibility = Visibility.Collapsed
		};
		_controlCircle = new()
		{
			Margin = BuiltinShapeDefaultMargin,
			Fill = new SolidColorBrush(userPreference.AuthorDefined_CellCircleFillColor),
			Visibility = Visibility.Collapsed
		};
		_controlCrossMark = new()
		{
			Margin = DefaultMargin,
			Stroke = new SolidColorBrush(userPreference.AuthorDefined_CrossMarkStrokeColor),
			StrokeThickness = userPreference.AuthorDefined_CrossMarkStrokeThickness,
			Visibility = Visibility.Collapsed
		};
		_controlStarMark = new()
		{
			Margin = DefaultMargin,
			Fill = new SolidColorBrush(userPreference.AuthorDefined_StarFillColor),
			Visibility = Visibility.Collapsed
		};
		_controlTriangleMark = new()
		{
			Margin = DefaultMargin,
			Fill = new SolidColorBrush(userPreference.AuthorDefined_TriangleFillColor),
			Visibility = Visibility.Collapsed
		};
		_controlDiamondMark = new()
		{
			Margin = DefaultMargin,
			Fill = new SolidColorBrush(userPreference.AuthorDefined_DiamondFillColor),
			Visibility = Visibility.Collapsed
		};

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
					newControl = GetControlViaShapeKind(value)!;
					newControl.Visibility = Visibility.Visible;

					break;
				}
			}
		}
	}

	/// <summary>
	/// Indicates the visibility of the shape.
	/// </summary>
	public bool ShowMark
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showMark;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (value, _previousVisibilityState)
			{
				case (true, { } previousState):
				{
					_showMark = value;

					_shape.Visibility = previousState;

					break;
				}
				case (false, _):
				{
					_showMark = value;

					_previousVisibilityState = _shape.Visibility;
					_shape.Visibility = Visibility.Collapsed;

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FrameworkElement[] GetControls()
		=> (
			from fieldInfo in typeof(CellMark).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
			where fieldInfo.IsDefined(typeof(ShapeAttribute))
			select (FrameworkElement)fieldInfo.GetValue(this)!
		).ToArray();

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private FrameworkElement? GetControlViaShapeKind(ShapeKind shapeKind)
		=> (
			from fieldInfo in typeof(CellMark).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
			where fieldInfo.GetCustomAttribute<ShapeAttribute>()?.Kind == shapeKind
			select (FrameworkElement)fieldInfo.GetValue(this)!
		).FirstOrDefault();
}

#endif