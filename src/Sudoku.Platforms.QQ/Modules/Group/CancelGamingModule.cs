namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
file sealed class CancelGamingModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "结束游戏";

	/// <inheritdoc/>
	public override string RequiredEnvironmentCommand => "开始游戏";


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
