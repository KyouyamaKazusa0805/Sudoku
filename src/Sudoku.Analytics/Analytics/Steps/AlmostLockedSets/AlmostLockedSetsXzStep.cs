namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XZ</b> or <b>Extended Subset Principle</b> technique.
/// </summary>
public sealed class AlmostLockedSetsXzStep(
	Conclusion[] conclusions,
	View[]? views,
	AlmostLockedSet als1,
	AlmostLockedSet als2,
	short xDigitsMask,
	short zDigitsMask,
	bool? isDoublyLinked
) : AlmostLockedSetsStep(conclusions, views)
{
	/// <summary>
	/// Indicates whether ALS-XZ is doubly-linked.
	/// </summary>
	/// <remarks>
	/// All possible values are <see langword="true"/>, <see langword="false"/> and <see langword="null"/>.
	/// If the value is <see langword="true"/> or <see langword="false"/>, the ALS-XZ is a Doubly- or Singly- Linked ALS-XZ;
	/// otherwise, an Extended Subset Principle technique.
	/// </remarks>
	public bool? IsDoublyLinked { get; } = isDoublyLinked;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

	/// <summary>
	/// Indicates the mask of X digits used.
	/// </summary>
	public short XDigitsMask { get; } = xDigitsMask;

	/// <summary>
	/// Indicates the mask of Z digits used.
	/// </summary>
	public short ZDigitsMask { get; } = zDigitsMask;

	/// <inheritdoc/>
	public override string? Format
		=> R[
			IsDoublyLinked is null
				? ZDigitsMask == 0
					? "TechniqueFormat_ExtendedSubsetPrincipleWithoutDuplicate"
					: "TechniqueFormat_ExtendedSubsetPrincipleWithDuplicate"
				: "TechniqueFormat_AlmostLockedSetsXzRule"
		];

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique Code
		=> IsDoublyLinked switch
		{
			true => Technique.DoublyLinkedAlmostLockedSetsXzRule,
			false => Technique.SinglyLinkedAlmostLockedSetsXzRule,
			null => Technique.ExtendedSubsetPrinciple
		};

	/// <summary>
	/// Indicates the first ALS used.
	/// </summary>
	public AlmostLockedSet FirstAls { get; } = als1;

	/// <summary>
	/// Indicates the second ALS used.
	/// </summary>
	public AlmostLockedSet SecondAls { get; } = als2;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{
				"en",
				IsDoublyLinked is null
					? ZDigitsMask == 0 ? new[] { CellsStr } : new[] { EspDigitStr, CellsStr }
					: new[] { Als1Str, Als2Str, XStr, ZResultStr }
			},
			{
				"zh",
				IsDoublyLinked is null
					? ZDigitsMask == 0 ? new[] { CellsStr } : new[] { EspDigitStr, CellsStr }
					: new[] { Als1Str, Als2Str, XStr, ZResultStr }
			}
		};

	private string CellsStr => (FirstAls.Map | SecondAls.Map).ToString();

	private string EspDigitStr => (TrailingZeroCount(ZDigitsMask) + 1).ToString();

	private string Als1Str => FirstAls.ToString();

	private string Als2Str => SecondAls.ToString();

	private string XStr => DigitMaskFormatter.Format(XDigitsMask, FormattingMode.Normal);

	private string ZResultStr
		=> ZDigitsMask == 0
			? string.Empty
			: $"{R["Comma"]!}Z = {DigitMaskFormatter.Format(ZDigitsMask, FormattingMode.Normal)}";
}
