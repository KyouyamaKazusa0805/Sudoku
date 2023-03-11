namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
file sealed class LookupModule : GroupModule
{
#pragma warning disable CS0414
	private static readonly string ViewContentKindDefaultValue = ViewContentKinds.Elementary;
	private static readonly int CloverLevelDefaultValue = -1;
	private static readonly int MainLevelDefaultValue = -1;
#pragma warning restore CS0414


	/// <inheritdoc/>
	public override string RaisingCommand => "查询";

	/// <summary>
	/// Indicates QQ number of the user.
	/// </summary>
	[DoubleArgumentCommand("QQ")]
	public string? UserId { get; set; }

	/// <summary>
	/// Indicates nick name of the user.
	/// </summary>
	[DoubleArgumentCommand("昵称")]
	public string? UserNickname { get; set; }

	/// <summary>
	/// Indicates the view content kind.
	/// </summary>
	[DoubleArgumentCommand("内容")]
	[DefaultValue(nameof(ViewContentKindDefaultValue))]
	public string ViewContentKind { get; set; } = null!;

	/// <summary>
	/// Indicates the main card level.
	/// </summary>
	[DoubleArgumentCommand("主卡")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue(nameof(MainLevelDefaultValue))]
	public int MainLevel { get; set; }

	/// <summary>
	/// Indicates the clover level.
	/// </summary>
	[DoubleArgumentCommand("三叶草")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue(nameof(CloverLevelDefaultValue))]
	public int CloverLevel { get; set; }

	/// <summary>
	/// Indicates the auxiliary cards.
	/// </summary>
	[DoubleArgumentCommand("辅助")]
	[ValueConverter<NumericArrayConverter<int>>]
	public int[]? AuxiliaryCards { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender: { Group: var group } sender })
		{
			return;
		}

		switch (this)
		{
			case { UserId: null, UserNickname: null, ViewContentKind: var kind }
			when sender is { Name: var senderName, Id: var senderId }:
			{
				await messageReceiver.QuoteMessageAsync(await showResult(senderName, senderId, kind));

				break;
			}
			case { UserNickname: { } nickname, ViewContentKind: var kind }:
			{
				var matchedMembers = await group.GetMatchedMembersViaNicknameAsync(nickname);
				var targetString = matchedMembers switch
				{
					[] => $"本群不存在昵称为“{nickname}”的用户。请检查一下然后重新查询。",
					[{ Id: var senderId, Name: var senderName }] => await showResult(senderName, senderId, kind),
					_ => "本群存在多个人的群名片一致的情况。请使用 QQ 严格确定唯一的查询用户。"
				};
				await messageReceiver.QuoteMessageAsync(targetString);

				break;
			}
			case { UserId: { } id, ViewContentKind: var kind }:
			{
				var matchedMembers = await group.GetMatchedMemberViaIdAsync(id);
				if (matchedMembers is not { Id: var senderId, Name: var senderName })
				{
					await messageReceiver.QuoteMessageAsync($"本群不存在 QQ 号码为“{id}”的用户。请检查一下后重新查询。");
					break;
				}

				await messageReceiver.QuoteMessageAsync(await showResult(senderName, senderId, kind));

				break;
			}
			default:
			{
				await messageReceiver.QuoteMessageAsync("查询数据不合法。");

				break;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		async Task<string> showResult(string senderName, string senderId, string viewContentKind)
		{
			var userData = InternalReadWrite.Read(senderId);
			if (userData is not { ExperiencePoint: var score })
			{
				return $"用户 {senderName}（{senderId}）尚未使用过机器人。";
			}

			switch (viewContentKind)
			{
				case ViewContentKinds.Elementary:
				{
					var grade = Scorer.GetGrade(score);
					var ranking = (await Scorer.GetUserRankingListAsync(group, rankingEmptyCallback))!;
					var rankingIndex = int.MaxValue;
					for (var i = 0; i < ranking.Length; i++)
					{
						if (ranking[i].Data.QQ == senderId)
						{
							rankingIndex = i + 1;
							break;
						}
					}

					return
						$"""
						用户 {senderName}（{senderId}）基本数据📦
						---
						经验值：{score}
						金币：{userData.Coin}
						级别：{grade}
						排名：第 {rankingIndex} 名
						连续签到天数：{userData.ComboCheckedIn}
						签到倍数：{Scorer.GetCheckInRate(userData.ComboCheckedIn)}
						总倍数：{Scorer.GetGlobalRate(userData.CardLevel):0.0}（卡片 {userData.CardLevel} 级）
						""";
				}
				case ViewContentKinds.PkResult:
				{
					var pkResult = userData.TotalPlayingCount.Count != 0
						? string.Join(
							Environment.NewLine,
							from kvp in userData.TotalPlayingCount
							let mode = kvp.Key
							let tried = userData.TriedCount.TryGetValue(mode, out var r) ? r : 0
							where tried != 0
							let total = kvp.Value
							let corrected = userData.CorrectedCount.TryGetValue(mode, out var r) ? r : 0
							let modeName = mode.GetType().GetField(mode.ToString())!.GetCustomAttribute<NameAttribute>()!.Name
							select $"  * {modeName}：回答数 {tried}，正确数 {corrected}，总答题数 {total}（正确率：{corrected / total:P2}）"
						)
						: "无";

					return
						$"""
						用户 {senderName}（{senderId}）PK 数据📦
						---
						{pkResult}
						""";
				}
				case ViewContentKinds.Items:
				{
					var itemsResult = userData.Items.Count != 0
						? string.Join(
							Environment.NewLine,
							from kvp in userData.Items
							let item = kvp.Key
							let itemName = item.GetType().GetField(item.ToString())!.GetCustomAttribute<NameAttribute>()!.Name
							let count = kvp.Value
							where count != 0
							select $"  * {itemName}：{count} 个"
						)
						: "无";

					var auxiliaryCardResult = userData.UplevelingCards.Count != 0
						? string.Join(
							Environment.NewLine,
							from kvp in userData.UplevelingCards
							let level = kvp.Key
							let count = kvp.Value
							where count != 0
							select $"  * {level} 级辅助卡：{count} 张"
						)
						: "无";

					return
						$"""
						用户 {senderName}（{senderId}）物品数据📦
						{itemsResult}
						---
						辅助卡片情况：
						{auxiliaryCardResult}
						""";
				}
				case ViewContentKinds.Upleveling:
				{
					if (AuxiliaryCards is null or [] or { Length: > 3 })
					{
						return "查询失败。辅助卡至少需要一张，最多三张，输入的时候使用逗号分开，中间没有空格。";
					}

					if (CloverLevel is < -1 or > 10)
					{
						return "查询失败。三叶草等级只能为 1 到 10，或者不填，表示不带三叶草强化。";
					}

					var main = MainLevel == -1 ? userData.CardLevel : MainLevel;
					if (Array.Exists(AuxiliaryCards, card => main - card < 0))
					{
						return $"查询失败。主卡级别为 {main}，但填入的辅助卡级别比主卡级别还要高。不支持这种强化。";
					}

					if (Array.Exists(AuxiliaryCards, card => main - card >= 3))
					{
						return $"查询失败。主卡级别为 {main}，但填入的辅助卡级别存在至少一张卡的等级和主卡级别差了 3 级甚至以上。不支持这种强化。";
					}

					var possibility = Scorer.GetUpLevelingSuccessPossibility(main, AuxiliaryCards, CloverLevel);
					var cloverString = CloverLevel == -1 ? string.Empty : $"，三叶草等级：{CloverLevel}";
					return AuxiliaryCards switch
					{
						[var c] => $"主卡级别：{main}，辅助卡级别：{c}{cloverString}，成功率：{possibility:P2}。",
						[var c1, var c2] => $"主卡级别：{main}，辅助卡级别：{c1} 和 {c2}{cloverString}，成功率：{possibility:P2}。",
						[var c1, var c2, var c3] => $"主卡级别：{main}，辅助卡级别：{c1}、{c2} 和 {c3}{cloverString}，成功率：{possibility:P2}。"
					};
				}
				default:
				{
					return "参数“内容”的数值有误——它只能是“对抗”、“基本”、“物品”或“强化”，请检查。";
				}
			}


			async Task rankingEmptyCallback() => await messageReceiver.SendMessageAsync("排名列表为空。");
		}
	}
}

file static class ViewContentKinds
{
	public const string PkResult = "对抗";
	public const string Elementary = "基本";
	public const string Items = "物品";
	public const string Upleveling = "强化";
}
