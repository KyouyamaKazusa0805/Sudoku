namespace Sudoku.Platforms.QQ.Modules;

[BuiltInModule]
file sealed class RankingModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "排名";

	/// <inheritdoc/>
	public override GroupRoleKind SupportedRoles => GroupRoleKind.GodAccount;


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver groupMessageReceiver)
	{
		if (groupMessageReceiver is not { Sender.Group: var group })
		{
			return;
		}

		// If the number of members are too large, we should only iterate the specified number of elements from top.
		var context = BotRunningContext.GetContext(group);
		var usersData = (
			await ICommandDataProvider.GetUserRankingListAsync(
				group,
				async () => await groupMessageReceiver.SendMessageAsync("群用户列表为空。")
			)
		)!.Take(10);

		await groupMessageReceiver.SendMessageAsync(
			new MessageChainBuilder()
				.Plain("用户排名：")
				.Plain(Environment.NewLine)
				.Plain("---")
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
				.Build()
		);
	}
}
