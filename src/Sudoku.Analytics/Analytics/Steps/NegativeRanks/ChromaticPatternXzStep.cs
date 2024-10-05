namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern XZ</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="blocks"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="cells">Indicates the cells that contains extra digit.</param>
/// <param name="extraCell">Indicates the extra cell used.</param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="extraDigitsMask">Indicates the mask of extra digits.</param>
public sealed partial class ChromaticPatternXzStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	House[] blocks,
	ref readonly CellMap pattern,
	[Property] ref readonly CellMap cells,
	[Property] Cell extraCell,
	Mask digitsMask,
	[Property] Mask extraDigitsMask
) : ChromaticPatternStep(conclusions, views, options, blocks, in pattern, digitsMask)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override Technique Code => Technique.ChromaticPatternXzRule;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(DigitsMask | ExtraDigitsMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [DigitsStr, CellsStr, BlocksStr, ExtraCellStr]),
			new(SR.ChineseLanguage, [BlocksStr, CellsStr, DigitsStr, ExtraCellStr])
		];

	private string ExtraCellStr => Options.Converter.CellConverter(in ExtraCell.AsCellMap());
}
