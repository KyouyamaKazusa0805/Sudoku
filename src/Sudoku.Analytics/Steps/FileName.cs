namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Domino Loop</b> technique.
/// </summary>
public sealed class DominoLoopStep(Conclusion[] conclusions, View[]? views, scoped in CellMap cells) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.6M;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.DominoLoop;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Technique Code => Technique.DominoLoop;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CellsCountStr, CellsStr } }, { "zh", new[] { CellsCountStr, CellsStr } } };

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
