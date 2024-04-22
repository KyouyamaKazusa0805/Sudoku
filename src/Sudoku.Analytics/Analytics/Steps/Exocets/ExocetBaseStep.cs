
namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet (Base)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="endoTargetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="conjugatePairs">Indicates the conjugate pairs used in target.</param>
public sealed partial class ExocetBaseStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap endoTargetCells,
	ref readonly CellMap crosslineCells,
	[PrimaryConstructorParameter] Conjugate[] conjugatePairs
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, in endoTargetCells, in crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + (Code == Technique.SeniorExocet ? 2 : 0);

	/// <inheritdoc/>
	public override Technique Code
		=> (Delta, ConjugatePairs) switch
		{
			(< 0, _) => Technique.SeniorExocet,
			(_, []) => Technique.JuniorExocet,
			_ => Technique.JuniorExocetConjugatePair
		};

	/// <inheritdoc/>
	public override FactorCollection Factors => [new ExocetConjugatePairsCountFactor()];
}
