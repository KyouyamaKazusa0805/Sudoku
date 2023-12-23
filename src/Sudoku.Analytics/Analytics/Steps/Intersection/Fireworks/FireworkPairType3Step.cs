namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="emptyRectangleBlock">Indicates the block index that empty rectangle forms.</param>
public sealed partial class FireworkPairType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] scoped ref readonly CellMap cells,
	[Data] Mask digitsMask,
	[Data] House emptyRectangleBlock
) : FireworkStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType3;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CellsStr, DigitsStr, EmptyRectangleStr]), new(ChineseLanguage, [CellsStr, DigitsStr, EmptyRectangleStr])];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string EmptyRectangleStr => Options.Converter.HouseConverter(1 << EmptyRectangleBlock);
}
