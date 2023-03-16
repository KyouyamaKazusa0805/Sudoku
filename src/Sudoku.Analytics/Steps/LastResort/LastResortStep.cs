namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Last Resort</b> technique.
/// </summary>
public abstract class LastResortStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views);
