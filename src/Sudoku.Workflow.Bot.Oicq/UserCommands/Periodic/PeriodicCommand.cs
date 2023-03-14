namespace Sudoku.Workflow.Bot.Oicq.UserCommands.Periodic;

/// <summary>
/// 表示一个周期性执行的指令。这个指令不同于 <see cref="IModule"/> 的模块化处理接口，它属于是单独被 <see cref="MiraiBot"/> 进行调用。
/// </summary>
/// <seealso cref="IModule"/>
/// <seealso cref="MiraiBot"/>
public abstract class PeriodicCommand
{
	/// <summary>
	/// 执行此指令。注意，该方法为无参数方法，如果你需要发送消息，请使用静态方法 <see cref="MessageManager.SendGroupMessageAsync(string, MessageChain)"/>
	/// 或 <see cref="MessageManager.SendGroupMessageAsync(Group, MessageChain)"/> 发送消息流。
	/// </summary>
	/// <returns>一个 <see cref="Task"/> 包裹了异步执行的逻辑。</returns>
	/// <seealso cref="MessageManager.SendGroupMessageAsync(string, MessageChain)"/>
	/// <seealso cref="MessageManager.SendGroupMessageAsync(Group, MessageChain)"/>
	public abstract Task ExecuteAsync();
}
