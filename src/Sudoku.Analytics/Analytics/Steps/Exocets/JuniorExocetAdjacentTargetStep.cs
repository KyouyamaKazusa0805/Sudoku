namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet (Adjacent Target)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="singleMirrors">Indicates the single mirror cells. The value should be used one-by-one.</param>
public sealed partial class JuniorExocetAdjacentTargetStep(
	ReadOnlyMemory<Conclusion> conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap endoTargetCells,
	ref readonly CellMap crosslineCells,
	[Property] ref readonly CellMap singleMirrors
) : ExocetStep(
	conclusions,
	views,
	options,
	digitsMask,
	in baseCells,
	in targetCells,
	in endoTargetCells,
	in crosslineCells
)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocetAdjacentTarget;
}
