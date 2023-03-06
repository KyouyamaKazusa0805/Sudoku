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
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver groupMessageReceiver)
	{
		switch (this)
		{
			case { UserId: null, UserNickname: null }:
			{
				var senderName = groupMessageReceiver.Sender.Name;
				var senderId = groupMessageReceiver.Sender.Id;
				await groupMessageReceiver.QuoteMessageAsync(await showResult(senderName, senderId));

				break;
			}
			case { UserNickname: { } nickname }:
			{
				var matchedMembers = await getMatchedNicknameMembers(groupMessageReceiver.Sender.Group, nickname);
				var targetString = matchedMembers switch
				{
					[] => $"本群不存在昵称为“{nickname}”的用户。请检查一下然后重新查询。",
					[{ Id: var senderId, Name: var senderName }] => await showResult(senderName, senderId),
					_ => "本群存在多个人的群名片一致的情况。请使用 QQ 严格确定唯一的查询用户。"
				};
				await groupMessageReceiver.QuoteMessageAsync(targetString);

				break;
			}
			case { UserId: { } id }:
			{
				var matchedMembers = await getMatchedIdMembers(groupMessageReceiver.Sender.Group, id);
				if (matchedMembers is not { Id: var senderId, Name: var senderName })
				{
					await groupMessageReceiver.QuoteMessageAsync($"本群不存在 QQ 号码为“{id}”的用户。请检查一下后重新查询。");
					break;
				}

				await groupMessageReceiver.QuoteMessageAsync(await showResult(senderName, senderId));

				break;
			}
			default:
			{
				await groupMessageReceiver.QuoteMessageAsync("查询数据不合法。");

				break;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static async Task<Member[]> getMatchedNicknameMembers(Group group, string nickname)
			=> (from m in await @group.GetGroupMembersAsync() where m.Name == nickname select m).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static async Task<Member?> getMatchedIdMembers(Group group, string id)
			=> (from m in await @group.GetGroupMembersAsync() where m.Id == id select m).FirstOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		async Task<string> showResult(string senderName, string senderId)
		{
			var userData = InternalReadWrite.Read(senderId);
			if (userData is not { Score: var score })
			{
				return $"用户 {senderName}（{senderId}）尚未使用过机器人。";
			}

			var grade = Scorer.GetGrade(score);
			var ranking = (await ICommandDataProvider.GetUserRankingListAsync(groupMessageReceiver.Sender.Group, rankingEmptyCallback))!;
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


			async Task rankingEmptyCallback() => await groupMessageReceiver.SendMessageAsync("排名列表为空。");
		}
	}
}
