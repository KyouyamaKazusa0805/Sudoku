namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Hidden Subset</b>.
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
/// <term><see langword="true"/></term>
/// <description>The subset is a locked hidden subset.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The subset is a normal hidden subset without any extra locked candidates.</description>
/// </item>
/// </list>
/// </param>
public sealed partial class HiddenSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	House house,
	ref readonly CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] bool isLocked
) : SubsetStep(conclusions, views, options, house, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 4;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedHiddenPair,
			(_, 2) => Technique.HiddenPair,
			(true, 3) => Technique.LockedHiddenTriple,
			(_, 3) => Technique.HiddenTriple,
			(_, 4) => Technique.HiddenQuadruple
		};

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [new(EnglishLanguage, [DigitStr, HouseStr]), new(ChineseLanguage, [DigitStr, HouseStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new HiddenSubsetSizeFactor(), new HiddenSubsetIsLockedFactor()];

	private string DigitStr => Options.Converter.DigitConverter(DigitsMask);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
