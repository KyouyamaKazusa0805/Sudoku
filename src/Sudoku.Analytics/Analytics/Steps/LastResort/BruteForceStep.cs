using Sudoku.Analytics.Categorization;
using Sudoku.Rendering;
using Sudoku.Text;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Brute Force</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public sealed class BruteForceStep(Conclusion[] conclusions, View[]? views) : LastResortStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 20.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.BruteForce;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [AssignmentStr]), new(ChineseLanguage, [AssignmentStr])];

	private string AssignmentStr => ConclusionNotation.ToCollectionString(Conclusions);
}
