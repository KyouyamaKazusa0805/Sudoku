namespace Nano;

/// <summary>
/// Indicates the event handler that is bound with the concept on sudoku grid being changed.
/// </summary>
/// <param name="sender">The sender that triggers the event.</param>
/// <param name="e">The event arguments provided.</param>
public delegate void GridChangedEventHandler(object? sender, GridChangedEventArgs e);