using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Subset</b> technique.
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
/// <description>The subset is a locked subset.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The subset is a naked subset with at least one extra locked candidate.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term>
/// <description>The subset is a normal naked subset without any extra locked candidates.</description>
/// </item>
/// </list>
/// </param>
/// <param name="isBivalueSubset">Indicates whether the cells form a bi-value subset.</param>
/// <param name="distance">The distance for the two cells farest away from each other.</param>
public sealed partial class NakedSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	House house,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[Data] bool? isLocked,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] bool isBivalueSubset,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] decimal distance
) : SubsetStep(conclusions, views, options, house, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(_, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(_, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			(null, 4) => Technique.NakedQuadruple
		};

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M }),
			new(ExtraDifficultyFactorNames.Locked, IsLocked switch { true => Size switch { 2 => -1.0M, 3 => -1.1M }, false => .1M, _ => 0 })
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, HouseStr]), new(ChineseLanguage, [DigitsStr, HouseStr, SubsetName])];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
		=> Options.IsDirectMode switch
		{
			false => (IsLocked, _isBivalueSubset, House.ToHouseType(), HotSpot.GetHotSpot(House), Cells.Count, _distance) switch
			{
				(true, _, _, var hotspot, var count, var dist) => [
					new(LocatingDifficultyFactorNames.HousePosition, hotspot * 9),
					new(LocatingDifficultyFactorNames.Size, count)
				],
				(_, true, _, var hotspot, var count, var dist) => [
					new(LocatingDifficultyFactorNames.HousePosition, hotspot * 9),
					new(LocatingDifficultyFactorNames.Distance, dist * 3),
					new(LocatingDifficultyFactorNames.Size, count)
				],
				var (_, _, type, hotspot, count, dist) => [
					new(
						LocatingDifficultyFactorNames.HouseType,
						type switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 } * 27
					),
					new(LocatingDifficultyFactorNames.HousePosition, hotspot * 9),
					new(LocatingDifficultyFactorNames.Distance, dist * 3),
					new(LocatingDifficultyFactorNames.Size, count)
				]
			},
			_ => [
				new(
					LocatingDifficultyFactorNames.HouseType,
					House.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 } * 27
				),
				new(LocatingDifficultyFactorNames.HousePosition, HotSpot.GetHotSpot(House) * 9),
				new(LocatingDifficultyFactorNames.Size, Cells.Count),
				new(LocatingDifficultyFactorNames.Distance, Math.Round(_distance, 2))
			]
		};

	/// <inheritdoc/>
	public override Formula LocatingDifficultyFormula
		=> Options.IsDirectMode switch
		{
			false => (IsLocked, _isBivalueSubset) switch
			{
				(true, _) => new(a => a[0] * a[1]),
				(_, true) => new(a => (a[0] + a[1]) * a[2]),
				_ => new(a => (a[0] + a[1] + a[2]) * a[3])
			},
			_ => new(a => (a[0] + a[1]) * a[2] + a[3] * 2)
		};

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);

	private string SubsetName => GetString($"SubsetNamesSize{Size}")!;
}
