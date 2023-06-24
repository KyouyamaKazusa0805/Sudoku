namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Rank Theory</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class RankTheoryStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views);
