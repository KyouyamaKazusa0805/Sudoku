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
	[PrimaryConstructorParameter] HouseMask crosslineHousesMask,
	[PrimaryConstructorParameter] HouseMask extraHousesMask,
	[PrimaryConstructorParameter] Conjugate[] conjugatePairs
) :
	ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells),
	IComplexSeniorExocet
{
	/// <inheritdoc/>
	public override int BaseDifficulty
		=> base.BaseDifficulty + 1 + this.GetShapeKind() switch
		{
			ExocetShapeKind.Franken => 4,
			ExocetShapeKind.Mutant => 6
		};

	/// <inheritdoc/>
	public override Technique Code
		=> this.GetShapeKind() switch
		{
			ExocetShapeKind.Franken => Technique.FrankenJuniorExocetMirrorConjugatePair,
			ExocetShapeKind.Mutant => Technique.MutantJuniorExocetMirrorConjugatePair
		};
}
