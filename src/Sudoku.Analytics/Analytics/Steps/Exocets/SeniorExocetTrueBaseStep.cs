namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Senior Exocet (True Base)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="trueBaseDigit">Indicates the target true base digit that is used for endo-target cell, as value representation.</param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
public sealed partial class SeniorExocetTrueBaseStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	[Property] Digit trueBaseDigit,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap endoTargetCells,
	ref readonly CellMap crosslineCells
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, in endoTargetCells, in crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override Technique Code => Technique.SeniorExocetTrueBase;
}
