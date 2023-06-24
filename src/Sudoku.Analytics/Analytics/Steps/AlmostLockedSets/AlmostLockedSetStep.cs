namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class AlmostLockedSetsStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views);
