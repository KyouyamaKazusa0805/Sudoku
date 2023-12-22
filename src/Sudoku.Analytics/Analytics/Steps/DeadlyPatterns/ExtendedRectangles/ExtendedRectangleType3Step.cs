using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the extra cells used that can form the subset.</param>
/// <param name="subsetDigitsMask">Indicates the subset digits used.</param>
/// <param name="house">Indicates the house that subset formed.</param>
public sealed partial class ExtendedRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[Data] scoped ref readonly CellMap subsetCells,
	[Data] Mask subsetDigitsMask,
	[Data] House house
) : ExtendedRectangleStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [.. base.ExtraDifficultyFactors, new(ExtraDifficultyFactorNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr, HouseStr]),
			new(ChineseLanguage, [DigitsStr, CellsStr, HouseStr, ExtraCellsStr, ExtraDigitsStr])
		];

	private string ExtraDigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string ExtraCellsStr => Options.Converter.CellConverter(SubsetCells);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
