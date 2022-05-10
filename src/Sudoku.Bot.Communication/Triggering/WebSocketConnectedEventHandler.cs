namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that the bot client instance has connected the web socket.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
public delegate void WebSocketConnectedEventHandler(BotClient sender);