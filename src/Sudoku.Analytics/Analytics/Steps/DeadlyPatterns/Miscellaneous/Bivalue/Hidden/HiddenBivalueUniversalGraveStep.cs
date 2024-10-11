namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Bivalue Universal Grave</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class HiddenBivalueUniversalGraveStep(ReadOnlyMemory<Conclusion> conclusions, View[]? views, StepGathererOptions options) :
	MiscellaneousDeadlyPatternStep(conclusions, views, options),
	IDeadlyPatternTypeTrait
{
	/// <inheritdoc/>
	public sealed override bool OnlyUseBivalueCells => false;

	/// <inheritdoc/>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override int BaseDifficulty => 60;

	/// <inheritdoc/>
	public abstract override Technique Code { get; }
}
