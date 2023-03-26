namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示题库指令。
/// </summary>
[Command("题库")]
[RequiredRole(SenderRole = GroupRoleKind.God)]
internal sealed class PuzzleLibraryCommand : Command
{
	/// <summary>
	/// 表示你需要操作题库的具体操作类型。默认情况下该数值是“初始化”。
	/// </summary>
	[DoubleArgument("操作")]
	[Hint("表示你需要操作题库的具体操作类型。默认情况下该数值是“初始化”。")]
	[DefaultValue<string>(Operations.Initialize)]
	[DisplayingIndex(0)]
	public string Operation { get; set; } = null!;

	/// <summary>
	/// 表示你要操作的目标群的群名。
	/// </summary>
	[DoubleArgument("群名")]
	[Hint("表示你要操作的目标群的群名。")]
	[DisplayingIndex(1)]
	public string? GroupName { get; set; }

	/// <summary>
	/// 表示你要操作的目标群的群号。
	/// </summary>
	[DoubleArgument("QQ")]
	[Hint("表示你要操作的目标群的群号。")]
	[DisplayingIndex(1)]
	public string? GroupId { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { GroupId: var groupId })
		{
			return;
		}

		switch (Operation, GroupName, GroupId)
		{
			case (Operations.Initialize, null, null):
			{
				await messageReceiver.SendMessageAsync(initializeCore(groupId));
				break;
			}
			case (Operations.Initialize, not null, { } id):
			{
				await messageReceiver.SendMessageAsync("同时指定了“群名”和“QQ”两个参数。参数“群名”会被忽略。");
				await Task.Delay(1000);
				await messageReceiver.SendMessageAsync(initializeCore(id));
				break;
			}
			case (Operations.Initialize, _, { } id):
			{
				await messageReceiver.SendMessageAsync(initializeCore(id));
				break;
			}
			case (Operations.Initialize, { } name, _):
			{
				if ((await AccountManager.GetGroupsAsync()).FirstOrDefault(group => group.Name == name) is not { Id: var id })
				{
					await messageReceiver.SendMessageAsync(
						"""
						指定的群名称的群无法找到，或者是机器人没有添加。
						机器人没有添加的群是无法发送消息的，所以给这种群增添的题库也是没有意义的。
						""".RemoveLineEndings()
					);
					return;
				}

				await messageReceiver.SendMessageAsync(initializeCore(id));
				break;
			}
			default:
			{
				await messageReceiver.SendMessageAsync("指令的参数有误，导致无法成功解析。请检查你的输入。");
				break;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string initializeCore(string groupId)
			=> PuzzleLibraryOperations.Initialize(groupId)
				? "初始化题库数据成功。"
				: "初始化题库数据未完成。可能是本群根本就不包含题库，或者题库创建路径错误等机器人无法识别到题库的情况。";
	}
}

/// <summary>
/// 提供一种操作名称，用于 <see cref="PuzzleLibraryCommand.Operation"/> 属性。
/// </summary>
/// <seealso cref="PuzzleLibraryCommand.Operation"/>
file static class Operations
{
	/// <summary>
	/// 表示初始化题库。
	/// </summary>
	public const string Initialize = "初始化";
}
