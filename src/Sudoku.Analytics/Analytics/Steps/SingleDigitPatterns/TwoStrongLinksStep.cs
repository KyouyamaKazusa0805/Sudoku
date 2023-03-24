namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
public sealed class TwoStrongLinksStep(Conclusion[] conclusions, View[]? views, int digit, int baseHouse, int targetHouse) :
	SingleDigitPatternStep(conclusions, views, digit)
{
	/// <summary>
	/// Indicates the base house used.
	/// </summary>
	public int BaseHouse { get; } = baseHouse;

	/// <summary>
	/// Indicates the target house used.
	/// </summary>
	public int TargetHouse { get; } = targetHouse;

	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> Code switch { Technique.TurbotFish => 4.2M, Technique.Skyscraper => 4.0M, Technique.TwoStringKite => 4.1M };

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique Code
		=> (baseHouse / 9, targetHouse / 9) switch
		{
			(0, _) or (_, 0) => Technique.TurbotFish,
			(1, 1) or (2, 2) => Technique.Skyscraper,
			(1, 2) or (2, 1) => Technique.TwoStringKite
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, BaseHouseStr, TargetHouseStr } },
			{ "zh", new[] { DigitStr, BaseHouseStr, TargetHouseStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string BaseHouseStr => HouseFormatter.Format(1 << baseHouse);

	private string TargetHouseStr => HouseFormatter.Format(1 << targetHouse);
}
