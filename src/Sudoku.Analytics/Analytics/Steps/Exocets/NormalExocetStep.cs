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
public sealed partial class NormalExocetStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap endoTargetCells,
	ref readonly CellMap crosslineCells,
	[Property] Conjugate[] conjugatePairs
) :
	ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, in endoTargetCells, in crosslineCells),
	IConjugatePairTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + (Code == Technique.SeniorExocet ? 2 : 0);

	/// <inheritdoc/>
	public override Technique Code
		=> (Delta, ConjugatePairs) switch
		{
			( < 0, _) => Technique.SeniorExocet,
			(_, []) => Technique.JuniorExocet,
			_ => Technique.JuniorExocetConjugatePair
		};

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | MaskOperations.Create(from c in ConjugatePairs select c.Digit));

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_ExocetConjugatePairsCountFactor",
				[nameof(IConjugatePairTrait.ConjugatePairsCount)],
				GetType(),
				static args => OeisSequences.A004526((int)args![0]!)
			)
		];

	/// <inheritdoc/>
	int IConjugatePairTrait.ConjugatePairsCount => ConjugatePairs.Length;
}
