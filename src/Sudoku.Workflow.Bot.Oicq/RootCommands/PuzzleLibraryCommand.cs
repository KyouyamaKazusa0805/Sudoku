namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示题库指令。
/// </summary>
[Command("题库")]
[RequiredRole(SenderRole = GroupRoleKind.God)]
internal sealed class PuzzleLibraryCommand : Command
{
	/// <summary>
	/// 表示你需要操作题库的具体操作类型。支持的值有“初始化”、“回答”和“设置”。默认情况下该数值是“初始化”。
	/// </summary>
	[DoubleArgument("操作")]
	[Hint("表示你需要操作题库的具体操作类型。支持的值有“初始化”、“回答”和“设置”。默认情况下该数值是“初始化”。")]
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
	/// 表示你需要回答的答案。
	/// </summary>
	/// <remarks>
	/// 由于 OCR 系统极为复杂，而且识别成功率（尤其是手写字体）较低，无法 100% 正确判断结果（尤其是一些题库的难度特别大，
	/// 玩家手写的答案是辛辛苦苦俩小时仨小时才完成的一个题，结果 OCR 识别不了正确答案，这不要人命么），因此回答答案采用字符串的形式进行比较。
	/// 考虑到用户把 81 个空格都写上数字的字符串特别长，所以只要求固定某行/列/宫的 9 个数字作为结果即可。
	/// </remarks>
	[DoubleArgument("答案")]
	[Hint("表示你需要回答的答案。")]
	[DisplayingIndex(3)]
	public string? UserResultAnswer { get; set; }

	/// <summary>
	/// 表示你需要设置的属性名称。该属性配合 <see cref="Operation"/> 为 <see cref="Operations.Update"/> 的时候。支持的有“描述”、“难度”、“标签”和“作者”。
	/// </summary>
	/// <seealso cref="Operations.Update"/>
	[DoubleArgument("属性")]
	[Hint("表示你需要更新的属性名称。支持的有“描述”、“难度”、“标签”和“作者”。")]
	[DisplayingIndex(4)]
	public string? SetPropertyName { get; set; }

	/// <summary>
	/// 表示你需要更新的属性的对应值。若要清除值则缺省该参数；若要赋值“标签”，请用逗号“，”分隔每一个标签。
	/// </summary>
	[DoubleArgument("值")]
	[Hint("表示你需要更新的属性的对应值。若要清除值则缺省该参数；若要赋值“标签”，请用逗号“，”分隔每一个标签。")]
	[DisplayingIndex(5)]
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
			case Operations.Answer:
			{
				if (GroupId is not null || GroupName is not null)
				{
					await messageReceiver.SendMessageAsync("抱歉，回答题库的操作暂不支持跨群题库。");
					break;
				}

				if (PuzzleLibraryName is null)
				{
					await messageReceiver.SendMessageAsync("抱歉，参数“题库名”缺少。无法正确对应到回答的题目上去。");
					break;
				}

				var lib = PuzzleLibraryOperations.GetLibrary(groupId, PuzzleLibraryName);
				if (lib is null)
				{
					await messageReceiver.SendMessageAsync("抱歉，当前题库不存在。请检查输入的题库名称。");
					break;
				}

				var grid = PuzzleLibraryOperations.LoadFromCachedPath(groupId, lib);
				if (grid.IsUndefined)
				{
					await messageReceiver.SendMessageAsync("抱歉，请先抽取题目后，才可进行题目的回答。");
					break;
				}

				if (UserResultAnswer is not { Length: 9 })
				{
					await messageReceiver.SendMessageAsync("抱歉，你回答的结果无法被正确解析为 9 个数字。请勿省略提示数的数字，也不要额外添加别的数据。");
					break;
				}

				if (Array.Exists(UserResultAnswer.ToCharArray(), static character => !char.IsDigit(character)))
				{
					await messageReceiver.SendMessageAsync("抱歉，你回答的结果数据里包含其他符号，无法正确解析为回答结果。");
					break;
				}

				// HousesMap 是一个 CellMap[]，传入的索引器范围是 0 到 26，为 27 个数独基本区域（行 1-9、列 1-9、宫 1-9）。
				// 由于体系设计的问题，宫会被优先计算，因此 0-8 对应了第一到第九个宫，而 9-17 是第一到第九行。所以 17 对应的是最后一行。
				var answerValues = grid.GetSolution()[HousesMap[17]];
				var userResult = (from character in UserResultAnswer select character - '1').ToArray();
				if (!answerEquals(answerValues, userResult))
				{
					await messageReceiver.SendMessageAsync("抱歉，回答错误。");
					break;
				}

				await messageReceiver.SendMessageAsync("恭喜你回答正确。暂时还没有奖励哦~ 等有空了我再为你追加奖励~");

				// 注意，此时因为玩家已经正确回答了结果，所以本地的临时缓存进度文件需要删除掉，并同步更新完成情况。
				PuzzleLibraryOperations.DeleteCachedFile(groupId, lib);

				lib.FinishedPuzzlesCount++;
				PuzzleLibraryOperations.UpdateLibrary(groupId, lib);

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


		static bool answerEquals(int[] answer, int[] userResult)
		{
			// 这里我们没有判断两个数组的长度是否都是 9。因为这个方法在调用之前就已经判断过了。
			// 但是，正是因为这个原因，我才把它定义为了一个本地函数，避免别人将代码直接抽取出去变为类型的成员，从而产生不必要的错误。
			for (var i = 0; i < 9; i++)
			{
				if (answer[i] != userResult[i])
				{
					return false;
				}
			}

			return true;
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
	/// 表示操作为回答题库的答案。
	/// </summary>
	public const string Answer = "回答";

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
