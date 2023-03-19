namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Template</b> technique.
/// </summary>
public sealed class TemplateStep(Conclusion[] conclusions, View[]? views, bool isTemplateDeletion) : LastResortStep(conclusions, views)
{
	/// <summary>
	/// Indicates the current template step is a template deletion.
	/// </summary>
	public bool IsTemplateDeletion { get; } = isTemplateDeletion;

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.0M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique Code => IsTemplateDeletion ? Technique.TemplateDelete : Technique.TemplateSet;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.Templating;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr } }, { "zh", new[] { DigitStr } } };

	private string DigitStr => (Digit + 1).ToString();
}
