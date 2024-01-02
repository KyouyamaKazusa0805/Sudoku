namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XZ</b> or <b>Extended Subset Principle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="als1">Indicates the first ALS used.</param>
/// <param name="als2">Indicates the second ALS used.</param>
/// <param name="xDigitsMask">Indicates the mask of X digits used.</param>
/// <param name="zDigitsMask">Indicates the mask of Z digits used.</param>
/// <param name="isDoublyLinked">
/// <para>Indicates whether ALS-XZ is doubly-linked.</para>
/// <para>
/// All possible values are <see langword="true"/>, <see langword="false"/> and <see langword="null"/>.
/// If the value is <see langword="true"/> or <see langword="false"/>, the ALS-XZ is a Doubly- or Singly- Linked ALS-XZ;
/// otherwise, an Extended Subset Principle technique.
/// </para>
/// </param>
public sealed partial class AlmostLockedSetsXzStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data(GeneratedMemberName = "FirstAls")] AlmostLockedSet als1,
	[Data(GeneratedMemberName = "SecondAls")] AlmostLockedSet als2,
	[Data] Mask xDigitsMask,
	[Data] Mask zDigitsMask,
	[Data] bool isDoublyLinked
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

	/// <inheritdoc/>
	public override TechniqueFormat Format => $"{"AlmostLockedSetsXzRule"}";

	/// <inheritdoc/>
	public override Technique Code
		=> IsDoublyLinked ? Technique.DoublyLinkedAlmostLockedSetsXzRule : Technique.SinglyLinkedAlmostLockedSetsXzRule;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Als1Str, Als2Str, XStr, ZResultStr]),
			new(ChineseLanguage, [Als1Str, Als2Str, XStr, ZResultStr])
		];

	private string Als1Str => FirstAls.ToString(Options.Converter);

	private string Als2Str => SecondAls.ToString(Options.Converter);

	private string XStr => Options.Converter.DigitConverter(XDigitsMask);

	private string ZResultStr
		=> ZDigitsMask == 0
			? string.Empty
			: $"{GetString("Comma", ResultCurrentCulture)!}Z = {Options.Converter.DigitConverter(ZDigitsMask)}";
}
