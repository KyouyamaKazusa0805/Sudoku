using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Single</b> or <b>Last Digit</b> (for special cases) technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="house">Indicates the house where the current Hidden Single technique forms.</param>
/// <param name="enableAndIsLastDigit">
/// Indicates whether currently options enable "Last Digit" technique, and the current instance is a real Last Digit.
/// </param>
/// <param name="eliminatedCellsCount">The total eliminated cells.</param>
/// <param name="eliminatedHouses">The total eliminated houses.</param>
public sealed partial class HiddenSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	[Data] House house,
	[Data] bool enableAndIsLastDigit,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int[] eliminatedCellsCount,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] House[] eliminatedHouses
) : SingleStep(conclusions, views, options, cell, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => this switch { { EnableAndIsLastDigit: true } => 1.1M, { House: < 9 } => 1.2M, _ => 1.5M };

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty
		=> Code switch { Technique.LastDigit => 200, Technique.HiddenSingleBlock or Technique.CrosshatchingBlock => 250, _ => 300 };

	/// <inheritdoc/>
	public override TechniqueFormat Format => $"{(EnableAndIsLastDigit ? "LastDigit" : "HiddenSingle")}";

	/// <inheritdoc/>
	public override Technique Code
		=> (Options.IsDirectMode, EnableAndIsLastDigit) switch
		{
			(_, true) => Technique.LastDigit,
			(true, false) => (Technique)((int)Technique.CrosshatchingBlock + (int)House.ToHouseType()),
			_ => (Technique)((int)Technique.HiddenSingleBlock + (int)House.ToHouseType())
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr]),
			new(ChineseLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr])
		];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> Code switch
		{
			Technique.LastDigit => [
				new(
					LocatingDifficultyFactorNames.HouseType,
					9 * House.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 }
				),
				new(LocatingDifficultyFactorNames.HousePosition, 3 * HotSpot.GetHotSpot(House))
			],
			_ => [
				new(
					LocatingDifficultyFactorNames.HouseType,
					9 * House.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 }
				),
				new(LocatingDifficultyFactorNames.HousePosition, 3 * HotSpot.GetHotSpot(House)),
				new(
					LocatingDifficultyFactorNames.HiddenSingleExcluder,
					_eliminatedCellsCount.Zip(_eliminatedHouses).Sum(ExcluderValueSelector)
				)
			]
		};

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << House);


	private static int ExcluderValueSelector((int Left, House Right) p)
		=> p.Right.ToHouseType() switch { HouseType.Block => 3, HouseType.Row => 1, HouseType.Column => 2 } + p.Left * 3;
}
