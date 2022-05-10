namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the events using <see cref="JsonElement"/> instance as the event arguments.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
/// <param name="e">The JSON element as the extra event data.</param>
public delegate void JsonElementRelatedEventHandler(BotClient sender, JsonElement e);