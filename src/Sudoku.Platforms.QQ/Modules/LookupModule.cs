namespace Sudoku.Platforms.QQ.Modules;

[BuiltInModule]
file sealed class LookupModule : GroupModule
{
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


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		switch (this)
		{
			case { UserId: null, UserNickname: null }:
			{
				var senderName = messageReceiver.Sender.Name;
				var senderId = messageReceiver.Sender.Id;
				await messageReceiver.QuoteMessageAsync(await showResult(senderName, senderId));

				break;
			}
			case { UserNickname: { } nickname }:
			{
				var matchedMembers = await messageReceiver.Sender.Group.GetMatchedMembersViaNicknameAsync(nickname);
				var targetString = matchedMembers switch
				{
					[] => $"本群不存在昵称为“{nickname}”的用户。请检查一下然后重新查询。",
					[{ Id: var senderId, Name: var senderName }] => await showResult(senderName, senderId),
					_ => "本群存在多个人的群名片一致的情况。请使用 QQ 严格确定唯一的查询用户。"
				};
				await messageReceiver.QuoteMessageAsync(targetString);

				break;
			}
			case { UserId: { } id }:
			{
				var matchedMembers = await messageReceiver.Sender.Group.GetMatchedMemberViaIdAsync(id);
				if (matchedMembers is not { Id: var senderId, Name: var senderName })
				{
					await messageReceiver.QuoteMessageAsync($"本群不存在 QQ 号码为“{id}”的用户。请检查一下后重新查询。");
					break;
				}

				await messageReceiver.QuoteMessageAsync(await showResult(senderName, senderId));

				break;
			}
			default:
			{
				await messageReceiver.QuoteMessageAsync("查询数据不合法。");

				break;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		async Task<string> showResult(string senderName, string senderId)
		{
			var userData = InternalReadWrite.Read(senderId);
			if (userData is not { Score: var score })
			{
				return $"用户 {senderName}（{senderId}）尚未使用过机器人。";
			}

			var grade = Scorer.GetGrade(score);
			var ranking = (await Scorer.GetUserRankingListAsync(messageReceiver.Sender.Group, rankingEmptyCallback))!;
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
				用户 {senderName}（{senderId}）数据📦
				---
				分数：{score}
				级别：{grade}
				排名：第 {rankingIndex} 名
				""";


			async Task rankingEmptyCallback() => await messageReceiver.SendMessageAsync("排名列表为空。");
		}
	}
}
