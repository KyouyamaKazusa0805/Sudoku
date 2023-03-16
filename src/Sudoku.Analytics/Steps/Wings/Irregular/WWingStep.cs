namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
public sealed class WWingStep(Conclusion[] conclusions, View[]? views, int startCell, int endCell, scoped in Conjugate conjugatePair) :
	IrregularWingStep(conclusions, views)
{
	/// <summary>
	/// Indicates the start cell.
	/// </summary>
	public int StartCell { get; } = startCell;

	/// <summary>
	/// Indicates the end cell.
	/// </summary>
	public int EndCell { get; } = endCell;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.WWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the conjugate pair connecting with cells <see cref="StartCell"/> and <see cref="EndCell"/>.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { StartCellStr, EndCellStr, ConjStr } },
			{ "zh", new[] { StartCellStr, EndCellStr, ConjStr } }
		};

	private string StartCellStr => RxCyNotation.ToCellString(StartCell);

	private string EndCellStr => RxCyNotation.ToCellString(EndCell);

	private string ConjStr => ConjugatePair.ToString();
}
