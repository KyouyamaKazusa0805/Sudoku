namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Negative Rank</b> technique.
/// </summary>
public abstract class NegativeRankStep(Conclusion[] conclusions, View[]? views) : RankTheoryStep(conclusions, views);
