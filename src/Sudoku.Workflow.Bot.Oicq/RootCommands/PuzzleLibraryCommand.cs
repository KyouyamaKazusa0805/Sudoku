namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示题库指令。
/// </summary>
[Command("题库")]
[Usage("！题库 操作 初始化", "初始化本群所有题库的信息，将数据重置。")]
[Usage("！题库 操作 设置 题库名 sdc 属性 作者 SunnieShine", "将本群题库“SDC”的作者配置项改成“SunnieShine”。")]
[RequiredRole(SenderRole = GroupRoleKind.God)]
internal sealed class PuzzleLibraryCommand : Command
{
	/// <summary>
	/// 表示你需要操作题库的具体操作类型。支持的值有“初始化”、“回答”和“设置”。默认情况下该数值是“初始化”。
	/// </summary>
	[DoubleArgument("操作")]
	[Hint("表示你需要操作题库的具体操作类型。支持的值有“初始化”和“设置”。默认情况下该数值是“初始化”。")]
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
	[DoubleArgument("群号")]
	[Hint("表示你要操作的目标群的群号。")]
	[DisplayingIndex(1)]
	public string? GroupId { get; set; }

	/// <summary>
	/// 表示你要回答的题目所在题库的题库名称。
	/// </summary>
	[DoubleArgument("题库名")]
	[Hint("表示你要回答的题目所在题库的题库名称。")]
	[DisplayingIndex(2)]
	public string? PuzzleLibraryName { get; set; }

	/// <summary>
	/// 表示你需要设置的属性名称。该属性配合 <see cref="Operation"/> 为 <see cref="Operations.Update"/> 的时候。支持的有“描述”、“难度”、“标签”和“作者”。
	/// </summary>
	/// <seealso cref="Operations.Update"/>
	[DoubleArgument("属性")]
	[Hint("表示你需要更新的属性名称。支持的有“描述”、“难度”、“标签”和“作者”。")]
	[DisplayingIndex(3)]
	public string? SetPropertyName { get; set; }

	/// <summary>
	/// 表示你需要更新的属性的对应值。若要清除值则缺省该参数；若要赋值“标签”，请用逗号“，”分隔每一个标签。
	/// </summary>
	[DoubleArgument("值")]
	[Hint("表示你需要更新的属性的对应值。若要清除值则缺省该参数；若要赋值“标签”，请用逗号“，”分隔每一个标签。")]
	[DisplayingIndex(4)]
	public string? Value { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { GroupId: var groupId })
		{
			return;
		}

		switch (Operation)
		{
			case Operations.Initialize:
			{
				switch (GroupName, GroupId)
				{
					case (null, null):
					{
						await messageReceiver.SendMessageAsync(initializeCore(groupId));
						break;
					}
					case (not null, { } id):
					{
						await messageReceiver.SendMessageAsync("同时指定了“群名”和“QQ”两个参数。参数“群名”会被忽略。");
						await Task.Delay(1000);
						await messageReceiver.SendMessageAsync(initializeCore(id));
						break;
					}
					case (_, { } id):
					{
						await messageReceiver.SendMessageAsync(initializeCore(id));
						break;
					}
					case ({ } name, _):
					{
						var groups = (from @group in await AccountManager.GetGroupsAsync() where @group.Name == name select @group).ToArray();
						if (groups is not [{ Id: var id }])
						{
							await messageReceiver.SendMessageAsync("机器人添加的群里包含多个重名的群，或者没有找到该名称的群。请使用“群号”严格确定群。");
							break;
						}

						await messageReceiver.SendMessageAsync(initializeCore(id));
						break;
					}
				}
				break;
			}
			case Operations.Update:
			{
				if (SetPropertyName is null)
				{
					await messageReceiver.SendMessageAsync("必要的参数“属性”缺失。请给出该参数的数值。");
					break;
				}

				if (PuzzleLibraryName is null)
				{
					await messageReceiver.SendMessageAsync("必要的参数“题库名”缺失。请给出该参数的数值。");
					break;
				}

				if (PuzzleLibraryOperations.GetLibrary(groupId, PuzzleLibraryName) is not { } lib)
				{
					await messageReceiver.SendMessageAsync($"抱歉，指定题库名称“{PuzzleLibraryName}”的题库不存在。请检查输入是否有误。");
					break;
				}

				// Value 此时不作判定。如果不赋值，那么 Value 就是默认数值 null，那么就表示该参数缺省，用于清空该属性的结果。
				switch (SetPropertyName)
				{
					case SetPropertyNames.Author:
					{
						lib.Author = Value;
						PuzzleLibraryOperations.UpdateLibrary(groupId, lib);

						await messageReceiver.SendMessageAsync("属性“作者”更新成功。");
						break;
					}
					case SetPropertyNames.DifficultyText:
					{
						lib.DifficultyText = Value;
						PuzzleLibraryOperations.UpdateLibrary(groupId, lib);

						await messageReceiver.SendMessageAsync("属性“难度”更新成功。");
						break;
					}
					case SetPropertyNames.Description:
					{
						lib.Description = Value;
						PuzzleLibraryOperations.UpdateLibrary(groupId, lib);

						await messageReceiver.SendMessageAsync("属性“描述”更新成功。");
						break;
					}
					case SetPropertyNames.Tags:
					{
						lib.Tags = Value?.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
						PuzzleLibraryOperations.UpdateLibrary(groupId, lib);

						await messageReceiver.SendMessageAsync("属性“标签”更新成功。");
						break;
					}
					default:
					{
						await messageReceiver.SendMessageAsync("抱歉，参数“属性”的数值不合法。它目前只支持“难度”、“描述”、“标签”和“作者”这几个值。");
						break;
					}
				}
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

	/// <summary>
	/// 表示操作为设置属性的数值。
	/// </summary>
	public const string Update = "设置";
}

/// <summary>
/// 为 <see cref="PuzzleLibraryCommand.SetPropertyName"/> 提供数值。
/// </summary>
/// <seealso cref="PuzzleLibraryCommand.SetPropertyName"/>
file static class SetPropertyNames
{
	/// <summary>
	/// 表示你要设置的是作者这个属性。它对应了 <see cref="PuzzleLibrary.Author"/> 属性。
	/// </summary>
	/// <seealso cref="PuzzleLibrary.Author"/>
	public const string Author = "作者";

	/// <summary>
	/// 表示你要设置的是难度这个属性。它对应了 <see cref="PuzzleLibrary.DifficultyText"/> 属性。
	/// </summary>
	/// <seealso cref="PuzzleLibrary.DifficultyText"/>
	public const string DifficultyText = "难度";

	/// <summary>
	/// 表示你要设置的是描述这个属性。它对应了 <see cref="PuzzleLibrary.Description"/> 属性。
	/// </summary>
	/// <seealso cref="PuzzleLibrary.Description"/>
	public const string Description = "描述";

	/// <summary>
	/// 表示你要设置的是标签这个属性。它对应了 <see cref="PuzzleLibrary.Tags"/> 属性。
	/// </summary>
	/// <seealso cref="PuzzleLibrary.Tags"/>
	public const string Tags = "标签";
}
