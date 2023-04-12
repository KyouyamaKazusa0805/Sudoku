namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
public sealed class AlmostLockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	Mask digitsMask,
	scoped in CellMap baseCells,
	scoped in CellMap targetCells,
	bool hasValueCell
) : IntersectionStep(conclusions, views)
{
	/// <summary>
	/// Indicates whether the step contains value cells.
	/// </summary>
	public bool HasValueCell { get; } = hasValueCell;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <summary>
	/// Indicates the mask that contains the digits used.
	/// </summary>
	public Mask DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique Code => (Technique)((int)Technique.AlmostLockedPair + Size - 2);

	/// <summary>
	/// Indicates the base cells.
	/// </summary>
	public CellMap BaseCells { get; } = baseCells;

	/// <summary>
	/// Indicates the target cells.
	/// </summary>
	public CellMap TargetCells { get; } = targetCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .7M, 4 => 1.2M }),
			(ExtraDifficultyCaseNames.ValueCell, HasValueCell ? Size switch { 2 or 3 => .1M, 4 => .2M } : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, BaseCellsStr, TargetCellsStr } },
			{ "zh", new[] { DigitsStr, BaseCellsStr, TargetCellsStr } }
		};

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string BaseCellsStr => BaseCells.ToString();

	private string TargetCellsStr => TargetCells.ToString();
}
