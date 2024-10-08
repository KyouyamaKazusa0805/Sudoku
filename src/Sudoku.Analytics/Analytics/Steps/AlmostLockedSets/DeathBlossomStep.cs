namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class DeathBlossomStep(ReadOnlyMemory<Conclusion> conclusions, View[]? views, StepGathererOptions options) :
	AlmostLockedSetsStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 82;
}
