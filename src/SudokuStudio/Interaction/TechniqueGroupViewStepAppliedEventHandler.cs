namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="TechniqueGroupView.StepApplied"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="TechniqueGroupView.StepApplied"/>
public delegate void TechniqueGroupViewStepAppliedEventHandler(TechniqueGroupView sender, TechniqueGroupViewStepAppliedEventArgs e);
