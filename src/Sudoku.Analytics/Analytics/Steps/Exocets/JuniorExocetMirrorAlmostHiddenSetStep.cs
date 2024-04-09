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
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	scoped ref readonly CellMap baseCells,
	scoped ref readonly CellMap targetCells,
	scoped ref readonly CellMap crosslineCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap extraCells,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) :
	ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells),
	IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <summary>
	/// Indicates the subset size.
	/// </summary>
	public int SubsetSize => PopCount((uint)ExtraDigitsMask);

	/// <inheritdoc/>
	public override Technique Code => Technique.JuniorExocetMirrorAlmostHiddenSet;

	/// <inheritdoc/>
	public override FactorCollection Factors => [new ExocetAlmostHiddenSetSizeFactor(Options)];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>.IsHidden => true;

	/// <inheritdoc/>
	Mask IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>.SubsetDigitsMask => ExtraDigitsMask;

	/// <inheritdoc/>
	CellMap IPatternType3StepTrait<JuniorExocetMirrorAlmostHiddenSetStep>.SubsetCells => ExtraCells;
}
