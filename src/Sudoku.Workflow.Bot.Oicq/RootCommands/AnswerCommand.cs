namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示回答指令。
/// </summary>
[Command("回答")]
internal sealed class AnswerCommand : Command
{
	/// <summary>
	/// 表示你需要回答的全局可用的内容。支持的参数有“每日一题”。
	/// </summary>
	[DoubleArgument("内容")]
	[Hint("表示你需要回答的全局可用的内容。支持的参数有“每日一题”。")]
	public string? AnswerKind { get; set; }

	/// <summary>
	/// 表示你需要回答的问题的答案。
	/// </summary>
	[DoubleArgument("答案")]
	[Hint("表示你需要回答的问题的答案。")]
	[ValueConverter<NumericArrayWithoutSeparatorConverter<int>>]
	public int[]? Answer { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		switch (AnswerKind, Answer)
		{
			case (_, null):
			{
				await messageReceiver.SendMessageAsync("参数“答案”缺失或非法。请检查你的输入。");
				return;
			}
			case (AnswerKinds.DailyPuzzle, _):
			{
				// 优先撤回用户回复消息，避免后续回答的用户查看答案。
				// 当然，有些人会使用破解工具，破解撤回，但是首先它就不属于不正常的操作，
				// 其次，使用其他的验证模式（例如私聊回答答案等）都比起这种回答来说复杂特别多。
				await messageReceiver.RecallAsync();
				await Task.Delay(100);

				if (StorageHandler.ReadDailyPuzzleAnswer() is not { } answer)
				{
					await messageReceiver.SendMessageAsync("抱歉，当前没有每日一题。");
					return;
				}

				var user = StorageHandler.Read(senderId, new() { Number = senderId });
				if (!sequenceEquals(answer, Answer))
				{
					await messageReceiver.SendMessageAsync("抱歉，你回答的每日一题结果不对。");
					return;
				}

				var exp = LocalScorer.GetExperience(user);
				var coin = LocalScorer.GetCoin(user);
				var item = LocalScorer.GetItem();
				user.ExperiencePoint += exp;
				user.Coin += coin;

				var finalScore = ScoreHandler.GetEarnedScoringDisplayingString(exp);
				var finalCoin = ScoreHandler.GetEarnedCoinDisplayingString(coin);
				if (item is { } earnedItem)
				{
					if (!user.Items.TryAdd(earnedItem, 1))
					{
						user.Items[earnedItem]++;
					}

					await messageReceiver.SendMessageAsync(
						$"""
						恭喜你回答正确。恭喜获得：
						* {finalScore} 经验值
						* {finalCoin} 金币
						* {earnedItem.GetName()} * 1
						"""
					);
				}
				else
				{
					await messageReceiver.SendMessageAsync(
						$"""
						恭喜你回答正确。恭喜获得：
						* {finalScore} 经验值
						* {finalCoin} 金币
						"""
					);
				}

				StorageHandler.Write(user);

				return;
			}


			static bool sequenceEquals(int[] a, int[] b)
			{
				if ((a, b) is not ({ Length: 9 }, { Length: 9 }))
				{
					return false;
				}

				for (var i = 0; i < 9; i++)
				{
					if (a[i] != b[i])
					{
						return false;
					}
				}

				return true;
			}
		}
	}
}

/// <summary>
/// 为参数 <see cref="AnswerCommand.AnswerKind"/> 提供参数。
/// </summary>
/// <seealso cref="AnswerCommand.AnswerKind"/>
file static class AnswerKinds
{
	/// <summary>
	/// 表示每日一题。
	/// </summary>
	public const string DailyPuzzle = "每日一题";
}

/// <summary>
/// 本地的计分规则的方法。
/// </summary>
file static class LocalScorer
{
	/// <summary>
	/// 获得经验值。
	/// </summary>
	/// <param name="user">用户的基本信息。</param>
	/// <returns>经验值获得。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperience(User user)
		=> (int)(30 * ScoreHandler.GetGlobalRate(user.CardLevel) * ScoreHandler.GetWeekendFactor());

	/// <summary>
	/// 获得金币数。
	/// </summary>
	/// <param name="user">用户的基本信息。</param>
	/// <returns>金币数。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoin(User user)
		=> (int)((500 + Rng.Next(-30, 35)) * ScoreHandler.GetGlobalRate(user.CardLevel) * ScoreHandler.GetWeekendFactor());

	/// <summary>
	/// 获得的物品。
	/// </summary>
	/// <returns>获得的物品。可能为 <see langword="null"/>。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Item? GetItem()
		=> Rng.Next(0, 10000) switch
		{
			< 400 => Item.CloverLevel4,
			< 1000 => Item.CloverLevel3,
			< 2500 => Item.CloverLevel2,
			< 4000 => Item.CloverLevel1,
			< 8000 => Item.Card,
			_ => null
		};
}
