namespace Sudoku.UI.Data;

/// <summary>
/// Represents an event handler that occurs when the the status of the undoing stack is changed.
/// The delegate type is used for the event declaration which binds the concept with the undoing stack.
/// </summary>
/// <param name="sender">The object that triggers the event.</param>
public delegate void UndoStackChangedEventHandler(object sender);