namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="Block">Indicates the block that the empty rectangle structure formed.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
internal sealed record EmptyRectangleStep(ConclusionList Conclusions, ViewList Views, int Digit, int Block, scoped in Conjugate ConjugatePair) :
	SingleDigitPatternStep(Conclusions, Views, Digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.6M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.EmptyRectangle;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.EmptyRectangle;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, HouseStr, ConjStr } },
			{ "zh", new[] { DigitStr, HouseStr, ConjStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string HouseStr => HouseFormatter.Format(1 << Block);

	private string ConjStr => ConjugatePair.ToString();
}
