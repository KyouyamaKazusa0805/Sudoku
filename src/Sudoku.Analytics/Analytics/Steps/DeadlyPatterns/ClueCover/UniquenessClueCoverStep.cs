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
	[PrimaryConstructorParameter] ref readonly CellMap extraCells,
	[PrimaryConstructorParameter] Mask extraDigits,
	[PrimaryConstructorParameter] int chuteIndex
) : DeadlyPatternStep(conclusions, views, options), IExtraCellListTrait
{
	/// <inheritdoc/>
	public override bool OnlyUseBivalueCells => false;

	/// <inheritdoc/>
	public override int BaseDifficulty => 65;

	/// <inheritdoc/>
	public override Technique Code => Technique.UniquenessClueCover;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [ChuteString, ChuteCellsString]),
			new(SR.ChineseLanguage, [ChuteString, ChuteCellsString]),
		];

	/// <inheritdoc/>
	public override FactorArray Factors => [new UniquenessClueCoverExtraCellsFactor()];

	/// <inheritdoc/>
	int IExtraCellListTrait.ExtraCellSize => ExtraCells.Count;

	private string ChuteString => Options.Converter.ChuteConverter([Chutes[ChuteIndex]]);

	private string ChuteCellsString => Options.Converter.CellConverter(Chutes[ChuteIndex].Cells);
}
