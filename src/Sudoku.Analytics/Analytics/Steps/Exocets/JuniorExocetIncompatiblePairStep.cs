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
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	[PrimaryConstructorParameter] scoped ref readonly CandidateMap incompatibleCandidates,
	scoped ref readonly CellMap baseCells,
	scoped ref readonly CellMap targetCells,
	scoped ref readonly CellMap crosslineCells
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocetIncompatiblePair;
}
