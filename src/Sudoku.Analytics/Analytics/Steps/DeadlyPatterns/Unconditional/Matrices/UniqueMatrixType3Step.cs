namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetDigitsMask">Indicates the mask that describes the extra digits used in the subset.</param>
/// <param name="subsetCells">Indicates the cells that the subset used.</param>
public sealed partial class UniqueMatrixType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	ref readonly CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask
) : UniqueMatrixStep(conclusions, views, options, in cells, digitsMask), IPatternType3StepTrait<UniqueMatrixType3Step>
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | SubsetDigitsMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitStr, ExtraCellsStr, SubsetName]),
			new(SR.ChineseLanguage, [ExtraDigitStr, ExtraCellsStr, SubsetName, DigitsStr, CellsStr])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_UniqueMatrixSubsetSizeFactor",
				[nameof(IPatternType3StepTrait<UniqueMatrixType3Step>.SubsetSize)],
				GetType(),
				static args => (int)args![0]!
			)
		];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<UniqueMatrixType3Step>.IsHidden => false;

	/// <inheritdoc/>
	int IPatternType3StepTrait<UniqueMatrixType3Step>.SubsetSize => Mask.PopCount(SubsetDigitsMask);

	private string ExtraCellsStr => Options.Converter.CellConverter(SubsetCells);

	private string ExtraDigitStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetName => TechniqueNaming.GetSubsetName(Mask.PopCount(SubsetDigitsMask));
}
