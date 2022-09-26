namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets XY-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Als1">Indicates the first ALS used in this pattern.</param>
/// <param name="Als2">Indicates the second ALS used in this pattern.</param>
/// <param name="Bridge">Indicates the ALS that is as a bridge.</param>
/// <param name="XDigitsMask">Indicates the mask that holds the digits for the X value.</param>
/// <param name="YDigitsMask">Indicates the mask that holds the digits for the Y value.</param>
/// <param name="ZDigitsMask">Indicates the mask that holds the digits for the Z value.</param>
internal sealed record AlmostLockedSetsXyWingStep(
	ConclusionList Conclusions,
	ViewList Views,
	AlmostLockedSet Als1,
	AlmostLockedSet Als2,
	AlmostLockedSet Bridge,
	short XDigitsMask,
	short YDigitsMask,
	short ZDigitsMask
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 6.0M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedSetsChainingLike;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.AlmostLockedSetsXyWing;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[ResourceTextFormatter]
	internal string Als1Str() => Als1.ToString();

	[ResourceTextFormatter]
	internal string BridgeStr() => Bridge.ToString();

	[ResourceTextFormatter]
	internal string Als2Str() => Als2.ToString();

	[ResourceTextFormatter]
	internal string XStr() => DigitMaskFormatter.Format(XDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string YStr() => DigitMaskFormatter.Format(YDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string ZStr() => DigitMaskFormatter.Format(ZDigitsMask, FormattingMode.Normal);
}
