namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="extraCell1">Indicates the first extra digit used.</param>
/// <param name="extraCell2">Indicates the second extra digit used.</param>
public sealed partial class FireworkPairType1Step(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped in CellMap cells,
	[DataMember] Mask digitsMask,
	[DataMember] Cell extraCell1,
	[DataMember] Cell extraCell2
) : FireworkStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkPairType1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [CellsStr, DigitsStr, ExtraCell1Str, ExtraCell2Str]),
			new(ChineseLanguage, [CellsStr, DigitsStr, ExtraCell1Str, ExtraCell2Str])
		];

	private string CellsStr => Cells.ToString();

	private string DigitsStr => DigitNotation.ToString(DigitsMask);

	private string ExtraCell1Str => CellNotation.ToString(ExtraCell1);

	private string ExtraCell2Str => CellNotation.ToString(ExtraCell2);
}
