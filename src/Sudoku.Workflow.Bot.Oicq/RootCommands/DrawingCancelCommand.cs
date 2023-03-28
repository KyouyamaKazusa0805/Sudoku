namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示一个退出结束绘图操作的指令。
/// </summary>
[Command("结束绘图")]
[DependencyCommand<DrawingCommand>]
[RequiredUserLevel(30)]
internal sealed class DrawingCancelCommand : Command
{
	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (!RunningContexts.TryGetValue(messageReceiver.GroupId, out var context))
		{
			return;
		}

		// 重置上下文的数据。
		context.ExecutingCommand = null;

		// 发送信息。
		await messageReceiver.SendMessageAsync("退出绘图操作成功。你可以继续使用其他功能了。");
	}
}
