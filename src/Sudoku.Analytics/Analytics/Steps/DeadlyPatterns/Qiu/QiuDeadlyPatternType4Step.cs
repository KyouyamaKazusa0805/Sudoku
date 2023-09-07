using System.SourceGeneration;
using Sudoku.Analytics.Rating;
using Sudoku.DataModel;
using Sudoku.Rendering;
using Sudoku.Text;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class QiuDeadlyPatternType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	[DataMember] scoped in Conjugate conjugatePair
) : QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.ConjugatePair, .2M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, ConjStr]), new(ChineseLanguage, [ConjStr, PatternStr])];

	private string ConjStr => ConjugatePair.ToString();
}
