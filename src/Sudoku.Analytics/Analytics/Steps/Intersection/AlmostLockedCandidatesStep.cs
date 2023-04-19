namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask that contains the digits used.</param>
/// <param name="baseCells">Indicates the base cells.</param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="hasValueCell">Indicates whether the step contains value cells.</param>
public sealed partial class AlmostLockedCandidatesStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] scoped in CellMap baseCells,
	[PrimaryConstructorParameter] scoped in CellMap targetCells,
	[PrimaryConstructorParameter] bool hasValueCell
) : IntersectionStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique Code => (Technique)((int)Technique.AlmostLockedPair + Size - 2);

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
