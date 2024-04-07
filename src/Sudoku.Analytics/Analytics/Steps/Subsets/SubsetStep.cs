namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Subset</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="house">The house that pattern lies in.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask that contains all digits used.</param>
public abstract partial class SubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] scoped ref readonly CellMap cells,
	[PrimaryConstructorParameter] Mask digitsMask
) : Step(conclusions, views, options), ISizeTrait
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.0M;

	/// <inheritdoc/>
	public int Size => PopCount((uint)DigitsMask);
}
