namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Negative Rank</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record NegativeRankStep(Conclusion[] Conclusions, View[]? Views) : RankTheoryStep(Conclusions, Views);
