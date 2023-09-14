using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Domino Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
public sealed partial class DominoLoopStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped in CellMap cells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.6M;

	/// <inheritdoc/>
	public override Technique Code => Technique.DominoLoop;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CellsCountStr, CellsStr]), new(ChineseLanguage, [CellsCountStr, CellsStr])];

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
