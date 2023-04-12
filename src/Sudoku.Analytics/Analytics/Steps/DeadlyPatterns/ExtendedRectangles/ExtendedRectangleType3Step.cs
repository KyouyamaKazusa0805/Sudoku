namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 3</b> technique.
/// </summary>
public sealed class ExtendedRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	scoped in CellMap subsetCells,
	Mask subsetDigitsMask,
	int house
) : ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <summary>
	/// Indicates the house that subset formed.
	/// </summary>
	public int House { get; } = house;

	/// <summary>
	/// Indicates the subset digits used.
	/// </summary>
	public Mask SubsetDigitsMask { get; } = subsetDigitsMask;

	/// <summary>
	/// Indicates the extra cells used that can form the subset.
	/// </summary>
	public CellMap SubsetCells { get; } = subsetCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], (ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr, HouseStr } },
			{ "zh", new[] { DigitsStr, CellsStr, HouseStr, ExtraCellsStr, ExtraDigitsStr } }
		};

	private string ExtraDigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string ExtraCellsStr => SubsetCells.ToString();

	private string HouseStr => HouseFormatter.Format(1 << House);
}
