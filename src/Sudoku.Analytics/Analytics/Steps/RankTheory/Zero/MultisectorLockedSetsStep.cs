namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-sector Locked Sets</b> technique.
/// </summary>
public sealed class MultisectorLockedSetsStep(Conclusion[] conclusions, View[]? views, scoped in CellMap cells) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.MultisectorLockedSets;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.MultisectorLockedSets;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <summary>
	/// Indicates the cells used in this pattern.
	/// </summary>
	public CellMap Cells { get; } = cells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, A002024(Cells.Count) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { CellsCountStr, CellsStr } }, { "zh", new[] { CellsCountStr, CellsStr } } };

	private string CellsCountStr => Cells.Count.ToString();

	private string CellsStr => Cells.ToString();
}
