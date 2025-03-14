namespace Sudoku.Analytics.Steps.Exocets;

/// <summary>
/// Provides with a step that is an <b>Exocet (Mirror Conjugate Pair)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="conjugatePairs">Indicates the conjugate pairs used.</param>
public sealed partial class ExocetMirrorConjugatePairStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	in CellMap baseCells,
	in CellMap targetCells,
	in CellMap endoTargetCells,
	in CellMap crosslineCells,
	[Property] Conjugate[] conjugatePairs
) : ExocetStep(conclusions, views, options, digitsMask, baseCells, targetCells, endoTargetCells, crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Technique Code => Delta < 0 ? Technique.SeniorExocetMirrorConjugatePair : Technique.JuniorExocetMirrorConjugatePair;
}
