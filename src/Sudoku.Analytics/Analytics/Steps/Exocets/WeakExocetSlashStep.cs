namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Weak Exocet (Slash)</b> technique.
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
public sealed partial class WeakExocetSlashStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Mask digitsMask,
	[PrimaryConstructorParameter] Cell stabilityBalancer,
	[PrimaryConstructorParameter] Cell missingValueCell,
	ref readonly CellMap baseCells,
	ref readonly CellMap targetCells,
	ref readonly CellMap crosslineCells
) : ExocetStep(conclusions, views, options, digitsMask, in baseCells, in targetCells, [], in crosslineCells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 8;

	/// <inheritdoc/>
	public override Technique Code => Technique.WeakExocetSlash;
}
