namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示一个匿名指令。用户输入艾特并回复内容就会直接触发此指令。这个指令一般只用于长指令环境之中用于回复内容。
/// </summary>
public abstract class AnonymousCommand : IAnonymousCommandBase
{
	/// <summary>
	/// 表示绑定的长指令的名称。
	/// </summary>
	public string CommandName => GetType().GetCustomAttribute<AnonymousCommandAttribute>()!.CommandName;


	/// <inheritdoc/>
	public abstract Task GroupCallback(ChatMessageApi api, ChatMessage message);
}
