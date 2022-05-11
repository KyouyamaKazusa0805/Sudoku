namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that an event in the specified message reaction
/// has been triggered.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
/// <param name="e">The event data provided.</param>
public delegate void MessageReactionRelatedEventHandler(BotClient sender, MessageReactionRelatedEventArgs e);