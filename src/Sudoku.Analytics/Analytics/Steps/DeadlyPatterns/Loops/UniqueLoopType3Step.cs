using System.Numerics;
using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Facts;
using Sudoku.Rendering;
using Sudoku.Runtime.MaskServices;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the cells that are subset cells.</param>
/// <param name="subsetDigitsMask">Indicates the mask that contains the subset digits used in this instance.</param>
/// <param name="loopPath"><inheritdoc/></param>
public sealed partial class UniqueLoopType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap loop,
	[Data] scoped ref readonly CellMap subsetCells,
	[Data] Mask subsetDigitsMask,
	Cell[] loopPath
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop, loopPath)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [.. base.ExtraDifficultyFactors, new(ExtraDifficultyFactorNames.Size, SubsetCells.Count * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr])
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
				new(LocatingDifficultyFactorNames.ExtraDigit, 9 * GetHouseScore(SubsetCells.Houses & ~HouseMaskOperations.AllBlocksMask)),
				new(LocatingDifficultyFactorNames.Size, Loop.Count),
			];
		}
	}

	/// <inheritdoc/>
	public override Formula LocatingDifficultyFormula
		=> new(a => (decimal)Math.Round(Math.Log((double)a[3], 4) * (double)(a[0] + a[1] + a[2]), 2));

	private string SubsetCellsStr => Options.Converter.CellConverter(SubsetCells);

	private string DigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetName => TechniqueFact.GetSubsetName(SubsetCells.Count);


	/// <summary>
	/// Try to get the score for houses.
	/// </summary>
	private int GetHouseScore(HouseMask houses)
		=> houses.GetAllSets().Sum(static (scoped ref readonly House house) => HotSpot.GetHotSpot(house));
}
