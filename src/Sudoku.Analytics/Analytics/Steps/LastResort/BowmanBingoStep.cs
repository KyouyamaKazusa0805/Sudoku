namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bowman's Bingo</b> technique.
/// </summary>
public sealed class BowmanBingoStep(Conclusion[] conclusions, View[]? views, Conclusion[] contradictionLinks) : LastResortStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.0M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { (ExtraDifficultyCaseNames.Length, ChainDifficultyRating.GetExtraDifficultyByLength(ContradictionLinks.Length)) };

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique Code => Technique.BowmanBingo;

	/// <summary>
	/// Indicates the list of contradiction links.
	/// </summary>
	public Conclusion[] ContradictionLinks { get; } = contradictionLinks;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { ContradictionSeriesStr } }, { "zh", new[] { ContradictionSeriesStr } } };

	private string ContradictionSeriesStr => ConclusionFormatter.Format(ContradictionLinks, " -> ", false);
}
