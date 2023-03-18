namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 3</b> technique.
/// </summary>
public sealed class BorescoperDeadlyPatternType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap map,
	short digitsMask,
	scoped in CellMap subsetCells,
	short subsetDigitsMask
) : BorescoperDeadlyPatternStep(conclusions, views, map, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <summary>
	/// Indicates the mask of subset digits used.
	/// </summary>
	public short SubsetDigitsMask { get; } = subsetDigitsMask;

	/// <summary>
	/// Indicates the cells that the subset used.
	/// </summary>
	public CellMap SubsetCells { get; } = subsetCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, SubsetCells.Count * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr } },
			{ "zh", new[] { DigitsStr, CellsStr, ExtraCellsStr, ExtraDigitsStr } }
		};

	private string ExtraDigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string ExtraCellsStr => SubsetCells.ToString();
}
