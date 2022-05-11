namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the case that an error has been encountered when calling an API.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
/// <param name="e">The event data provided.</param>
public delegate void ApiEncounteredErrorEventHandler(BotClient? sender, ApiEncounteredErrorEventArgs e);