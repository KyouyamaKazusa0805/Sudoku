namespace Sudoku.Analytics.Steps.Invalidity;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="blocks">Indicates the blocks that the current pattern lies in.</param>
/// <param name="pattern">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask of digits.</param>
public abstract partial class ChromaticPatternStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] House[] blocks,
	[Property] ref readonly CellMap pattern,
	[Property] Mask digitsMask
) : NegativeRankStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 65;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	private protected string BlocksStr
		=> Options.Converter.HouseConverter(Blocks.Aggregate(@delegate.BitMerger));

	private protected string CellsStr => Options.Converter.CellConverter(Pattern);

	private protected string DigitsStr => Options.Converter.DigitConverter(DigitsMask);
}
