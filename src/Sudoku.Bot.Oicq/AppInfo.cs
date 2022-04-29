namespace Sudoku.Bot.Oicq;

/// <summary>
/// Defines an instance that represents for app info.
/// </summary>
/// <param name="Name">Indicates the name of the plugin.</param>
/// <param name="Version">Indicates the version of the plugin.</param>
/// <param name="Author">Indicates the author of the plugin.</param>
/// <param name="Description">Indicates the description of the plugin.</param>
/// <param name="AppId">Indicates the App ID value.</param>
public readonly record struct AppInfo(string Name, string Version, string Author, string Description, string AppId);