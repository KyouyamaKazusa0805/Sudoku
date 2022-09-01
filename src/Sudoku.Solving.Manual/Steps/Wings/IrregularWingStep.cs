namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Irregular Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract partial record IrregularWingStep(ConclusionList Conclusions, ViewList Views) : WingStep(Conclusions, Views);
