namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Domino Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
public sealed partial class DominoLoopStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] scoped in CellMap cells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.6M;

	/// <inheritdoc/>
	public override Technique Code => Technique.DominoLoop;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { EnglishLanguage, new[] { CellsCountStr, CellsStr } }, { ChineseLanguage, new[] { CellsCountStr, CellsStr } } };

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
