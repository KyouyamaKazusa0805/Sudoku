namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 自定义指令前缀
	/// <para>
	/// 当机器人识别到消息的头部包含指令前缀时触发指令识别功能<br/>
	/// 默认值："/"
	/// </para>
	/// </summary>
	public string CommandPrefix { get; set; } = "/";

	/// <summary>
	/// 缓存动态注册的消息指令事件
	/// </summary>
	private ConcurrentDictionary<string, Command> Commands { get; } = new();


	/// <summary>
	/// 添加消息指令
	/// <para>
	/// 注1：指令匹配忽略消息前的 @机器人 标签，并移除所有前导和尾随空白字符。<br/>
	/// 注2：被指令命中的消息不会再触发 OnAtMessage 和 OnMsgCreate 事件
	/// </para>
	/// </summary>
	/// <param name="command">指令对象</param>
	/// <param name="overwrite">指令名称重复的处理办法<para>true:替换, false:忽略</para></param>
	/// <returns></returns>
	public BotClient AddCommand(Command command, bool overwrite = false)
	{
		string cmdName = command.Name;
		if (Commands.ContainsKey(cmdName))
		{
			if (overwrite)
			{
				Log.Warn($"[CommandManager] 指令 {cmdName} 已存在,已替换新注册的功能!");
				Commands[cmdName] = command;
			}
			else
			{
				Log.Warn($"[CommandManager] 指令 {cmdName} 已存在,已忽略新功能的注册!");
			}
		}
		else
		{
			Log.Info($"[CommandManager] 指令 {cmdName} 已注册.");
			Commands[cmdName] = command;
		}

		return this;
	}

	/// <summary>
	/// 删除消息指令
	/// </summary>
	/// <param name="cmdName">指令名称</param>
	/// <returns></returns>
	public BotClient DelCommand(string cmdName)
	{
		if (Commands.ContainsKey(cmdName))
		{
			if (Commands.Remove(cmdName, out _))
			{
				Log.Info($"[CommandManager] 指令 {cmdName} 已删除.");
			}
			else
			{
				Log.Warn($"[CommandManager] 指令 {cmdName} 删除失败.");
			}
		}
		else
		{
			Log.Warn($"[CommandManager] 指令 {cmdName} 不存在!");
		}

		return this;
	}

	/// <summary>
	/// 获取所有已注册的指令
	/// </summary>
	public List<Command> GetCommands => Commands.Values.ToList();
}
