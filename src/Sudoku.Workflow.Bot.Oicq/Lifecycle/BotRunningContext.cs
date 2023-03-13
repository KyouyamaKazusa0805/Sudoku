namespace Sudoku.Workflow.Bot.Oicq.Lifecycle;

/// <summary>
/// 提供一个上下文，表示机器人在运行期间的数据。
/// </summary>
internal sealed class BotRunningContext
{
	/// <summary>
	/// 表示机器人是否已经被禁言。如果禁言了，指令是无法被发送出去的。这个时候不论执行什么都不行。
	/// </summary>
	public bool IsMuted { get; set; }

	/// <summary>
	/// 表示当前机器人正在运行的长期指令。比如游戏开始期间，无法签到。该属性记录的就是“开始游戏”指令。
	/// </summary>
	public string? ExecutingCommand { get; set; }

	/// <inheritdoc cref="Lifecycle.AnsweringContext"/>
	public AnsweringContext AnsweringContext { get; set; } = new();

	/// <inheritdoc cref="Lifecycle.DrawingContext"/>
	public DrawingContext DrawingContext { get; set; } = new();


	/// <summary>
	/// 尝试根据群号码获得机器人在当前群里的上下文数据。
	/// </summary>
	public static BotRunningContext? GetContext(string group) => RunningContexts.TryGetValue(group, out var result) ? result : null;

	/// <inheritdoc cref="GetContext(string)"/>
	public static BotRunningContext? GetContext(Group group) => GetContext(group.Id);
}
