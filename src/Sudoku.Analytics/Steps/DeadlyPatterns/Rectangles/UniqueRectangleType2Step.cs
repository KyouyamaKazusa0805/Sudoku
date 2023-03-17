namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 2</b> technique.
/// </summary>
public sealed class UniqueRectangleType2Step(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	Technique code,
	scoped in CellMap cells,
	bool isAvoidable,
	int extraDigit,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	code,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, ExtraDigitStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, ExtraDigitStr } }
		};

	private string ExtraDigitStr => (extraDigit + 1).ToString();
}
