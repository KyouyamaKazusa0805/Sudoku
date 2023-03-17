namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Positive Rank</b> technique.
/// </summary>
public abstract class PositiveRankStep(Conclusion[] conclusions, View[]? views) : RankTheoryStep(conclusions, views);
