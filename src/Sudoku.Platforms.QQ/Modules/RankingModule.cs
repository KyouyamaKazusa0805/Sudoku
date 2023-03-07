namespace Sudoku.Platforms.QQ.Modules;

[BuiltInModule]
file sealed class RankingModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "排名";

	/// <inheritdoc/>
	public override GroupRoleKind SupportedRoles => GroupRoleKind.GodAccount;


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Group: var group })
		{
			return;
		}

		// If the number of members are too large, we should only iterate the specified number of elements from top.
		var context = BotRunningContext.GetContext(group);
		var usersData = (await Scorer.GetUserRankingListAsync(group, async () => await messageReceiver.SendMessageAsync("群用户列表为空。")))!.Take(10);

		await messageReceiver.SendMessageAsync(
			new MessageChainBuilder()
				.Plain("用户排名：")
				.Plain(Environment.NewLine)
				.Plain(
					string.Join(
						Environment.NewLine,
						usersData.Select(
							static (pair, i) =>
							{
								var score = pair.Data.Score;
								var grade = Scorer.GetGrade(score);
								return $"#{i + 1}：{pair.Name}（{pair.Data.QQ}），{score} 分，{grade} 级";
							}
						)
					)
				)
				.Plain(Environment.NewLine)
				.Plain("---")
				.Plain(Environment.NewLine)
				.Plain("排名最多仅列举本群前十名的成绩；想要精确查看用户名次请使用“查询”指令。")
				.Build()
		);
	}
}
