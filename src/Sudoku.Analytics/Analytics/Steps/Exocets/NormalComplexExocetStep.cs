namespace Sudoku.Analytics.Steps.Exocets;

/// <summary>
/// Provides with a step that is a <b>Complex Junior Exocet</b> or <b>Complex Senior Exocet</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="crosslineHousesMask">Indicates the mask holding a list of houses spanned for cross-line cells.</param>
/// <param name="extraHousesMask">Indicates the mask holding a list of extra houses.</param>
public sealed partial class NormalComplexExocetStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	in CellMap baseCells,
	in CellMap targetCells,
	in CellMap endoTargetCells,
	in CellMap crosslineCells,
	[Property] HouseMask crosslineHousesMask,
	[Property] HouseMask extraHousesMask
) :
	ExocetStep(conclusions, views, options, digitsMask, baseCells, targetCells, endoTargetCells, crosslineCells),
	IComplexSeniorExocet
{
	/// <inheritdoc/>
	public override int BaseDifficulty
		=> base.BaseDifficulty + this.GetShapeKind() switch { ExocetShapeKind.Franken => 4, ExocetShapeKind.Mutant => 6 };

	/// <inheritdoc/>
	public override Technique Code
		=> (EndoTargetCells, this.GetShapeKind()) switch
		{
			([], ExocetShapeKind.Franken) => Technique.FrankenJuniorExocet,
			(_, ExocetShapeKind.Franken) => Technique.FrankenSeniorExocet,
			([], ExocetShapeKind.Mutant) => Technique.MutantJuniorExocet,
			(_, ExocetShapeKind.Mutant) => Technique.MutantSeniorExocet
		};
}
