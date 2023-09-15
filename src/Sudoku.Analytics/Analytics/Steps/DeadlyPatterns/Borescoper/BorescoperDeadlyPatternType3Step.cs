using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the cells that the subset used.</param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits used.</param>
public sealed partial class BorescoperDeadlyPatternType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[DataMember] scoped ref readonly CellMap subsetCells,
	[DataMember] Mask subsetDigitsMask
) : BorescoperDeadlyPatternStep(conclusions, views, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Size, SubsetCells.Count * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr]),
			new(ChineseLanguage, [DigitsStr, CellsStr, ExtraCellsStr, ExtraDigitsStr])
		];

	private string ExtraDigitsStr => DigitNotation.ToString(SubsetDigitsMask);

	private string ExtraCellsStr => SubsetCells.ToString();
}
