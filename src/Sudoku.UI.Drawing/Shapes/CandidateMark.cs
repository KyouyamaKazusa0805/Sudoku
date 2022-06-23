#if AUTHOR_FEATURE_CANDIDATE_MARKS

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a candidate mark.
/// </summary>
internal sealed class CandidateMark : DrawingElement
{
	/// <summary>
	/// Indicates the supported shapes.
	/// </summary>
	public static readonly ShapeKind[] SupportedShapes = new[]
	{
		ShapeKind.None,
		ShapeKind.Rectangle,
		ShapeKind.Circle,
		ShapeKind.CrossMark,
		ShapeKind.Triangle
	};


	/// <summary>
	/// Indicates the states of showing marks.
	/// </summary>
	private readonly bool[] _showMarkStates = new bool[9];

	/// <summary>
	/// Indicates the previous visibilities.
	/// </summary>
	private readonly Visibility?[] _previousVisibilities = new Visibility?[9];

	/// <summary>
	/// Indicates the shape kinds.
	/// </summary>
	private readonly ShapeKind[] _shapeKinds = new ShapeKind[9];

	/// <summary>
	/// Indicates the grid layout.
	/// </summary>
	private readonly GridLayout _gridLayout;

	/// <summary>
	/// Indicates the current control.
	/// </summary>
	private readonly FrameworkElement[] _currentControls = new FrameworkElement[9];

	/// <summary>
	/// Indicates the list of controls.
	/// </summary>
	private readonly FrameworkElement[][] _pool = new FrameworkElement[9][];

	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	private readonly IDrawingPreference _userPreference;


	/// <summary>
	/// Initializes a <see cref="CandidateMark"/> instance via the specified stroke thickness.
	/// </summary>
	/// <param name="userPreference">The user preference instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidateMark(IDrawingPreference userPreference)
	{
		_userPreference = userPreference;
		_gridLayout = new();
		_gridLayout.RowDefinitions.Add(new());
		_gridLayout.RowDefinitions.Add(new());
		_gridLayout.RowDefinitions.Add(new());
		_gridLayout.ColumnDefinitions.Add(new());
		_gridLayout.ColumnDefinitions.Add(new());
		_gridLayout.ColumnDefinitions.Add(new());

		for (int i = 0; i < 9; i++)
		{
			var c1 = new Rectangle
			{
				Stroke = new SolidColorBrush(_userPreference.AuthorDefined_CandidateMarkStrokeColor),
				StrokeThickness = _userPreference.AuthorDefined_CandidateMarkStrokeThickness,
				Visibility = Visibility.Collapsed
			};
			GridLayout.SetRow(c1, i / 3);
			GridLayout.SetColumn(c1, i % 3);
			_gridLayout.Children.Add(c1);

			var c2 = new Ellipse
			{
				Stroke = new SolidColorBrush(_userPreference.AuthorDefined_CandidateMarkStrokeColor),
				StrokeThickness = _userPreference.AuthorDefined_CandidateMarkStrokeThickness,
				Visibility = Visibility.Collapsed
			};
			GridLayout.SetRow(c2, i / 3);
			GridLayout.SetColumn(c2, i % 3);
			_gridLayout.Children.Add(c2);

			var c3 = new CrossMark
			{
				Stroke = new SolidColorBrush(_userPreference.AuthorDefined_CandidateMarkStrokeColor),
				StrokeThickness = _userPreference.AuthorDefined_CandidateMarkStrokeThickness,
				Visibility = Visibility.Collapsed
			};
			GridLayout.SetRow(c3, i / 3);
			GridLayout.SetColumn(c3, i % 3);
			_gridLayout.Children.Add(c3);

			var c4 = new TriangleMark
			{
				Stroke = new SolidColorBrush(_userPreference.AuthorDefined_CandidateMarkStrokeColor),
				StrokeThickness = _userPreference.AuthorDefined_CandidateMarkStrokeThickness,
				Visibility = Visibility.Collapsed
			};
			GridLayout.SetRow(c4, i / 3);
			GridLayout.SetColumn(c4, i % 3);
			_gridLayout.Children.Add(c4);

			ref var poolOfDigit = ref _pool[i];
			poolOfDigit = new FrameworkElement[Enum.GetValues<ShapeKind>().Length];
			poolOfDigit[(int)ShapeKind.Rectangle] = c1;
			poolOfDigit[(int)ShapeKind.Circle] = c2;
			poolOfDigit[(int)ShapeKind.CrossMark] = c3;
			poolOfDigit[(int)ShapeKind.Triangle] = c4;

			_currentControls[i] = c1;
		}
	}


	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CandidateMark);


	/// <summary>
	/// Sets the stroke thickness value at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="strokeThickness">The stroke thickness.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStrokeThickness(int digit, double strokeThickness)
	{
		if (strokeThickness <= 0 || GetStrokeThickness(digit).NearlyEquals(strokeThickness, 1E-2))
		{
			return;
		}

		((dynamic)_currentControls[digit]).StrokeThickness = strokeThickness;
	}

	/// <summary>
	/// Sets the specified shape.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="shapeKind">The shape kind.</param>
	/// <exception cref="NotSupportedException">Throws when the specified shape kind is not supported.</exception>
	public void SetShapeKind(int digit, ShapeKind shapeKind)
	{
		if (GetShapeKind(digit) == shapeKind || !Enum.IsDefined(shapeKind))
		{
			return;
		}

		_shapeKinds[digit] = shapeKind;
		if (shapeKind == ShapeKind.None)
		{
			// Hides all possible controls.
			Array.ForEach(_pool[digit], static c => _ = c is not null ? c.Visibility = Visibility.Collapsed : default);
		}
		else if (_pool[digit][(int)shapeKind] is { } control)
		{
			// Hides the old shape control.
			_currentControls[digit].Visibility = Visibility.Collapsed;

			// Displays the new control.
			control.Visibility = Visibility.Visible;
			_currentControls[digit] = control;
		}
		else
		{
			throw new NotSupportedException("The specified shape is not supported.");
		}
	}

	/// <summary>
	/// Sets the show mark value at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="showMark">Indicates whether the candidate mark will be shown.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetShowMark(int digit, bool showMark)
	{
		switch (showMark, _previousVisibilities[digit])
		{
			case (true, { } previousState):
			{
				_showMarkStates[digit] = showMark;
				_currentControls[digit].Visibility = previousState;

				break;
			}
			case (false, _):
			{
				_showMarkStates[digit] = showMark;

				_previousVisibilities[digit] = _currentControls[digit].Visibility;
				_currentControls[digit].Visibility = Visibility.Collapsed;

				break;
			}
		}
	}

	/// <summary>
	/// Hides the circle control at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void HideDigit(int digit) => _currentControls[digit].Visibility = Visibility.Collapsed;

	/// <summary>
	/// Gets the state that describes whether the current digit shows marks.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool GetShowMark(int digit) => _showMarkStates[digit];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CandidateMark comparer && _gridLayout == comparer._gridLayout;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _gridLayout.GetHashCode());

	/// <summary>
	/// Gets the stroke thickness at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The stroke thickness.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double GetStrokeThickness(int digit) => ((dynamic)_currentControls[digit]).StrokeThickness;

	/// <summary>
	/// Gets the shape at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The shape kind.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ShapeKind GetShapeKind(int digit) => _shapeKinds[digit];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;
}

#endif