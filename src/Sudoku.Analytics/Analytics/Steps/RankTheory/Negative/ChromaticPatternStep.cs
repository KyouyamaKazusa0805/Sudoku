namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="blocks">Indicates the blocks that the current pattern lies in.</param>
/// <param name="pattern">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask of digits.</param>
public abstract partial class ChromaticPatternStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] House[] blocks,
	[PrimaryConstructorParameter] scoped in CellMap pattern,
	[PrimaryConstructorParameter] Mask digitsMask
) : NegativeRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.5M;

	private protected string BlocksStr => string.Join(", ", from block in Blocks select $"{block + 1}");

	private protected string CellsStr => Pattern.ToString();

	private protected string DigitsStr => DigitNotation.ToString(DigitsMask);
}
