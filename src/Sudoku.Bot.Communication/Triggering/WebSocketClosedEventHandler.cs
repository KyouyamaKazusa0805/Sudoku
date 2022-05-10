namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that the bot client has closed the connection.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
public delegate void WebSocketClosedEventHandler(BotClient sender);