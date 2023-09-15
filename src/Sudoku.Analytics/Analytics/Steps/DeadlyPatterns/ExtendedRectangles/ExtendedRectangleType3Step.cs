using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the extra cells used that can form the subset.</param>
/// <param name="subsetDigitsMask">Indicates the subset digits used.</param>
/// <param name="house">Indicates the house that subset formed.</param>
public sealed partial class ExtendedRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[DataMember] scoped ref readonly CellMap subsetCells,
	[DataMember] Mask subsetDigitsMask,
	[DataMember] House house
) : ExtendedRectangleStep(conclusions, views, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [.. base.ExtraDifficultyCases, new(ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr, HouseStr]),
			new(ChineseLanguage, [DigitsStr, CellsStr, HouseStr, ExtraCellsStr, ExtraDigitsStr])
		];

	private string ExtraDigitsStr => DigitNotation.ToString(SubsetDigitsMask);

	private string ExtraCellsStr => SubsetCells.ToString();

	private string HouseStr => HouseNotation.ToString(House);
}
