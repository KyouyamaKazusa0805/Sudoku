namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示开始游戏的指令。
/// </summary>
[Command("数独", IsDebugging = true)]
[CommandUsage("数独 <模式>", IsSyntax = true)]
[CommandUsage("数独 找编号")]
public sealed class PlayGameCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
		=> await api.SendGroupMessageAsync(message, "占位消息");
}
