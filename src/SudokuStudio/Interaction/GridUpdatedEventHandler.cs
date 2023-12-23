namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="SudokuPane.GridUpdated"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="SudokuPane.GridUpdated"/>
public delegate void GridUpdatedEventHandler(SudokuPane sender, GridUpdatedEventArgs e);
