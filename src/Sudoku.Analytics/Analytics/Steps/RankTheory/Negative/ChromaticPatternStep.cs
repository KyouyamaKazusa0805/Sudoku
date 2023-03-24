namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern</b> technique.
/// </summary>
public abstract class ChromaticPatternStep(Conclusion[] conclusions, View[]? views, int[] blocks, scoped in CellMap pattern, short digitsMask) :
	NegativeRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.5M;

	/// <summary>
	/// Indicates the blocks that the current pattern lies in.
	/// </summary>
	public int[] Blocks { get; } = blocks;

	/// <summary>
	/// Indicates the mask of digits.
	/// </summary>
	public short DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.TrivalueOddagon;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Pattern { get; } = pattern;

	private protected string BlocksStr => string.Join(", ", from block in Blocks select $"{block + 1}");

	private protected string CellsStr => Pattern.ToString();

	private protected string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
}
