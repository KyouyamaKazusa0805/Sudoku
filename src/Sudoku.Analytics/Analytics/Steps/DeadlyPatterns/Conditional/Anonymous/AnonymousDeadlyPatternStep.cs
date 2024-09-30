namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="cells">Indicates cells used.</param>
public abstract partial class AnonymousDeadlyPatternStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap cells
) : ConditionalDeadlyPatternStep(conclusions, views, options), IDeadlyPatternTypeTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 50;

	/// <inheritdoc/>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override bool OnlyUseBivalueCells => false;

	/// <inheritdoc/>
	public sealed override Technique Code => Technique.AnonymousDeadlyPatternType1 + Type - 1;

	/// <inheritdoc/>
	public sealed override Mask DigitsUsed => DigitsMask;
}
