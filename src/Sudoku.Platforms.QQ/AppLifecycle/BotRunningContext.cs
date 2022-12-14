namespace Sudoku.Platforms.QQ.AppLifecycle;

/// <summary>
/// Defines a context that represents a detail data in a group used by a bot running.
/// </summary>
internal sealed class BotRunningContext
{
	/// <summary>
	/// Indicates whether the bot has been muted by other users in the same group that can mute bot.
	/// </summary>
	public bool IsMuted { get; set; }

	/// <summary>
	/// The command currently executing.
	/// </summary>
	public string? ExecutingCommand { get; set; }

	/// <inheritdoc cref="AppLifecycle.AnsweringContext"/>
	public AnsweringContext AnsweringContext { get; set; } = new();

	/// <inheritdoc cref="AppLifecycle.DrawingContext"/>
	public DrawingContext DrawingContext { get; set; } = new();

	/// <inheritdoc cref="UserDefinedConfigurations"/>
	public UserDefinedConfigurations Configuration { get; set; } = new();


	/// <summary>
	/// Try to fetch <see cref="BotRunningContext"/> instance of the specified group.
	/// </summary>
	/// <param name="group">The group.</param>
	/// <returns>The <see cref="BotRunningContext"/> result if found.</returns>
	public static BotRunningContext? GetContext(string group) => RunningContexts.TryGetValue(group, out var result) ? result : null;

	/// <inheritdoc cref="GetContext(string)"/>
	public static BotRunningContext? GetContext(Group group) => GetContext(group.Id);
}
