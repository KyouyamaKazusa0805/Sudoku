namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that a message is audited.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
/// <param name="e">The event data provided.</param>
public delegate void MessageAuditedEventHandler(BotClient sender, MessageAuditedEventArgs? e);