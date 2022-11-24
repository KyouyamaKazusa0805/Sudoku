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
}
