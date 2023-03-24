namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern XZ</b> technique.
/// </summary>
public sealed class ChromaticPatternXzStep(
	Conclusion[] conclusions,
	View[]? views,
	int[] blocks,
	scoped in CellMap pattern,
	scoped in CellMap cells,
	int extraCell,
	short digitsMask,
	short extraDigitsMask
) : ChromaticPatternStep(conclusions, views, blocks, pattern, digitsMask)
{
	/// <summary>
	/// Indicates the mask of extra digits.
	/// </summary>
	public short ExtraDigitsMask { get; } = extraDigitsMask;

	/// <summary>
	/// Indicates the extra cell used.
	/// </summary>
	public int ExtraCell { get; } = extraCell;

	/// <inheritdoc/>
	public override Technique Code => Technique.ChromaticPatternXzRule;

	/// <summary>
	/// Indicates the cells that contains extra digit.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .2M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, BlocksStr, ExtraCellStr } },
			{ "zh", new[] { BlocksStr, CellsStr, DigitsStr, ExtraCellStr } }
		};

	private string ExtraCellStr => RxCyNotation.ToCellString(ExtraCell);
}
