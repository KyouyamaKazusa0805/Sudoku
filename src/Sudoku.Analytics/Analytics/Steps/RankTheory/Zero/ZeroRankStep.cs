using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Zero Rank</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class ZeroRankStep(Conclusion[] conclusions, View[]? views) : RankTheoryStep(conclusions, views);
