namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示一个机器人指令。这个指令可以灵活使用必须按斜杠 <c>/</c> 触发。在设计上，不走斜杠触发的指令只有上下文的时候可以用。
/// </summary>
public abstract class Command : ICommandBase
{
	/// <summary>
	/// 表示指令的名称。
	/// </summary>
	public string CommandName => GetType().GetCustomAttribute<CommandAttribute>()!.CommandName;

	/// <summary>
	/// 表示指令的完整名称，包含斜杠。
	/// </summary>
	public string CommandFullName => $"/{CommandName}";

	/// <summary>
	/// 表示指令的描述信息。
	/// </summary>
	public string? Description => GetType().GetCustomAttribute<CommandDescriptionAttribute>()?.Description;

	/// <summary>
	/// 表示指令的样例用法。默认情况下，只有带斜杠和指令名称，如“/签到”。
	/// </summary>
	public string HelpCommandString
		=> ((CommandUsageAttribute[])GetType().GetCustomAttributes<CommandUsageAttribute>())
			.FirstOrDefault(static a => a.IsSyntax)?
			.ExampleUsage
		?? CommandFullName;

	/// <summary>
	/// 表示默认情况下（参数错误等）反馈的字符串。可以用于在参数校验后返回。
	/// </summary>
	protected internal string HelpUsageString => $"用法：“{HelpCommandString}”。";


	/// <inheritdoc/>
	public abstract Task GroupCallback(ChatMessageApi api, ChatMessage message);
}
