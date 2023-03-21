namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern Type 1</b> technique.
/// </summary>
public sealed class ChromaticPatternType1Step(
	Conclusion[] conclusions,
	View[]? views,
	int[] blocks,
	scoped in CellMap pattern,
	int extraCell,
	short digitsMask
) : ChromaticPatternStep(conclusions, views, blocks, pattern, digitsMask)
{
	/// <summary>
	/// Indicates the extra cell used.
	/// </summary>
	public int ExtraCell { get; } = extraCell;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { CellsStr, BlocksStr, DigitsStr } },
			{ "zh", new[] { BlocksStr, CellsStr, DigitsStr } }
		};

	/// <inheritdoc/>
	public override Technique Code => Technique.ChromaticPatternType1;
}
