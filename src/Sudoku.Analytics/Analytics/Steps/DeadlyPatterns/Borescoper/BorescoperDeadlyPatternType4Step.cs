using System.SourceGeneration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="conjugateHouse">Indicates the cells used as generalized conjugate.</param>
/// <param name="extraDigitsMask">Indicates the mask of extra digits used.</param>
public sealed partial class BorescoperDeadlyPatternType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped ref readonly CellMap cells,
	Mask digitsMask,
	[DataMember] scoped ref readonly CellMap conjugateHouse,
	[DataMember] Mask extraDigitsMask
) : BorescoperDeadlyPatternStep(conclusions, views, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ConjHouseStr, ExtraCombStr]),
			new(ChineseLanguage, [DigitsStr, CellsStr, ExtraCombStr, ConjHouseStr])
		];

	private string ExtraCombStr => DigitNotation.ToString(ExtraDigitsMask);

	private string ConjHouseStr => ConjugateHouse.ToString();
}
