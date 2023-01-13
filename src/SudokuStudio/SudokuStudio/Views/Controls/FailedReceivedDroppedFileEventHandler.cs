namespace SudokuStudio.Views.Controls;

/// <summary>
/// Provides an event handler that is used by <see cref="SudokuPane.FailedReceivedDroppedFile"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="SudokuPane.FailedReceivedDroppedFile"/>
public delegate void FailedReceivedDroppedFileEventHandler(object? sender, FailedReceivedDroppedFileEventArgs e);
