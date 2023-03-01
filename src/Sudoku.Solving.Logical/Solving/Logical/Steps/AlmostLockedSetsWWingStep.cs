namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets W-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Als1">Indicates the first ALS used in this pattern.</param>
/// <param name="Als2">Indicates the second ALS used in this pattern.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
/// <param name="WDigitsMask">Indicates the mask that holds the W digit.</param>
/// <param name="X">Indicates the X digit.</param>
internal sealed record AlmostLockedSetsWWingStep(
	ConclusionList Conclusions,
	ViewList Views,
	AlmostLockedSet Als1,
	AlmostLockedSet Als2,
	Conjugate ConjugatePair,
	short WDigitsMask,
	int X
) : AlmostLockedSetsStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.2M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedSetsChainingLike;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.AlmostLockedSetsWWing;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Als1Str, Als2Str, ConjStr, WStr, XStr } },
			{ "zh", new[] { Als1Str, Als2Str, ConjStr, WStr, XStr } }
		};

	private string Als1Str => Als1.ToString();

	private string Als2Str => Als2.ToString();

	private string ConjStr => ConjugatePair.ToString();

	private string WStr => DigitMaskFormatter.Format(WDigitsMask, FormattingMode.Normal);

	private string XStr => (X + 1).ToString();
}
