using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Subset</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="house"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="isLocked">
/// Indicates which locked type this subset is. The cases are as belows:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>The subset is a locked hidden subset.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The subset is a normal hidden subset without any extra locked candidates.</description>
/// </item>
/// </list>
/// </param>
/// <param name="distance">The distance for the two cells farest away from each other.</param>
/// <param name="interferersCount">The number of interferer candidates that makes the pattern more difficult to be found.</param>
public sealed partial class HiddenSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	House house,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[Data] bool isLocked,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] decimal distance,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int interferersCount
) : SubsetStep(conclusions, views, options, house, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .4M;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedHiddenPair,
			(_, 2) => Technique.HiddenPair,
			(true, 3) => Technique.LockedHiddenTriple,
			(_, 3) => Technique.HiddenTriple,
			(_, 4) => Technique.HiddenQuadruple
		};

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M }),
			new(ExtraDifficultyFactorNames.Locked, IsLocked switch { true => Size switch { 2 => -1.2M, 3 => -1.3M }, _ => 0 })
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr]), new(ChineseLanguage, [DigitStr, HouseStr])];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> Options.IsDirectMode switch
		{
			false => [
				new(
					LocatingDifficultyFactorNames.HouseType,
					House.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 } * 27
				),
				new(LocatingDifficultyFactorNames.HousePosition, HotSpot.GetHotSpot(House) * 9),
				new(LocatingDifficultyFactorNames.Size, Cells.Count),
				new(LocatingDifficultyFactorNames.Distance, Math.Round(_distance, 2)),
				new(LocatingDifficultyFactorNames.Interferer, _interferersCount * 3)
			],
			_ => [
				new(
					LocatingDifficultyFactorNames.HouseType,
					House.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 } * 27
				),
				new(LocatingDifficultyFactorNames.HousePosition, HotSpot.GetHotSpot(House) * 9),
				new(LocatingDifficultyFactorNames.Size, Cells.Count),
				new(LocatingDifficultyFactorNames.Distance, Math.Round(_distance, 2)),
			]
		};

	/// <inheritdoc/>
	public override Formula LocatingDifficultyFormula
		=> Options.IsDirectMode switch
		{
			false => new(a => (a[0] + a[1]) * a[2] + a[3] * 2 + a[4]),
			_ => new(a => (a[0] + a[1]) * a[2] + a[3] * 6)
		};

	private string DigitStr => Options.Converter.DigitConverter(DigitsMask);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
