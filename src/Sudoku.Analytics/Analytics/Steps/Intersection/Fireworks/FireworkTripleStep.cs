using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Triple</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
public sealed partial class FireworkTripleStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped in CellMap cells,
	[DataMember] Mask digitsMask
) : FireworkStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkTriple;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CellsStr, DigitsStr]), new(ChineseLanguage, [CellsStr, DigitsStr])];

	private string CellsStr => Cells.ToString();

	private string DigitsStr => DigitNotation.ToString(DigitsMask);
}
