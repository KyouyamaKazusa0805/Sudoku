namespace Sudoku.Platforms.QQ.Modules;

[BuiltInModule]
file sealed class LookupModule : GroupModule
{
	private static readonly string ViewContentKindDefaultValue = ViewContentKinds.Elementary;


	/// <inheritdoc/>
	public override string RaisingCommand => "查询";

	/// <summary>
	/// Indicates QQ number of the user.
	/// </summary>
	[DoubleArgument("QQ")]
	public string? UserId { get; set; }

	/// <summary>
	/// Indicates nick name of the user.
	/// </summary>
	[DoubleArgument("昵称")]
	public string? UserNickname { get; set; }

	/// <summary>
	/// Indicates the view content kind.
	/// </summary>
	[DoubleArgument("内容")]
	[DefaultValue(nameof(ViewContentKindDefaultValue))]
	public string ViewContentKind { get; set; } = null!;


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
						倍数：{Scorer.GetScoringRate(userData.ComboCheckedIn)}
						""";
				}
				case ViewContentKinds.PkResult:
				{
					var pkResult = string.Join(
						Environment.NewLine,
						from kvp in userData.TotalPlayingCount
						let mode = kvp.Key
						let tried = userData.TriedCount.TryGetValue(mode, out var r) ? r : 0
						where tried != 0
						let total = kvp.Value
						let corrected = userData.CorrectedCount.TryGetValue(mode, out var r) ? r : 0
						let modeName = mode.GetType().GetField(mode.ToString())!.GetCustomAttribute<NameAttribute>()!.Name
						select $"  * {modeName}：回答数 {tried}，正确数 {corrected}，总答题数 {total}（正确率：{corrected / total:P2}）"
					);

					return
						$"""
						用户 {senderName}（{senderId}）PK 成绩数据📦
						---
						{pkResult}
						""";
				}
				case ViewContentKinds.Items:
				{
					var itemsResult = string.Join(
						Environment.NewLine,
						from kvp in userData.Items
						let item = kvp.Key
						let itemName = item.GetType().GetField(item.ToString())!.GetCustomAttribute<NameAttribute>()!.Name
						let count = kvp.Value
						where count != 0
						select $"  * {itemName}：{count} 个"
					);

					return
						$"""
						用户 {senderName}（{senderId}）商品数据📦
						---
						{itemsResult}
						""";
				}
				default:
				{
					return "参数“内容”的数值有误——它只能是“对抗”、“基本”或“物品”，请检查。";
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
}
