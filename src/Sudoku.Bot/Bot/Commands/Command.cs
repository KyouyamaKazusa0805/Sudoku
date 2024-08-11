namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示一个机器人指令。这个指令不同于配置页面的指令，这个指令可以灵活使用，并不走斜杠 <c>/</c> 触发。也可以是那样的指令。
/// </summary>
public abstract class Command
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


	/// <summary>
	/// 这个方法会在群里艾特并且指令触发时执行。
	/// </summary>
	/// <param name="api">群聊消息 API，用来发送消息。</param>
	/// <param name="message">群消息的提供参数。</param>
	/// <returns>异步函数返回的 <see cref="Task"/> 对象。</returns>
	public abstract Task GroupCallback(ChatMessageApi api, ChatMessage message);


	/// <summary>
	/// 获取程序集里内置的所有指令。
	/// </summary>
	/// <param name="isDebugging">表示指令是否含调试指令。该参数默认为 <see langword="false"/>，即不包含。</param>
	public static Command[] AssemblyCommands(bool isDebugging = false)
		=>
		from type in typeof(Command).Assembly.GetTypes()
		where !type.IsAbstract && type.HasParameterlessConstructor()
		let attribute = type.GetCustomAttribute<CommandAttribute>()
		where attribute is not null
		let isDebuggingCommand = attribute.IsDebugging
		where !isDebuggingCommand || isDebugging
		select (Command)Activator.CreateInstance(type)!;
}
