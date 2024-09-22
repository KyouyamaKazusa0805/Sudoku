namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="trueCandidates">Indicates the true candidates used.</param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits.</param>
/// <param name="cells">Indicates the subset cells used.</param>
/// <param name="isNaked">Indicates whether the subset is naked.</param>
public sealed partial class BivalueUniversalGraveType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CandidateMap trueCandidates,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] bool isNaked
) : BivalueUniversalGraveStep(conclusions, views, options), IPatternType3StepTrait<BivalueUniversalGraveType3Step>
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType3;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(TrueCandidates.Digits | SubsetDigitsMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [TrueCandidatesStr, SubsetTypeStr(SR.EnglishLanguage), SizeStr, ExtraDigitsStr, CellsStr]),
			new(SR.ChineseLanguage, [TrueCandidatesStr, SubsetTypeStr(SR.ChineseLanguage), SizeStr, CellsStr, ExtraDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_BivalueUniversalGraveSubsetSizeFactor",
				[nameof(IPatternType3StepTrait<BivalueUniversalGraveType3Step>.SubsetSize)],
				GetType(),
				static args => (int)args![0]!
			),
			Factor.Create(
				"Factor_BivalueUniversalGraveSubsetIsHiddenFactor",
				[nameof(IPatternType3StepTrait<BivalueUniversalGraveType3Step>.IsHidden)],
				GetType(),
				static args => (bool)args![0]! ? 1 : 0
			)
		];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<BivalueUniversalGraveType3Step>.IsHidden => !IsNaked;

	/// <inheritdoc/>
	int IPatternType3StepTrait<BivalueUniversalGraveType3Step>.SubsetSize => Size;

	/// <inheritdoc/>
	CellMap IPatternType3StepTrait<BivalueUniversalGraveType3Step>.SubsetCells => Cells;

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => Mask.PopCount(SubsetDigitsMask);

	private string TrueCandidatesStr => Options.Converter.CandidateConverter(TrueCandidates);

	private string SizeStr => TechniqueNaming.GetSubsetName(Size);

	private string ExtraDigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string CellsStr => Options.Converter.CellConverter(Cells);


	private string SubsetTypeStr(string cultureName)
	{
		var culture = new CultureInfo(cultureName);
		return SR.Get(IsNaked ? "NakedKeyword" : "HiddenKeyword", culture);
	}
}
