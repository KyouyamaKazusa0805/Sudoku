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
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique Code => Technique.BruteForce;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { EnglishLanguage, new[] { AssignmentStr } }, { ChineseLanguage, new[] { AssignmentStr } } };

	private string AssignmentStr => ConclusionFormatter.Format(Conclusions, FormattingMode.Normal);
}
