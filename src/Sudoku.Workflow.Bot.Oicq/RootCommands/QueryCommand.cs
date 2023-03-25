namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 查询指令。
/// </summary>
[Command("查询")]
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
	public int MainLevel { get; set; }

	/// <summary>
	/// 表示你需要查询的强化期间，三叶草的级别。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。
	/// </summary>
	[DoubleArgument("三叶草")]
	[Hint("表示你需要查询的强化期间，三叶草的级别。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue<int>(-1)]
	[DisplayingIndex(5)]
	public int CloverLevel { get; set; }

	/// <summary>
	/// 表示你需要查询的强化期间，辅助卡的级别，书写格式和“强化”指令里的“辅助”用法一致。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。
	/// </summary>
	[DoubleArgument("辅助")]
	[Hint("表示你需要查询的强化期间，辅助卡的级别，书写格式和“强化”指令里的“辅助”用法一致。该参数必须配合“内容”是“强化”的时候使用，否则该参数没有效果。")]
	[ValueConverter<NumericArrayConverter<int>>]
	[DisplayingIndex(4)]
	public int[]? AuxiliaryCards { get; set; }

	/// <summary>
	/// 表示你需要查询的具体内容。可以是“基本”、“对抗”、“物品”和“强化”。该参数可以没有，默认表示的是查询基本信息，即“基本”。
	/// </summary>
	[DoubleArgument("内容")]
	[Hint("表示你需要查询的具体内容。可以是“基本”、“对抗”、“物品”和“强化”。该参数可以没有，默认表示的是查询基本信息，即“基本”。")]
	[DefaultValue<string>(QueryContentKinds.Elementary)]
	[DisplayingIndex(1)]
	public string? QueryContentKind { get; set; }

	/// <summary>
	/// 表示你需要查询的用户的 QQ 号码。
	/// </summary>
	[DoubleArgument("QQ")]
	[Hint("表示你需要查询的用户的 QQ 号码。")]
	[DisplayingIndex(0)]
	public string? UserId { get; set; }

	/// <summary>
	/// 表示你需要查询的用户的群名片。
	/// </summary>
	[DoubleArgument("昵称")]
	[Hint("表示你需要查询的用户的群名片。")]
	[DisplayingIndex(0)]
	public string? UserNickname { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		_ = (this, messageReceiver) switch
		{
			// 默认情况 -> 查询本人的基本信息。
			({ UserId: null, UserNickname: null, QueryContentKind: var kind }, { Sender: { Group: var group, Name: var senderName, Id: var senderId } })
				=> await messageReceiver.QuoteMessageAsync(await getResultMessage(senderName, senderId, kind, group)),

			// 根据昵称查人。
			({ UserNickname: { } nickname, QueryContentKind: var kind }, { Sender.Group: var group })
				=> await group.GetMatchedMembersViaNicknameAsync(nickname) switch
				{
					var matchedMembers
						=> await messageReceiver.QuoteMessageAsync(
							matchedMembers switch
							{
								[] => $"本群不存在昵称为“{nickname}”的用户。请检查一下然后重新查询。",
								[{ Id: var senderId, Name: var senderName }] => await getResultMessage(senderName, senderId, kind, group),
								_ => "本群存在多个人的群名片一致的情况。请使用 QQ 严格确定唯一的查询用户。"
							}
						)
				},

			// 根据 QQ 号码查人。
			({ UserId: { } id, QueryContentKind: var kind }, { Sender.Group: var group })
				=> await group.GetMatchedMemberViaIdAsync(id) switch
				{
					{ Id: var senderId, Name: var senderName }
						=> await messageReceiver.QuoteMessageAsync(await getResultMessage(senderName, senderId, kind, group)),
					_ => await messageReceiver.QuoteMessageAsync($"本群不存在 QQ 号码为“{id}”的用户。请检查一下后重新查询。")
				},

			// 数据不合法。
			_ => await messageReceiver.QuoteMessageAsync("查询数据不合法。")
		};


		async Task<string> getResultMessage(string senderName, string senderId, string? viewContentKind, Group group)
		{
			return StorageHandler.Read(senderId) switch
			{
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
				} user => viewContentKind switch
				{
					QueryContentKinds.Elementary
						=>
						$"""
						用户 {senderName}（{senderId}）基本数据📦
						---
						经验值：{score}
						金币：{coin}
						级别：{ScoreHandler.GetGrade(score)}
						排名：第 {getRank((await ScoreHandler.GetUserRankingListAsync(group, rankingEmptyCallback))!)} 名
						连续签到天数：{comboCheckedIn}
						签到倍数：{ScoreHandler.GetCheckInRate(comboCheckedIn)}
						总倍数：{ScoreHandler.GetGlobalRate(cardLevel):0.0}（卡片 {cardLevel} 级）
						""",
					QueryContentKinds.PkResult
						=>
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
						""",
					QueryContentKinds.Items
						=>
						$"""
						用户 {senderName}（{senderId}）物品数据📦
						{(
							items.Count != 0
								? string.Join(
									Environment.NewLine,
									from kvp in items
									let item = kvp.Key
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
									let count = kvp.Value
									where count != 0
									select $"  * {level} 级辅助卡：{count} 张"
								)
								: "无"
						)}
						""",
					QueryContentKinds.Upleveling => this switch
					{
						{ AuxiliaryCards: null or [] or { Length: > 3 } }
							=> "查询失败。辅助卡至少需要一张，最多三张，输入的时候使用逗号分开，中间没有空格。",
						{ CloverLevel: < -1 or > 10 }
							=> "查询失败。三叶草等级只能为 1 到 10，或者不填，表示不带三叶草强化。",
						{ AuxiliaryCards: var auxiliary, CloverLevel: var clover, MainLevel: var mainTemp }
							=> (mainTemp == -1 ? cardLevel : mainTemp) switch
							{
								var main
									=> Array.Exists(auxiliary, card => main - card < 0)
										? $"查询失败。主卡级别为 {main}，但填入的辅助卡级别比主卡级别还要高。不支持这种强化。"
										: Array.Exists(auxiliary, card => main - card >= 3)
											? $"查询失败。主卡级别为 {main}，但填入的辅助卡级别存在至少一张卡的等级和主卡级别差了 3 级甚至以上。不支持这种强化。"
											: (
												ScoreHandler.GetUpLevelingSuccessPossibility(main, auxiliary, clover),
												clover == -1 ? string.Empty : $"，三叶草等级：{clover}"
											) switch
											{
												var (p, c) => auxiliary switch
												{
													[var c1]
														=> $"主卡级别：{main}，辅助卡级别：{c1}{c}，成功率：{p:P2}。",
													[var c1, var c2]
														=> $"主卡级别：{main}，辅助卡级别：{c1} 和 {c2}{c}，成功率：{p:P2}。",
													[var c1, var c2, var c3]
														=> $"主卡级别：{main}，辅助卡级别：{c1}、{c2} 和 {c3}{c}，成功率：{p:P2}。"
												}
											}
							},
					},
					_ => "参数“内容”的数值有误——它只能是“对抗”、“基本”、“物品”或“强化”，请检查。"
				},
				_ => $"用户 {senderName}（{senderId}）尚未使用过机器人。"
			};


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
}
