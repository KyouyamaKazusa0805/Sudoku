using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Irregular Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class IrregularWingStep(Conclusion[] conclusions, View[]? views) : WingStep(conclusions, views);
