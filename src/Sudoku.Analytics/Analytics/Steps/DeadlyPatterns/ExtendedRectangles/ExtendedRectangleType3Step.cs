namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the extra cells used that can form the subset.</param>
/// <param name="subsetDigitsMask">Indicates the subset digits used.</param>
/// <param name="house">Indicates the house that subset formed.</param>
/// <param name="isCannibalism">Indicates whether the pattern is cannibalism.</param>
public sealed partial class ExtendedRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	ref readonly CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] bool isCannibalism
) : ExtendedRectangleStep(conclusions, views, options, in cells, digitsMask), IPatternType3StepTrait<ExtendedRectangleType3Step>
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override Technique Code => IsCannibalism ? Technique.ExtendedRectangleType3Cannibalism : Technique.ExtendedRectangleType3;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr, HouseStr]),
			new(ChineseLanguage, [DigitsStr, CellsStr, HouseStr, ExtraCellsStr, ExtraDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [.. base.Factors, new ExtendedRectangleSubsetSizeFactor(), new ExtendedRectangleCannibalismFactor()];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<ExtendedRectangleType3Step>.IsHidden => false;

	/// <inheritdoc/>
	int IPatternType3StepTrait<ExtendedRectangleType3Step>.SubsetSize => PopCount((uint)SubsetDigitsMask);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string ExtraCellsStr => Options.Converter.CellConverter(SubsetCells);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
