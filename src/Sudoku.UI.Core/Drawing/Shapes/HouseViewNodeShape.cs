namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a house view node shape.
/// </summary>
public sealed class HouseViewNodeShape : DrawingElement
{
	/// <summary>
	/// Indicates the visible table.
	/// </summary>
	private readonly bool[] _isVisibleTable = new bool[27];

	/// <summary>
	/// Indicates the color identifiers.
	/// </summary>
	private readonly Identifier[] _identifiers = new Identifier[27];

	/// <summary>
	/// Indicates the grid layout.
	/// </summary>
	private readonly GridLayout _gridLayout = new GridLayout()
		.WithRowDefinitionsCount(9)
		.WithColumnDefinitionsCount(9);

	/// <summary>
	/// Indicates the inner rectangles.
	/// </summary>
	private readonly Rectangle[] _rectangles = new Rectangle[27];

	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;


	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	public IDrawingPreference Preference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		init
		{
			_preference = value;

			int i = 0;
			while (i < 9)
			{
				_gridLayout.AddChildren(
					_rectangles[i] = new Rectangle()
						.WithGridLayout(row: i / 3 * 3, column: i % 3 * 3, rowSpan: 3, columnSpan: 3)
						.WithOpacity(0)
						.WithOpacityTransition(TimeSpan.FromMilliseconds(500))
				);

				i++;
			}
			while (i < 18)
			{
				_gridLayout.AddChildren(
					_rectangles[i] = new Rectangle()
						.WithGridLayout(row: i - 9, columnSpan: 9)
						.WithOpacity(0)
						.WithOpacityTransition(TimeSpan.FromMilliseconds(500))
				);

				i++;
			}
			while (i < 27)
			{
				_gridLayout.AddChildren(
					_rectangles[i] = new Rectangle()
						.WithGridLayout(column: i - 18, rowSpan: 9)
						.WithOpacity(0)
						.WithOpacityTransition(TimeSpan.FromMilliseconds(500))
				);

				i++;
			}
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(HouseViewNodeShape);


	/// <summary>
	/// Sets the visibility at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <param name="isVisible">The visibility.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetIsVisible(int house, bool isVisible)
	{
		_isVisibleTable[house] = isVisible;
		_rectangles[house].Opacity = isVisible ? 1 : 0;
	}

	/// <summary>
	/// Sets the color identifier at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <param name="colorIdentifier">The color identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetIdentifier(int house, Identifier colorIdentifier)
	{
		var targetColor = colorIdentifier.AsColor(_preference);

		_identifiers[house] = colorIdentifier;
		_rectangles[house].Fill = new SolidColorBrush(targetColor with { A = 64 });
		_rectangles[house].Stroke = new SolidColorBrush(targetColor);
		_rectangles[house].StrokeThickness = _preference.HouseViewNodeStrokeThickness;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is HouseViewNodeShape comparer && Enumerable.SequenceEqual(comparer._identifiers, _identifiers)
			&& Enumerable.SequenceEqual(comparer._identifiers, _identifiers);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		result.Add(TypeIdentifier);

		for (int i = 0; i < 27; i++)
		{
			result.Add(_rectangles[i]);
		}

		return result.ToHashCode();
	}

	/// <summary>
	/// Gets the visibility at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The visibility.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool GetVisible(int house) => _isVisibleTable[house];

	/// <summary>
	/// Gets the color identifier at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The color identifier.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Identifier GetIdentifier(int house) => _identifiers[house];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;
}
