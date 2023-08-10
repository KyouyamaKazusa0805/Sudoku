namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-sector Locked Sets</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used in this pattern.</param>
public sealed partial class MultisectorLockedSetsStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped in CellMap cells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.MultisectorLockedSets;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Size, A002024(Cells.Count) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CellsCountStr, CellsStr]), new(ChineseLanguage, [CellsCountStr, CellsStr])];

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
