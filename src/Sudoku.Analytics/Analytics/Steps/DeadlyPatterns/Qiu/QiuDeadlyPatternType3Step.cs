namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 3</b> technique.
/// </summary>
public sealed class QiuDeadlyPatternType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	Mask subsetDigitsMask,
	scoped in CellMap subsetCells,
	bool isNaked
) : QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <summary>
	/// Indicates whether the subset is naked one.
	/// </summary>
	public bool IsNaked { get; } = isNaked;

	/// <inheritdoc/>
	public override int Type => 3;

	/// <summary>
	/// Indicates the mask of subset digits used.
	/// </summary>
	public Mask SubsetDigitsMask { get; } = subsetDigitsMask;

	/// <summary>
	/// Indicates the subset cells used.
	/// </summary>
	public CellMap SubsetCells { get; } = subsetCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { (ExtraDifficultyCaseNames.Size, PopCount((uint)SubsetDigitsMask) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { PatternStr, DigitsStr, CellsStr, SubsetName } },
			{ "zh", new[] { PatternStr, DigitsStr, CellsStr, SubsetName } }
		};

	private string DigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string CellsStr => SubsetCells.ToString();

	private string SubsetName => R[$"SubsetNamesSize{SubsetCells.Count + 1}"]!;
}
