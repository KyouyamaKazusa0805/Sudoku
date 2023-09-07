using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class WingStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views);
