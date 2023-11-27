using System.Numerics;
using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
/// <param name="loopPath"><inheritdoc/></param>
public sealed partial class UniqueLoopType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap loop,
	[Data] scoped ref readonly Conjugate conjugatePair,
	Cell[] loopPath
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop, loopPath)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, ConjStr]), new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, ConjStr])];

	/// <inheritdoc/>
	public override LocatingDifficultyFactor[] LocatingDifficultyFactors
	{
		get
		{
			var (houseTypeScore, housePositionScore) = GetLoopPathScore();
			return [
				new(LocatingDifficultyFactorNames.HouseType, 27 * houseTypeScore),
				new(LocatingDifficultyFactorNames.HousePosition, 9 * housePositionScore),
				new(
					LocatingDifficultyFactorNames.ConjugatePair,
					27 * ConjugatePair.Houses.SetAt(0).ToHouseType() switch
					{
						HouseType.Block => 1,
						HouseType.Row => 3,
						HouseType.Column => 6
					} + 9 * HotSpot.GetHotSpot(ConjugatePair.Houses.SetAt(0))
				),
				new(LocatingDifficultyFactorNames.Size, Loop.Count)
			];
		}
	}

	/// <inheritdoc/>
	public override Formula LocatingDifficultyFormula
		=> new(a => (decimal)Math.Round(Math.Log((double)a[3], 4) * (double)(a[0] + a[1] + a[2]), 2));

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
