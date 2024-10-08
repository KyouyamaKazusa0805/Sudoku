namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Junior Exocet (Mirror Almost Hidden Set)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
/// <param name="extraCells">Indicates the cells that provides with the AHS rule.</param>
/// <param name="extraDigitsMask">Indicates the mask that holds the digits used by the AHS.</param>
public sealed partial class JuniorExocetMirrorAlmostHiddenSetStep(
	ReadOnlyMemory<Conclusion> conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap crosslineCells,
	[Property] ref readonly CellMap extraCells,
	[Property] Mask extraDigitsMask
) :
	ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells),
	IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <summary>
	/// Indicates the subset size.
	/// </summary>
	public int SubsetSize => Mask.PopCount(ExtraDigitsMask);

	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocetMirrorAlmostHiddenSet;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | ExtraDigitsMask);

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_ExocetAlmostHiddenSetSizeFactor",
				[nameof(SubsetSize)],
				GetType(),
				static args => OeisSequences.A002024((int)args![0]!)
			)
		];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>.IsHidden => true;

	/// <inheritdoc/>
	Mask IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>.SubsetDigitsMask => ExtraDigitsMask;

	/// <inheritdoc/>
	CellMap IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>.SubsetCells => ExtraCells;
}
