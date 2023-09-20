using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using static Sudoku.Analytics.Strings.StringsAccessor;

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
	[DataMember(GeneratedMemberName = "FirstAls")] AlmostLockedSet als1,
	[DataMember(GeneratedMemberName = "SecondAls")] AlmostLockedSet als2,
	[DataMember] Mask xDigitsMask,
	[DataMember] Mask zDigitsMask,
	[DataMember] bool? isDoublyLinked
) : AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

	/// <inheritdoc/>
	public override string? Format
		=> GetString(
			(IsDoublyLinked, ZDigitsMask) switch
			{
				(null, 0) => "TechniqueFormat_ExtendedSubsetPrincipleWithoutDuplicate",
				(null, _) => "TechniqueFormat_ExtendedSubsetPrincipleWithDuplicate",
				_ => "TechniqueFormat_AlmostLockedSetsXzRule"
			}
		);

	/// <inheritdoc/>
	public override Technique Code
		=> IsDoublyLinked switch
		{
			true => Technique.DoublyLinkedAlmostLockedSetsXzRule,
			false => Technique.SinglyLinkedAlmostLockedSetsXzRule,
			_ => Technique.ExtendedSubsetPrinciple
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(
				EnglishLanguage,
				IsDoublyLinked is null ? ZDigitsMask == 0 ? [CellsStr] : [EspDigitStr, CellsStr] : [Als1Str, Als2Str, XStr, ZResultStr]
			),
			new(
				ChineseLanguage,
				IsDoublyLinked is null ? ZDigitsMask == 0 ? [CellsStr] : [EspDigitStr, CellsStr] : [Als1Str, Als2Str, XStr, ZResultStr]
			)
		];

	private string CellsStr => Options.CoordinateConverter.CellNotationConverter(FirstAls.Cells | SecondAls.Cells);

	private string EspDigitStr => Options.CoordinateConverter.DigitNotationConverter(ZDigitsMask);

	private string Als1Str => FirstAls.ToString(Options.CoordinateConverter);

	private string Als2Str => SecondAls.ToString(Options.CoordinateConverter);

	private string XStr => Options.CoordinateConverter.DigitNotationConverter(XDigitsMask);

	private string ZResultStr
		=> ZDigitsMask == 0 ? string.Empty : $"{GetString("Comma")!}Z = {Options.CoordinateConverter.DigitNotationConverter(ZDigitsMask)}";
}
