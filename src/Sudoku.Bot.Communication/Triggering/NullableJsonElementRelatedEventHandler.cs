namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Indicates the event handler that describes the events using <see cref="JsonElement"/> instance
/// or <see langword="null"/> as the event arguments.
/// </summary>
/// <param name="sender">The object that has triggered the event.</param>
/// <param name="e">The JSON element or <see langword="null"/> as the extra event data.</param>
public delegate void NullableJsonElementRelatedEventHandler(BotClient sender, JsonElement? e);