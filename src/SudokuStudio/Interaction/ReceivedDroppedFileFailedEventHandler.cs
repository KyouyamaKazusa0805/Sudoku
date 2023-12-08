using SudokuStudio.Views.Controls;

namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="SudokuPane.ReceivedDroppedFileFailed"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="SudokuPane.ReceivedDroppedFileFailed"/>
public delegate void ReceivedDroppedFileFailedEventHandler(object? sender, ReceivedDroppedFileFailedEventArgs e);
