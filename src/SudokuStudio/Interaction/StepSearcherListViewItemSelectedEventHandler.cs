namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="StepSearcherListView.ItemSelected"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="StepSearcherListView.ItemSelected"/>
public delegate void StepSearcherListViewItemSelectedEventHandler(StepSearcherListView sender, StepSearcherListViewItemSelectedEventArgs e);
