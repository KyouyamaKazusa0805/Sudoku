namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Weak Exocet (Base)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="stabilityBalancer">Indicates the value cell that makes the exocet pattern to be stable.</param>
/// <param name="missingValueCell">Indicates the missing value cell in cross-line.</param>
/// <param name="baseCells"><inheritdoc/></param>
/// <param name="targetCells"><inheritdoc/></param>
/// <param name="crosslineCells"><inheritdoc/></param>
public sealed partial class WeakExocetStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Mask digitsMask,
	[Property] Cell stabilityBalancer,
	[Property] Cell missingValueCell,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap crosslineCells
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 5;

	/// <inheritdoc/>
	public override Technique Code => StabilityBalancer != -1 ? Technique.WeakExocet : Technique.LameWeakExocet;
}
