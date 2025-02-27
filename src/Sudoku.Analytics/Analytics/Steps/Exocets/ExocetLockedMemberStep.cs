namespace Sudoku.Analytics.Steps.Exocets;

/// <summary>
/// Provides with a step that is an <b>Exocet (Locked Member)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="lockedMemberDigitsMask">Indicates the mask that holds a list of locked member digits.</param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
public sealed partial class ExocetLockedMemberStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	[Property] Mask lockedMemberDigitsMask,
	in CellMap baseCells,
	in CellMap targetCells,
	in CellMap endoTargetCells,
	in CellMap crosslineCells
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, in endoTargetCells, in crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override Technique Code => Delta < 0 ? Technique.SeniorExocetLockedMember : Technique.JuniorExocetLockedMember;
}
