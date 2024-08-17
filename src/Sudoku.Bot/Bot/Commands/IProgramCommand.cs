namespace Sudoku.Bot.Commands;

/// <summary>
/// 提供一个指令的基础类。
/// </summary>
/// <typeparam name="TSelf">当前类型。</typeparam>
/// <typeparam name="TAttribute">特性的类型。</typeparam>
public interface IProgramCommand<TSelf, TAttribute>
	where TSelf : class, IProgramCommand<TSelf, TAttribute>
	where TAttribute : CommandBaseAttribute
{
	/// <summary>
	/// 这个方法会在群里艾特并且指令触发时执行。
	/// </summary>
	/// <param name="api">群聊消息 API，用来发送消息。</param>
	/// <param name="message">群消息的提供参数。</param>
	/// <returns>异步函数返回的 <see cref="Task"/> 对象。该方法应以异步函数声明（<see langword="async"/> 修饰）。</returns>
	public abstract Task GroupCallback(ChatMessageApi api, ChatMessage message);


	/// <summary>
	/// 获取程序集里内置的所有指令。
	/// </summary>
	/// <param name="isDebugging">表示指令是否含调试指令。该参数默认为 <see langword="false"/>，即不包含。</param>
	public static sealed TSelf[] AssemblyCommands(bool isDebugging = false)
		=>
		from type in typeof(TSelf).Assembly.GetTypes()
		where !type.IsAbstract && type.HasParameterlessConstructor()
		let attribute = type.GetCustomAttribute<TAttribute>()
		where attribute is not null
		let isDebuggingCommand = attribute.IsDebugging
		where !isDebuggingCommand || isDebugging
		select (TSelf)Activator.CreateInstance(type)!;
}
