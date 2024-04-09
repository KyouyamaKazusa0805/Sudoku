namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-sector Locked Sets</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used in this pattern.</param>
/// <param name="rowsCount">Indicates the number of rows used.</param>
/// <param name="columnsCount">Indicates the number of columns used.</param>
public sealed partial class MultisectorLockedSetsStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] scoped ref readonly CellMap cells,
	[PrimaryConstructorParameter] int rowsCount,
	[PrimaryConstructorParameter] int columnsCount
) : LockedSetStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 94;

	/// <inheritdoc/>
	public override Technique Code => Technique.MultisectorLockedSets;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CellsCountStr, CellsStr]), new(ChineseLanguage, [CellsCountStr, CellsStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new MultisectorLockedSetsSizeFactor()];

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Options.Converter.CellConverter(Cells);
}
