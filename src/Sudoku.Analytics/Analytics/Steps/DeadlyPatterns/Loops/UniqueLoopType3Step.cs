namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 3</b> technique.
/// </summary>
public sealed class UniqueLoopType3Step(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap loop,
	Mask subsetDigitsMask,
	scoped in CellMap subsetCells
) : UniqueLoopStep(conclusions, views, digit1, digit2, loop)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <summary>
	/// Indicates the mask that contains the subset digits used in this instance.
	/// </summary>
	public Mask SubsetDigitsMask { get; } = subsetDigitsMask;

	/// <summary>
	/// Indicates the cells that are subset cells.
	/// </summary>
	public CellMap SubsetCells { get; } = subsetCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], new(ExtraDifficultyCaseNames.Size, SubsetCells.Count * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr } }
		};

	private string SubsetCellsStr => SubsetCells.ToString();

	private string DigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string SubsetName => R[$"SubsetNamesSize{SubsetCells.Count + 1}"]!;
}
