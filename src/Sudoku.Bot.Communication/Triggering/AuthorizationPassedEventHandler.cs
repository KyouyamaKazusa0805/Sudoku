namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that the bot client instance has connected the web socket.
/// </summary>
/// <param name="sender">The object that has triggered the event. The argument may be useless.</param>
/// <param name="e">The event data provided.</param>
public delegate void AuthoizationPassedEventHandler(BotClient? sender, AuthoizationPassedEventArgs e);