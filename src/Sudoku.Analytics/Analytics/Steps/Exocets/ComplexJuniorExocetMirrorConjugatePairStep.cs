namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Junior Exocet (Mirror Conjugate Pair)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="crosslineHousesMask">Indicates the mask holding a list of houses spanned for cross-line cells.</param>
/// <param name="extraHousesMask">Indicates the mask holding a list of extra houses.</param>
/// <param name="conjugatePairs">Indicates the conjugate pairs used.</param>
public sealed partial class ComplexJuniorExocetMirrorConjugatePairStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	scoped ref readonly CellMap baseCells,
	scoped ref readonly CellMap targetCells,
	scoped ref readonly CellMap crosslineCells,
	[PrimaryCosntructorParameter] HouseMask crosslineHousesMask,
	[PrimaryCosntructorParameter] HouseMask extraHousesMask,
	[PrimaryCosntructorParameter] Conjugate[] conjugatePairs
) :
	ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells),
	IComplexSeniorExocetStepBaseOverrides
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> base.BaseDifficulty + this.GetShapeKind() switch
		{
			ExocetShapeKind.Franken => .4M,
			ExocetShapeKind.Mutant => .6M
		};

	/// <inheritdoc/>
	public override Technique Code
		=> this.GetShapeKind() switch
		{
			ExocetShapeKind.Franken => Technique.FrankenJuniorExocetMirrorConjugatePair,
			ExocetShapeKind.Mutant => Technique.MutantJuniorExocetMirrorConjugatePair
		};

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.Mirror, .1M)];
}
