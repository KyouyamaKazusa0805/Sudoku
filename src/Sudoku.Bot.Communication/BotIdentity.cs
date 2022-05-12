namespace Sudoku.Bot.Communication;

/// <summary>
/// The identity instance.
/// </summary>
/// <param name="AppId">The ID of the bot. The value corresponds to the AppID.</param>
/// <param name="Token">The token of the bot.</param>
/// <param name="Secret">The secret key of the bot.</param>
public readonly record struct BotIdentity(string AppId, string Token, string Secret);
