namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 2</b> technique.
/// </summary>
public sealed class BorescoperDeadlyPatternType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap map,
	short digitsMask,
	int extraDigit
) : BorescoperDeadlyPatternStep(conclusions, views, map, digitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override int Type => 2;

	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitStr } },
			{ "zh", new[] { DigitsStr, CellsStr, ExtraDigitStr } }
		};

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
