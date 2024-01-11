namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Uniqueness Clue Cover</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="extraCells">Indicates the extra cells.</param>
/// <param name="extraDigits">Indicates the extra digits.</param>
/// <param name="chuteIndex">Indicates the chute index.</param>
public sealed partial class UniquenessClueCoverStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[RecordParameter] scoped ref readonly CellMap extraCells,
	[RecordParameter] Mask extraDigits,
	[RecordParameter] int chuteIndex
) : DeadlyPatternStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.UniquenessClueCover;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.Size, A004526(ExtraCells.Count) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [ChuteString, ChuteCellsString]),
			new(ChineseLanguage, [ChuteString, ChuteCellsString]),
		];

	private string ChuteString => Options.Converter.ChuteConverter([Chutes[ChuteIndex]]);

	private string ChuteCellsString => Options.Converter.CellConverter(Chutes[ChuteIndex].Cells);
}
