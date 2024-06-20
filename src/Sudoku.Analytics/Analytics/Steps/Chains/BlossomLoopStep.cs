namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Blossom Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern">Indicates the backing pattern.</param>
public sealed partial class BlossomLoopStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] BlossomLoop pattern
) : ChainStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsMultiple => true;

	/// <inheritdoc/>
	public override bool IsDynamic => false;

	/// <inheritdoc/>
	public override int Complexity => Pattern.Complexity;

	/// <inheritdoc/>
	public override int BaseDifficulty => 65;

	/// <inheritdoc/>
	public override Technique Code => Technique.BlossomLoop;
}
