namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="BaseHouse">Indicates the base house used.</param>
/// <param name="TargetHouse">Indicates the target house used.</param>
internal sealed record TwoStrongLinksStep(ConclusionList Conclusions, ViewList Views, int Digit, int BaseHouse, int TargetHouse) :
	SingleDigitPatternStep(Conclusions, Views, Digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> TechniqueCode switch { Technique.TurbotFish => 4.2M, Technique.Skyscraper => 4.0M, Technique.TwoStringKite => 4.1M };

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> (BaseHouse / 9, TargetHouse / 9) switch
		{
			(0, _) or (_, 0) => Technique.TurbotFish,
			(1, 1) or (2, 2) => Technique.Skyscraper,
			(1, 2) or (2, 1) => Technique.TwoStringKite
		};

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, BaseHouseStr, TargetHouseStr } },
			{ "zh", new[] { DigitStr, BaseHouseStr, TargetHouseStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string BaseHouseStr => HouseFormatter.Format(1 << BaseHouse);

	private string TargetHouseStr => HouseFormatter.Format(1 << TargetHouse);
}
