namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XY-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="als1">Indicates the first ALS used in this pattern.</param>
/// <param name="als2">Indicates the second ALS used in this pattern.</param>
/// <param name="bridge">Indicates the ALS that is as a bridge.</param>
/// <param name="xDigitsMask">Indicates the mask that holds the digits for the X value.</param>
/// <param name="yDigitsMask">Indicates the mask that holds the digits for the Y value.</param>
/// <param name="zDigitsMask">Indicates the mask that holds the digits for the Z value.</param>
public sealed partial class AlmostLockedSetsXyWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter(GeneratedMemberName = "FirstAls")] AlmostLockedSet als1,
	[PrimaryConstructorParameter(GeneratedMemberName = "SecondAls")] AlmostLockedSet als2,
	[PrimaryConstructorParameter(GeneratedMemberName = "BridgeAls")] AlmostLockedSet bridge,
	[PrimaryConstructorParameter] Mask xDigitsMask,
	[PrimaryConstructorParameter] Mask yDigitsMask,
	[PrimaryConstructorParameter] Mask zDigitsMask
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.AlmostLockedSetsXyWing;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Als1Str, BridgeStr, Als2Str, XStr, YStr, ZStr]),
			new(ChineseLanguage, [Als1Str, BridgeStr, Als2Str, XStr, YStr, ZStr])
		];

	private string Als1Str => FirstAls.ToString(Options.Converter);

	private string BridgeStr => BridgeAls.ToString(Options.Converter);

	private string Als2Str => SecondAls.ToString(Options.Converter);

	private string XStr => Options.Converter.DigitConverter(XDigitsMask);

	private string YStr => Options.Converter.DigitConverter(YDigitsMask);

	private string ZStr => Options.Converter.DigitConverter(ZDigitsMask);
}
