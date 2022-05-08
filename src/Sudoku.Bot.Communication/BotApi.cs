namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the structure of the API message unit.
/// </summary>
/// <param name="Type">The type of the API.</param>
/// <param name="Method">The method called.</param>
/// <param name="Path">The path.</param>
/// <completionlist cref="BotApis"/>
public readonly record struct BotApi(ApiType Type, HttpMethod Method, string Path);
