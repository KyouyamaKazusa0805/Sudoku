namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet (Incompatible Pair)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="incompatibleCandidates">Indicates the incompatible candidates.</param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
public sealed partial class JuniorExocetIncompatiblePairStep(
	ReadOnlyMemory<Conclusion> conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	[Property] ref readonly CandidateMap incompatibleCandidates,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap crosslineCells
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 3;

	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocetIncompatiblePair;
}
