namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseHouse">Indicates the base house used.</param>
/// <param name="targetHouse">Indicates the target house used.</param>
public sealed partial class TwoStrongLinksStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit,
	[PrimaryConstructorParameter] int baseHouse,
	[PrimaryConstructorParameter] int targetHouse
) : SingleDigitPatternStep(conclusions, views, digit)
{
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
