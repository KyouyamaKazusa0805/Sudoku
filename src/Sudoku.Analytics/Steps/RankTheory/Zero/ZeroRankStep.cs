namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Zero Rank</b> technique.
/// </summary>
public abstract class ZeroRankStep(Conclusion[] conclusions, View[]? views) : RankTheoryStep(conclusions, views);
