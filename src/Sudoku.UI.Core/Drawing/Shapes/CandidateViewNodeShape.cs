namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a candidate view node shape.
/// </summary>
public sealed class CandidateViewNodeShape : DrawingElement
{
	/// <summary>
	/// Indicates the table of visibility values.
	/// </summary>
	private readonly bool[] _isVisibleValues = new bool[9];

	/// <summary>
	/// Indicates the identifiers.
	/// </summary>
	private readonly Identifier[] _identifiers = new Identifier[9];

	/// <summary>
	/// The inner controls.
	/// </summary>
	private readonly Ellipse[] _ellipses = new Ellipse[9];

	/// <summary>
	/// Indicates the grid layout.
	/// </summary>
	private readonly GridLayout _gridLayout = new GridLayout()
		.WithRowDefinitionsCount(3)
		.WithColumnDefinitionsCount(3);

	/// <summary>
	/// Indicates the preference instance.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;


	/// <summary>
	/// The user preference instance.
	/// </summary>
	public required IDrawingPreference Preference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		init
		{
			_preference = value;

			for (int i = 0; i < 9; i++)
			{
				ref var ellipse = ref _ellipses[i];
				ellipse = new Ellipse().WithGridLayout(row: i / 3, column: i % 3);
				_gridLayout.Children.Add(ellipse);
			}
		}
	}


	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CandidateViewNodeShape);


	/// <summary>
	/// Sets the specified digit with the specified visibility.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="isVisible">The visibility.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetIsVisible(int digit, bool isVisible)
	{
		if (_isVisibleValues[digit] == isVisible)
		{
			return;
		}

		_isVisibleValues[digit] = isVisible;
		_ellipses[digit].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
	}

	/// <summary>
	/// Sets the specified digit with the specified color.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="identifier">The color identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetIdentifier(int digit, Identifier identifier)
	{
		if (_identifiers[digit] == identifier)
		{
			return;
		}

		_identifiers[digit] = identifier;
		_ellipses[digit].Fill = new SolidColorBrush(identifier.AsColor(_preference));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is CandidateViewNodeShape comparer && Enumerable.SequenceEqual(_identifiers, comparer._identifiers);

	/// <summary>
	/// Gets the visibility at the specified digit.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The visibility.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool GetIsVisible(int digit) => _isVisibleValues[digit];

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		for (int i = 0; i < 9; i++)
		{
			result.Add(_identifiers.GetHashCode());
		}

		return result.ToHashCode();
	}

	/// <summary>
	/// Gets the identifier at the specified digit.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The color identifier.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Identifier GetIdentifier(int digit) => _identifiers[digit];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;
}
