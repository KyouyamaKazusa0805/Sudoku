namespace Sudoku.Workflow.Bot.Oicq.UserCommands;

/// <summary>
/// 取消游戏指令。
/// </summary>
[Command("结束游戏")]
[DependencyModule<GameCommand>]
internal sealed class GameCancelCommand : Command
{
	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		var context = RunningContexts[messageReceiver.GroupId];
		if (context.AnsweringContext.IsCancelled)
		{
			return;
		}

		context.AnsweringContext.IsCancelled = true;
		await Task.Delay(10);
	}
}
