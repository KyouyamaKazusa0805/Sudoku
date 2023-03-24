namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 4</b> technique.
/// </summary>
public sealed class BorescoperDeadlyPatternType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap map,
	short digitsMask,
	scoped in CellMap conjugateHouse,
	short extraDigitsMask
) : BorescoperDeadlyPatternStep(conclusions, views, map, digitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override int Type => 4;

	/// <summary>
	/// Indicates the mask of extra digits used.
	/// </summary>
	public short ExtraDigitsMask { get; } = extraDigitsMask;

	/// <summary>
	/// Indicates the cells used as generialized conjugate.
	/// </summary>
	public CellMap ConjugateHouse { get; } = conjugateHouse;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ConjHouseStr, ExtraCombStr } },
			{ "zh", new[] { DigitsStr, CellsStr, ExtraCombStr, ConjHouseStr } }
		};

	private string ExtraCombStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string ConjHouseStr => ConjugateHouse.ToString();
}
