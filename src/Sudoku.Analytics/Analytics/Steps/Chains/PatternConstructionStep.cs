namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern Construction</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="constructedPattern">Indicates the constructed pattern used.</param>
public abstract partial class PatternConstructionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	ChainOrLoop pattern,
	[PrimaryConstructorParameter] object constructedPattern
) : NormalChainStep(conclusions, views, options, pattern)
{
	/// <inheritdoc/>
	public sealed override int BaseDifficulty => base.BaseDifficulty - 5;

	/// <inheritdoc/>
	public abstract override Technique Code { get; }

	/// <inheritdoc/>
	public override TechniqueFormat Format => nameof(NormalChainStep);
}
