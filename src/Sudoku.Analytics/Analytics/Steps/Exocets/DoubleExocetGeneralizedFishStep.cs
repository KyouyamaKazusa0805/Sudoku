namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Double Exocet (Fish)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="baseCellsTheOther">Indicates the other side of the base cells.</param>
/// <param name="targetCellsTheOther">Indicates the other side of the target cells.</param>
public sealed partial class DoubleExocetGeneralizedFishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	scoped ref readonly CellMap baseCells,
	scoped ref readonly CellMap targetCells,
	scoped ref readonly CellMap crosslineCells,
	[RecordParameter] scoped ref readonly CellMap baseCellsTheOther,
	[RecordParameter] scoped ref readonly CellMap targetCellsTheOther
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells), IDoubleExocetStepBaseOverrides
{
	/// <inheritdoc/>
	public override Technique Code => Technique.DoubleExocetGeneralizedFish;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.GeneralizedFish, .2M)];
}
