namespace Sudoku.Platforms.QQ.Modules.Group;

[GroupModule("结束游戏")]
[RequiredDependencyModule<GamingModule>]
file sealed class CancelGamingModule : GroupModule
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
