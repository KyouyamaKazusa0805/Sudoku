using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Facts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits used.</param>
/// <param name="subsetCells">Indicates the subset cells used.</param>
/// <param name="isNaked">Indicates whether the subset is naked one.</param>
public sealed partial class QiuDeadlyPatternType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[DataMember] scoped ref readonly CellMap subsetCells,
	[DataMember] Mask subsetDigitsMask,
	[DataMember] bool isNaked
) : QiuDeadlyPatternStep(conclusions, views, options, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.Size, PopCount((uint)SubsetDigitsMask) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [PatternStr, DigitsStr, CellsStr, SubsetName]),
			new(ChineseLanguage, [PatternStr, DigitsStr, CellsStr, SubsetName])
		];

	private string DigitsStr => DigitNotation.ToString(SubsetDigitsMask);

	private string CellsStr => SubsetCells.ToString();

	private string SubsetName => TechniqueFact.GetSubsetName(SubsetCells.Count + 1);
}
