using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.DataModel;
using Sudoku.Facts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits used.</param>
/// <param name="subsetCells">Indicates the subset cells used.</param>
/// <param name="isNaked">Indicates whether the subset is naked one.</param>
public sealed partial class QiuDeadlyPatternType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	[DataMember] scoped in CellMap subsetCells,
	[DataMember] Mask subsetDigitsMask,
	[DataMember] bool isNaked
) : QiuDeadlyPatternStep(conclusions, views, pattern)
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
