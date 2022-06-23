#if AUTHOR_FEATURE_CANDIDATE_MARKS

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a candidate mark.
/// </summary>
internal sealed class CandidateMark : DrawingElement
{
	/// <summary>
	/// Indicates the grid layout.
	/// </summary>
	private readonly GridLayout _gridLayout;

	/// <summary>
	/// Indicates the inner circles.
	/// </summary>
	private readonly Ellipse[] _ellipses = new Ellipse[9];


	/// <summary>
	/// Initializes a <see cref="CandidateMark"/> instance via the specified stroke color and thickness.
	/// </summary>
	/// <param name="strokeColor">The stroke color.</param>
	/// <param name="strokeThickness">The stroke thickness.</param>
	public CandidateMark(Color strokeColor, double strokeThickness)
	{
		_gridLayout = new();
		_gridLayout.RowDefinitions.Add(new());
		_gridLayout.RowDefinitions.Add(new());
		_gridLayout.RowDefinitions.Add(new());
		_gridLayout.ColumnDefinitions.Add(new());
		_gridLayout.ColumnDefinitions.Add(new());
		_gridLayout.ColumnDefinitions.Add(new());

		for (int i = 0; i < 9; i++)
		{
			ref var current = ref _ellipses[i];
			current = new()
			{
				Stroke = new SolidColorBrush(strokeColor),
				StrokeThickness = strokeThickness,
				Visibility = Visibility.Collapsed
			};

			GridLayout.SetRow(current, i / 3);
			GridLayout.SetColumn(current, i % 3);
			_gridLayout.Children.Add(current);
		}
	}


	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CandidateMark);


	/// <summary>
	/// Sets the specified color at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="color">The color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDigitColor(int digit, Color color)
	{
		if (GetStrokeColor(digit) == color)
		{
			return;
		}

		var current = _ellipses[digit];
		if (color == Colors.Transparent)
		{
			current.Visibility = Visibility.Collapsed;
		}
		else
		{
			current.Visibility = Visibility.Visible;
			current.Stroke = new SolidColorBrush(color);
		}
	}

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

		_ellipses[digit].StrokeThickness = strokeThickness;
	}

	/// <summary>
	/// Hides the circle control at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void HideDigit(int digit) => SetDigitColor(digit, Colors.Transparent);

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
	public double GetStrokeThickness(int digit) => _ellipses[digit].StrokeThickness;

	/// <summary>
	/// Gets the color at the specified digit's place.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The color.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color GetStrokeColor(int digit) => ((SolidColorBrush)_ellipses[digit].Stroke).Color;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;
}

#endif