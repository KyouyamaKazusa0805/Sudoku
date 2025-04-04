namespace Sudoku.Analytics.Steps.Intersections;

/// <summary>
/// Provides with a step that is a <b>Firework</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
public abstract partial class FireworkStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] in CellMap cells,
	[Property] Mask digitsMask
) : IntersectionStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 59;

	/// <inheritdoc/>
	public abstract int Size { get; }

	/// <inheritdoc/>
	public sealed override Mask DigitsUsed => DigitsMask;
}
