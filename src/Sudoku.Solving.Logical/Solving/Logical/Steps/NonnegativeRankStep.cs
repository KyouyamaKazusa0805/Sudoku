namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Non-negative Rank</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record NonnegativeRankStep(Conclusion[] Conclusions, View[]? Views) : RankTheoryStep(Conclusions, Views);
