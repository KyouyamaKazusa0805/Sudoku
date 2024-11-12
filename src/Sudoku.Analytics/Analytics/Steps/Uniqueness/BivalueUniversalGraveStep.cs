namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class BivalueUniversalGraveStep(StepConclusions conclusions, View[]? views, StepGathererOptions options) :
	MiscellaneousDeadlyPatternStep(conclusions, views, options),
	IDeadlyPatternTypeTrait
{
	/// <inheritdoc/>
	public sealed override bool OnlyUseBivalueCells => true;

	/// <inheritdoc/>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override int BaseDifficulty => 56;

	/// <inheritdoc/>
	public abstract override Technique Code { get; }
}
