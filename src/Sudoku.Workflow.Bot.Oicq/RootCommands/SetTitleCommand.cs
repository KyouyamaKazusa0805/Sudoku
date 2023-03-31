namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示设置头衔的指令。
/// </summary>
[Command("头衔")]
[RequiredRole(BotRole = GroupRoleKind.Owner)]
[RequiredUserLevel(35)]
internal sealed class SetTitleCommand : Command
{
	/// <summary>
	/// 表示设置的头衔。
	/// </summary>
	[DoubleArgument("内容")]
	[Hint("表示设置的头衔。")]
	[DisplayingIndex(0)]
	[ArgumentDisplayer("头衔名")]
	public string? Title { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { GroupId: var groupId, Sender.Id: var senderId })
		{
			return;
		}

		if (Title is null)
		{
			await messageReceiver.SendMessageAsync("参数“内容”不能为空。");
			return;
		}

		await GroupManager.SetMemberInfoAsync(senderId, groupId, title: Title);
	}
}
