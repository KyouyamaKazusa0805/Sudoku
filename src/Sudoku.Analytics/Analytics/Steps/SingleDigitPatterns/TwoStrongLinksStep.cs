using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single-Digit Pattern</b> or <b>Grouped Single-Digit Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseHouse">Indicates the base house used.</param>
/// <param name="targetHouse">Indicates the target house used.</param>
/// <param name="isGrouped">Indicates whether the links is grouped.</param>
public sealed partial class TwoStrongLinksStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit,
	[Data] House baseHouse,
	[Data] House targetHouse,
	[Data] bool isGrouped
) : SingleDigitPatternStep(conclusions, views, options, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> Code switch
		{
			Technique.TurbotFish => 4.2M,
			Technique.Skyscraper => 4.0M,
			Technique.TwoStringKite => 4.1M,
			Technique.GroupedTurbotFish => 4.4M,
			Technique.GroupedSkyscraper => 4.2M,
			Technique.GroupedTwoStringKite => 4.3M
		};

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 320;

	/// <inheritdoc/>
	public override Technique Code
		=> (BaseHouse / 9, TargetHouse / 9) switch
		{
			(0, _) or (_, 0) => IsGrouped ? Technique.GroupedTurbotFish : Technique.TurbotFish,
			(1, 1) or (2, 2) => IsGrouped ? Technique.GroupedSkyscraper : Technique.Skyscraper,
			(1, 2) or (2, 1) => IsGrouped ? Technique.GroupedTwoStringKite : Technique.TwoStringKite
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, BaseHouseStr, TargetHouseStr]), new(ChineseLanguage, [DigitStr, BaseHouseStr, TargetHouseStr])];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string BaseHouseStr => Options.Converter.HouseConverter(1 << BaseHouse);

	private string TargetHouseStr => Options.Converter.HouseConverter(1 << TargetHouse);
}
