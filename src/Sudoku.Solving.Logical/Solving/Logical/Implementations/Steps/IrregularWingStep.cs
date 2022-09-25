namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Irregular Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record IrregularWingStep(ConclusionList Conclusions, ViewList Views) : WingStep(Conclusions, Views);
