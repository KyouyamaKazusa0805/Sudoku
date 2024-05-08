namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Naked Subset</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="house"><inheritdoc cref="SubsetStep.House" path="/summary"/></param>
/// <param name="cells"><inheritdoc cref="SubsetStep.Cells" path="/summary"/></param>
/// <param name="digitsMask"><inheritdoc cref="SubsetStep.DigitsMask" path="/summary"/></param>
/// <param name="isLocked">
/// Indicates which locked type this subset is. The cases are as belows:
/// <list type="table">
/// <item>
/// <term><see langword="true" /></term>
/// <description>The subset is a locked subset.</description>
/// </item>
/// <item>
/// <term><see langword="false" /></term>
/// <description>The subset is a naked subset with at least one extra locked candidate.</description>
/// </item>
/// <item>
/// <term><see langword="null" /></term>
/// <description>The subset is a normal naked subset without any extra locked candidates.</description>
/// </item>
/// </list>
/// </param>
public sealed partial class NakedSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	House house,
	ref readonly CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] bool? isLocked
) : SubsetStep(conclusions, views, options, house, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(_, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(_, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			(null, 4) => Technique.NakedQuadruple
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, HouseStr]), new(ChineseLanguage, [DigitsStr, HouseStr, SubsetName])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new NakedSubsetSizeFactor(), new NakedSubsetIsLockedFactor()];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);

	private string SubsetName => ResourceDictionary.Get($"SubsetNamesSize{Size}", ResultCurrentCulture);
}
