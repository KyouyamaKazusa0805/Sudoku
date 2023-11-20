using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="startCell">Indicates the start cell.</param>
/// <param name="endCell">Indicates the end cell.</param>
/// <param name="conjugatePair">Indicates the conjugate pair connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.</param>
public sealed partial class WWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Cell startCell,
	[Data] Cell endCell,
	[Data] scoped ref readonly Conjugate conjugatePair
) : IrregularWingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.WWing;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [StartCellStr, EndCellStr, ConjStr]), new(ChineseLanguage, [StartCellStr, EndCellStr, ConjStr])];

	private string StartCellStr => Options.Converter.CellConverter([StartCell]);

	private string EndCellStr => Options.Converter.CellConverter([EndCell]);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
