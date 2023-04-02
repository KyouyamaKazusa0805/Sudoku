namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 查询指令。
/// </summary>
[Command("查询")]
[Usage("！查询 内容 物品", "查询用户自己的物品所有情况。")]
[Usage("！查询 内容 强化 主卡 3 辅助 1，1，1 三叶草 1", "查询强化期间，主卡为 3 级，辅助卡为三张 1 级，带有 1 级三叶草强化时，成功率为多少。")]
[Usage("！查询 内容 题库 群名 摇曳数独 题库名 sdc", "查询群名为“摇曳数独”的“sdc”题库的数据（完成情况等）。")]
internal sealed class QueryCommand : Command
{
	/// <summary>
	/// 表示你需要查询的强化期间，主卡的级别。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。
	/// </summary>
	[DoubleArgument("主卡")]
	[Hint("表示你需要查询的强化期间，主卡的级别。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue<int>(-1)]
	[DisplayingIndex(3)]
	[ArgumentDisplayer("1-16")]
	public int MainLevel { get; set; }

	/// <summary>
	/// 表示你需要查询的强化期间，三叶草的级别。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。
	/// </summary>
	[DoubleArgument("三叶草")]
	[Hint("表示你需要查询的强化期间，三叶草的级别。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue<int>(-1)]
	[DisplayingIndex(5)]
	[ArgumentDisplayer("1-10")]
	public int CloverLevel { get; set; }

	/// <summary>
	/// 表示你需要查询的强化期间，辅助卡的级别，书写格式和“强化”指令里的“辅助”用法一致。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。
	/// </summary>
	[DoubleArgument("辅助")]
	[Hint("表示你需要查询的强化期间，三张辅助卡的级别，书写格式和“强化”指令里的“辅助”用法一致。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。")]
	[ValueConverter<NumericArrayConverter<int>>]
	[DisplayingIndex(4)]
	[ArgumentDisplayer("辅卡1，[辅卡2]，[辅卡3]")]
	public int[]? AuxiliaryCards { get; set; }

	/// <summary>
	/// 表示你需要查询的具体内容。可以是“基本”、“对抗”、“物品”和“强化”。该参数可以没有，默认表示的是查询基本信息，即“基本”。
	/// </summary>
	[DoubleArgument("内容")]
	[Hint("表示你需要查询的具体内容。可以是“基本”、“对抗”、“物品”和“强化”。该参数可以没有，默认表示的是查询基本信息，即“基本”。")]
	[DefaultValue<string>(QueryContentKinds.Elementary)]
	[DisplayingIndex(0)]
	public string? QueryContentKind { get; set; }

	/// <summary>
	/// 表示你需要查询的用户的 QQ 号码。
	/// </summary>
	[DoubleArgument("账号")]
	[Hint("表示你需要查询的用户的 QQ 号码。")]
	[DisplayingIndex(1)]
	[ArgumentDisplayer("QQ")]
	public string? UserId { get; set; }

	/// <summary>
	/// 表示你需要查询的用户的群名片。
	/// </summary>
	[DoubleArgument("昵称")]
	[Hint("表示你需要查询的用户的群名片。")]
	[DisplayingIndex(1)]
	[ArgumentDisplayer("名称")]
	public string? UserNickname { get; set; }

	/// <summary>
	/// 表示你需要查询的群的群号。该参数主要用于查询例如题库之类的内容。
	/// </summary>
	[DoubleArgument("群号")]
	[Hint("表示你需要查询的群的群号。该参数主要用于查询例如题库之类的内容。")]
	[DisplayingIndex(2)]
	[ArgumentDisplayer("QQ")]
	public string? GroupId { get; set; }

	/// <summary>
	/// 表示你需要查询的群的群名。该参数主要用于查询例如题库之类的内容。
	/// </summary>
	[DoubleArgument("群名")]
	[Hint("表示你需要查询的群的群名。该参数主要用于查询例如题库之类的内容。")]
	[DisplayingIndex(2)]
	public string? GroupName { get; set; }

	/// <summary>
	/// 表示你需要查询的题库名称。
	/// </summary>
	[DoubleArgument("题库名")]
	[Hint("表示你需要查询的题库名称。")]
	[DisplayingIndex(6)]
	[ArgumentDisplayer("名称")]
	public string? PuzzleLibraryName { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		switch (QueryContentKind)
		{
			case QueryContentKinds.PuzzleLibrary:
			{
				switch (this)
				{
					// 查询当前群的所有题库。
					case { GroupName: null, GroupId: null, PuzzleLibraryName: null }:
					{
						await getResultMessage_PuzzleLibraries(messageReceiver.GroupId);
						break;
					}

					// 查询题库信息。
					case { GroupName: null, GroupId: null, PuzzleLibraryName: { } name }:
					{
						await getResultMessage_PuzzleLibrary(messageReceiver.GroupId, name);
						break;
					}

					// 查询跨群题库信息。
					case { GroupId: { } groupId, PuzzleLibraryName: var name }:
					{
						await (name is null ? getResultMessage_PuzzleLibraries(groupId) : getResultMessage_PuzzleLibrary(groupId, name));
						break;
					}

					// 查询跨群题库信息，指定名称。
					case { GroupName: { } groupName, PuzzleLibraryName: var name }:
					{
						var groups = (from g in await AccountManager.GetGroupsAsync() where g.Name == groupName select g).ToArray();
						if (groups is not [{ Id: var groupId }])
						{
							await messageReceiver.SendMessageAsync("机器人添加的群里包含多个重名的群，或者没有找到该名称的群。请使用“群号”严格确定群。");
							break;
						}

						await (name is null ? getResultMessage_PuzzleLibraries(groupId) : getResultMessage_PuzzleLibrary(groupId, name));
						break;
					}
				}
				break;
			}
			case var kind:
			{
				switch (this, messageReceiver)
				{
					// 默认情况 -> 查询本人的基本信息。
					case ({ UserId: null, UserNickname: null }, { Sender: { Group: var group, Name: var senderName, Id: var senderId } }):
					{
						await getResultMessage(senderName, senderId, kind, group);
						break;
					}

					// 根据昵称查人。
					case ({ UserNickname: { } nickname }, { Sender.Group: var group }):
					{
						switch (await group.GetMatchedMembersViaNicknameAsync(nickname))
						{
							case []:
							{
								await messageReceiver.SendMessageAsync($"本群不存在昵称为“{nickname}”的用户。请检查一下然后重新查询。");
								break;
							}
							case [{ Id: var senderId, Name: var senderName }]:
							{
								await getResultMessage(senderName, senderId, kind, group);
								break;
							}
							default:
							{
								await messageReceiver.SendMessageAsync("本群存在多个人的群名片一致的情况。请使用 QQ 严格确定唯一的查询用户。");
								break;
							}
						}
						break;
					}

					// 根据 QQ 号码查人。
					case ({ UserId: { } id }, { Sender.Group: var group }):
					{
						switch (await group.GetMatchedMemberViaIdAsync(id))
						{
							case { Id: var senderId, Name: var senderName }:
							{
								await getResultMessage(senderName, senderId, kind, group);
								break;
							}
							default:
							{
								await messageReceiver.SendMessageAsync($"本群不存在 QQ 号码为“{id}”的用户。请检查一下后重新查询。");
								break;
							}
						}
						break;
					}

					// 数据不合法。
					default:
					{
						await messageReceiver.SendMessageAsync("查询数据不合法。");
						break;
					}
				}
				break;
			}
		}


		async Task getResultMessage_PuzzleLibraries(string groupId)
			=> _ = PuzzleLibraryOperations.GetLibraries(groupId) switch
			{
				{ Length: var length } libs and not [] when (from lib in libs select lib.Name) is var libraryNames
					=> await messageReceiver.SendMessageAsync(
						$"""
						本群题库：{string.Join("、", libraryNames)}
						题库总数量：{length}
						---
						如需要使用题库，请使用“！抽题”指令。
						"""
					),
				_ => await messageReceiver.SendMessageAsync("本群尚不存在任何题库。")
			};

		async Task getResultMessage_PuzzleLibrary(string groupId, string name)
		{
			switch (PuzzleLibraryOperations.GetLibrary(groupId, name))
			{
				case
				{
					Name: var libName,
					Author: var author,
					Description: var description,
					DifficultyText: var difficultyText,
					Tags: var tags,
					FinishedPuzzlesCount: var count
				} lib:
				{
					var totalNumberOfPuzzles = PuzzleLibraryOperations.GetPuzzlesCount(lib);
					await messageReceiver.SendMessageAsync(
						$"""
						题库信息：
						---
						题库：{libName}
						作者：{author ?? "<匿名>"}
						描述：{description ?? "<无描述>"}
						难度：{difficultyText ?? "<未知>"}
						标签：{(tags is null ? "<未设置>" : string.Join('，', tags))}
						题目数量：{totalNumberOfPuzzles}
						已完成：{count} 题
						完成进度：{count / (double)totalNumberOfPuzzles:P2}
						"""
					);

					break;
				}
				default:
				{
					await messageReceiver.SendMessageAsync($"当前群不包含名称为“{name}”的题库。查询失败。");
					break;
				}
			}
		}

		async Task getResultMessage(string senderName, string senderId, string? viewContentKind, Group group)
		{
			switch (UserOperations.Read(senderId))
			{
				case
				{
					ExperiencePoint: var score,
					Coin: var coin,
					ComboCheckedIn: var comboCheckedIn,
					CardLevel: var cardLevel,
					TotalPlayingCount: var playingCount,
					TriedCount: var triedCount,
					CorrectedCount: var correctedCount,
					Items: var items,
					UplevelingCards: var uplevelingCards
				} user:
				{
					switch (viewContentKind)
					{
						case QueryContentKinds.Elementary:
						{
							await messageReceiver.SendMessageAsync(
								$"""
								用户 {senderName}（{senderId}）基本数据📦
								---
								经验值：{score}
								金币：{coin}
								级别：{ScoringOperation.GetGrade(score)}
								排名：第 {getRank((await ScoringOperation.GetUserRankingListAsync(group, rankingEmptyCallback))!)} 名
								连续签到天数：{comboCheckedIn}
								签到倍数：{ScoringOperation.GetCheckInRate(comboCheckedIn)}
								总倍数：{ScoringOperation.GetGlobalRate(cardLevel):0.0}（卡片 {cardLevel} 级）
								"""
							);
							break;
						}
						case QueryContentKinds.PkResult:
						{
							await messageReceiver.SendMessageAsync(
								$"""
								用户 {senderName}（{senderId}）PK 数据📦
								---
								{(
									playingCount.Count != 0
										? string.Join(
											Environment.NewLine,
											from kvp in playingCount
											let mode = kvp.Key
											let tried = triedCount.TryGetValue(mode, out var r) ? r : 0
											where tried != 0
											let total = kvp.Value
											let corrected = correctedCount.TryGetValue(mode, out var r) ? r : 0
											let modeName = mode.GetType().GetField(mode.ToString())!.GetCustomAttribute<NameAttribute>()!.Name
											select $"  * {modeName}：回答数 {tried}，正确数 {corrected}，总答题数 {total}（正确率：{corrected / total:P2}）"
										)
										: "无"
								)}
								"""
							);
							break;
						}
						case QueryContentKinds.Items:
						{
							await messageReceiver.SendMessageAsync(
								$"""
								用户 {senderName}（{senderId}）物品数据📦
								{(
									items.Count != 0
										? string.Join(
											Environment.NewLine,
											from kvp in items
											let item = kvp.Key
											orderby item
											let itemName = item.GetType().GetField(item.ToString())!.GetCustomAttribute<NameAttribute>()!.Name
											let count = kvp.Value
											where count != 0
											select $"  * {itemName}：{count} 个"
										)
										: "无"
								)}
								---
								辅助卡片情况：
								{(
									uplevelingCards.Count != 0
										? string.Join(
											Environment.NewLine,
											from kvp in uplevelingCards
											let level = kvp.Key
											orderby level
											let count = kvp.Value
											where count != 0
											select $"  * {level} 级辅助卡：{count} 张"
										)
										: "无"
								)}
								"""
							);
							break;
						}
						case QueryContentKinds.Upleveling:
						{
							switch (this)
							{
								case { AuxiliaryCards: null or [] or { Length: > 3 } }:
								{
									await messageReceiver.SendMessageAsync("查询失败。辅助卡至少需要一张，最多三张，输入的时候使用逗号分开，中间没有空格。");
									break;
								}
								case { CloverLevel: < -1 or > 10 }:
								{
									await messageReceiver.SendMessageAsync("查询失败。三叶草等级只能为 1 到 10，或者不填，表示不带三叶草强化。");
									break;
								}
								case { AuxiliaryCards: var auxiliary, CloverLevel: var clover, MainLevel: var mainTemp }:
								{
									var main = mainTemp == -1 ? cardLevel : mainTemp;
									await messageReceiver.SendMessageAsync(
										Array.Exists(auxiliary, card => main - card < 0)
										? $"查询失败。主卡级别为 {main}，但填入的辅助卡级别比主卡级别还要高。不支持这种强化。"
										: Array.Exists(auxiliary, card => main - card >= 3)
											? $"查询失败。主卡级别为 {main}，但填入的辅助卡级别存在至少一张卡的等级和主卡级别差了 3 级甚至以上。不支持这种强化。"
											: (
												ScoringOperation.GetUpLevelingSuccessPossibility(main, auxiliary, clover),
												clover == -1 ? string.Empty : $"，三叶草等级：{clover}"
											) switch
											{
												var (p, c) => auxiliary switch
												{
													[var c1] => $"主卡级别：{main}，辅助卡级别：{c1}{c}，成功率：{p:P2}。",
													[var c1, var c2] => $"主卡级别：{main}，辅助卡级别：{c1} 和 {c2}{c}，成功率：{p:P2}。",
													[var c1, var c2, var c3] => $"主卡级别：{main}，辅助卡级别：{c1}、{c2} 和 {c3}{c}，成功率：{p:P2}。"
												}
											}
									);
									break;
								}
							}
							break;
						}
						case QueryContentKinds.PuzzleLibrary:
						{
							await getResultMessage_PuzzleLibraries(group.Id);
							break;
						}
						default:
						{
							await messageReceiver.SendMessageAsync("参数“内容”的数值有误——它只能是“对抗”、“基本”、“物品”、“强化”或“题库”，请检查。");
							break;
						}
					}
					break;
				}
				case var _ when viewContentKind is QueryContentKinds.Elementary or QueryContentKinds.PkResult:
				{
					await messageReceiver.SendMessageAsync($"用户 {senderName}（{senderId}）尚未使用过机器人。");
					break;
				}
			}


			int getRank((string, User Data)[] ranking)
			{
				for (var i = 0; i < ranking.Length; i++)
				{
					if (ranking[i].Data.Number == senderId)
					{
						return i + 1;
					}
				}

				return int.MaxValue;
			}

			async Task rankingEmptyCallback() => await messageReceiver.SendMessageAsync("排名列表为空。");
		}
	}
}

/// <summary>
/// 表示查询的内容。
/// </summary>
file static class QueryContentKinds
{
	/// <summary>
	/// 表示查询 PK 成绩。
	/// </summary>
	public const string PkResult = "对抗";

	/// <summary>
	/// 表示查询用户基本数据。
	/// </summary>
	public const string Elementary = "基本";

	/// <summary>
	/// 表示查询用户的物品数据。
	/// </summary>
	public const string Items = "物品";

	/// <summary>
	/// 表示查询强化的成功率情况。
	/// </summary>
	public const string Upleveling = "强化";

	/// <summary>
	/// 表示查询本群的题库数据。
	/// </summary>
	public const string PuzzleLibrary = "题库";
}
