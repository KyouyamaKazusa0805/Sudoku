namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XZ</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Als1">Indicates the first ALS used in this pattern.</param>
/// <param name="Als2">Indicates the second ALS used in this pattern.</param>
/// <param name="XDigitsMask">Indicates the X digit used in this ALS-XZ pattern.</param>
/// <param name="ZDigitsMask">Indicates the Z digit used in this ALS-XZ pattern.</param>
/// <param name="IsDoublyLinked">Indicates whether the ALS-XZ is doubly-linked.</param>
internal sealed record AlmostLockedSetsXzStep(
	ConclusionList Conclusions,
	ViewList Views,
	AlmostLockedSet Als1,
	AlmostLockedSet Als2,
	short XDigitsMask,
	short ZDigitsMask,
	bool? IsDoublyLinked
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

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
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> IsDoublyLinked switch
		{
			true => Technique.DoublyLinkedAlmostLockedSetsXzRule,
			false => Technique.SinglyLinkedAlmostLockedSetsXzRule,
			_ => Technique.ExtendedSubsetPrinciple
		};

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;


	[ResourceTextFormatter]
	internal string CellsStr() => (Als1.Map | Als2.Map).ToString();

	[ResourceTextFormatter]
	internal string EspDigitStr() => (TrailingZeroCount(ZDigitsMask) + 1).ToString();

	[ResourceTextFormatter]
	internal string Als1Str() => Als1.ToString();

	[ResourceTextFormatter]
	internal string Als2Str() => Als2.ToString();

	[ResourceTextFormatter]
	internal string XStr() => DigitMaskFormatter.Format(XDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string ZResultStr() => ZDigitsMask != 0 ? $"{R.EmitPunctuation(Punctuation.Comma)}Z = {DigitMaskFormatter.Format(ZDigitsMask, FormattingMode.Normal)}" : string.Empty;
}
