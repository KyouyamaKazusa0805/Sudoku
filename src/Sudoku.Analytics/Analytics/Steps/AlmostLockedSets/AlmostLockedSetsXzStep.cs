namespace Sudoku.Analytics.Steps.AlmostLockedSets;

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
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property(NamingRule = "FirstAls")] AlmostLockedSetPattern als1,
	[Property(NamingRule = "SecondAls")] AlmostLockedSetPattern als2,
	[Property] Mask xDigitsMask,
	[Property] Mask zDigitsMask,
	[Property] bool isDoublyLinked
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => IsDoublyLinked is true ? 57 : 55;

	/// <inheritdoc/>
	public override Technique Code
		=> IsDoublyLinked ? Technique.DoublyLinkedAlmostLockedSetsXzRule : Technique.SinglyLinkedAlmostLockedSetsXzRule;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(FirstAls.DigitsMask | SecondAls.DigitsMask);

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [Als1Str, Als2Str, XStr, ZResultStr(SR.EnglishLanguage)]),
			new(SR.ChineseLanguage, [Als1Str, Als2Str, XStr, ZResultStr(SR.ChineseLanguage)])
		];

	private string Als1Str => FirstAls.ToString(Options.Converter);

	private string Als2Str => SecondAls.ToString(Options.Converter);

	private string XStr => Options.Converter.DigitConverter(XDigitsMask);


	private string ZResultStr(string cultureName)
	{
		var culture = new CultureInfo(cultureName);
		return ZDigitsMask == 0 ? string.Empty : $"{SR.Get("Comma", culture)}Z = {Options.Converter.DigitConverter(ZDigitsMask)}";
	}
}
