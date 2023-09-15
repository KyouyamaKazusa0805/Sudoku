using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell.</param>
/// <param name="endCell">Indicates the end cell.</param>
/// <param name="conjugatePair">Indicates the conjugate pair connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.</param>
public sealed partial class WWingStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] Cell startCell,
	[DataMember] Cell endCell,
	[DataMember] scoped ref readonly Conjugate conjugatePair
) : IrregularWingStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.WWing;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [StartCellStr, EndCellStr, ConjStr]), new(ChineseLanguage, [StartCellStr, EndCellStr, ConjStr])];

	private string StartCellStr => CellNotation.ToString(StartCell);

	private string EndCellStr => CellNotation.ToString(EndCell);

	private string ConjStr => ConjugatePair.ToString();
}
