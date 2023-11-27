using System.Numerics;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Runtime.MaskServices;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="code"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
/// <param name="extraCells">Indicates the extra cells.</param>
/// <param name="emptyCellsCount">The number of empty cells.</param>
public sealed partial class UniqueRectangleType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	Technique code,
	scoped ref readonly CellMap cells,
	bool isAvoidable,
	[Data] Digit extraDigit,
	int absoluteOffset,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] scoped ref readonly CellMap extraCells,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] int emptyCellsCount
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	code,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override decimal BaseLocatingDifficulty => 54;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, ExtraDigitStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, ExtraDigitStr])
		];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
	{
		get
		{
			var blocks = Cells.BlockMask.GetAllSets();
			return [
				new(LocatingDifficultyFactorNames.HousePosition, (HotSpot.GetHotSpot(blocks[0]) + HotSpot.GetHotSpot(blocks[1])) * 9),
				new(LocatingDifficultyFactorNames.Incompleteness, 60),
				new(LocatingDifficultyFactorNames.ExtraDigit, 9 * GetHouseScore(_extraCells.Houses & ~HouseMaskOperations.AllBlocksMask)),
				new(LocatingDifficultyFactorNames.AvoidableRectangle, _emptyCellsCount * 60)
			];
		}
	}

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));


	/// <summary>
	/// Try to get the score for houses.
	/// </summary>
	private int GetHouseScore(HouseMask houses)
		=> houses.GetAllSets().Sum(static (scoped ref readonly House house) => HotSpot.GetHotSpot(house));
}
