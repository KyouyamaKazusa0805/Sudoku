using System.Numerics;
using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Runtime.MaskServices;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
/// <param name="loopPath"><inheritdoc/></param>
/// <param name="extraCells">The extra cells used.</param>
public sealed partial class UniqueLoopType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap loop,
	[Data] Digit extraDigit,
	Cell[] loopPath,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] scoped ref readonly CellMap extraCells
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop, loopPath)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraDigitStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ExtraDigitStr])
		];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
	{
		get
		{
			var (houseTypeScore, housePositionScore) = GetLoopPathScore();
			return [
				new(LocatingDifficultyFactorNames.HouseType, 27 * houseTypeScore),
				new(LocatingDifficultyFactorNames.HousePosition, housePositionScore * 9),
				new(LocatingDifficultyFactorNames.ExtraDigit, 9 * GetHouseScore(_extraCells.Houses & ~HouseMaskOperations.AllBlocksMask)),
				new(LocatingDifficultyFactorNames.Size, Loop.Count),
			];
		}
	}

	/// <inheritdoc/>
	public override Formula LocatingDifficultyFormula
		=> new(a => (decimal)Math.Round(Math.Log((double)a[3], 4) * (double)(a[0] + a[1] + a[2]), 2));

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));


	/// <summary>
	/// Try to get the score for houses.
	/// </summary>
	private int GetHouseScore(HouseMask houses)
		=> houses.GetAllSets().Sum(static (scoped ref readonly House house) => HotSpot.GetHotSpot(house));
}
