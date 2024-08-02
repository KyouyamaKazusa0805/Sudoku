namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Subset</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="house">Indiscates the house that pattern cells lying.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask that contains all digits used.</param>
public abstract partial class SubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] Mask digitsMask
) : IndirectStep(conclusions, views, options), ISizeTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 30;

	/// <inheritdoc/>
	public int Size => Mask.PopCount(DigitsMask);
}
