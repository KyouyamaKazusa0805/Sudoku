namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that a message is created.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
public delegate void MessageCreatedEventHandler(Sender sender);