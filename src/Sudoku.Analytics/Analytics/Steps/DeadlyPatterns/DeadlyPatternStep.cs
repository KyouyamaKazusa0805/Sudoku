namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Deadly Pattern</b> technique.
/// </summary>
public abstract class DeadlyPatternStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views);
