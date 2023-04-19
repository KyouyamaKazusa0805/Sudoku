namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Last Resort</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class LastResortStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views);
