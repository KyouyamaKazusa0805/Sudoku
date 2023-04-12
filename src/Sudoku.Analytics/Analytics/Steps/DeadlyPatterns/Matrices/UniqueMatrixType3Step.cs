namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 3</b> technique.
/// </summary>
public sealed class UniqueMatrixType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	Mask subsetDigitsMask,
	scoped in CellMap subsetCells
) : UniqueMatrixStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <summary>
	/// Indicates the mask that describes the extra digits used in the subset.
	/// </summary>
	public Mask SubsetDigitsMask { get; } = subsetDigitsMask;

	/// <summary>
	/// Indicates the cells that the subset used.
	/// </summary>
	public CellMap SubsetCells { get; } = subsetCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitStr, ExtraCellsStr, SubsetName } },
			{ "zh", new[] { ExtraDigitStr, ExtraCellsStr, SubsetName, DigitsStr, CellsStr } }
		};

	private string ExtraCellsStr => SubsetCells.ToString();

	private string ExtraDigitStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string SubsetName => R[$"SubsetNamesSize{SubsetCells.Count + 1}"]!;
}
