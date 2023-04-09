namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle Intersection Pair</b> technique.
/// </summary>
public sealed class EmptyRectangleIntersectionPairStep(
	Conclusion[] conclusions,
	View[]? views,
	int startCell,
	int endCell,
	int house,
	int digit1,
	int digit2
) : AlmostLockedSetsStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.0M;

	/// <summary>
	/// Indicates the start cell to be calculated.
	/// </summary>
	public int StartCell { get; } = startCell;

	/// <summary>
	/// Indicates the end cell to be calculated.
	/// </summary>
	public int EndCell { get; } = endCell;

	/// <summary>
	/// Indicates the house index that the empty rectangle forms.
	/// </summary>
	public int House { get; } = house;

	/// <summary>
	/// Indicates the first digit used.
	/// </summary>
	public int Digit1 { get; } = digit1;

	/// <summary>
	/// Indicates the second digit used.
	/// </summary>
	public int Digit2 { get; } = digit2;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangleIntersectionPair;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, StartCellStr, EndCellStr, HouseStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, StartCellStr, EndCellStr, HouseStr } }
		};

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();

	private string StartCellStr => RxCyNotation.ToCellString(StartCell);

	private string EndCellStr => RxCyNotation.ToCellString(EndCell);

	private string HouseStr => HouseFormatter.Format(1 << House);
}
