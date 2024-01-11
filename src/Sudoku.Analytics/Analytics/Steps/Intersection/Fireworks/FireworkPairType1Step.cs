namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="extraCell1">Indicates the first extra digit used.</param>
/// <param name="extraCell2">Indicates the second extra digit used.</param>
public sealed partial class FireworkPairType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[RecordParameter] scoped ref readonly CellMap cells,
	[RecordParameter] Mask digitsMask,
	[RecordParameter] Cell extraCell1,
	[RecordParameter] Cell extraCell2
) : FireworkStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, DigitsStr, ExtraCell1Str, ExtraCell2Str]),
			new(ChineseLanguage, [CellsStr, DigitsStr, ExtraCell1Str, ExtraCell2Str])
		];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string ExtraCell1Str => Options.Converter.CellConverter([ExtraCell1]);

	private string ExtraCell2Str => Options.Converter.CellConverter([ExtraCell2]);
}
