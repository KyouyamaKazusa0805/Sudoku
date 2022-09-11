namespace Sudoku.Solving.Logics;

/// <summary>
/// Defines a delegate type that handles the event that a step is applied.
/// </summary>
/// <param name="sender">The object that triggers the event.</param>
/// <param name="e">The event arguments provided.</param>
public delegate void StepAppliedEventHandler(object sender, StepAppliedEventArgs e);
