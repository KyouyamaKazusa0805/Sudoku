namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="TechniqueGroupView.StepChosen"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="TechniqueGroupView.StepChosen"/>
public delegate void TechniqueGroupViewStepChosenEventHandler(TechniqueGroupView sender, TechniqueGroupViewStepChosenEventArgs e);
