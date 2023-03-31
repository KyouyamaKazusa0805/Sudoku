namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示每日一题的回答功能。
/// </summary>
[Command("每日一题")]
internal sealed class DailyPuzzleAnswerCommand : Command
{
	/// <summary>
	/// 表示你需要回答的问题的答案。
	/// </summary>
	[DoubleArgument("答案")]
	[Hint("表示你需要回答的问题的答案。")]
	[ValueConverter<NumericArrayWithoutSeparatorConverter<int>>]
	[DisplayingIndex(0)]
	public int[]? Answer { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		// 本来这里有一段是撤回消息防止其他人偷看的代码的。
		// 但是想了又想，我还是觉得不加这段比较好。靠自觉吧。毕竟每日一题是拿来讨论的。得分只是次要的。

		if (DailyPuzzleOperations.ReadDailyPuzzleAnswer() is not { } answer)
		{
			await messageReceiver.SendMessageAsync("抱歉，当前没有每日一题。");
			return;
		}

		var user = UserOperations.Read(senderId, new() { Number = senderId });
		if (user.LastAnswerDailyPuzzle.Date == DateTime.Today)
		{
			await messageReceiver.SendMessageAsync("抱歉，你今日已完成正确作答。请勿重复作答。");
			return;
		}

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

		var finalScore = ScoringOperation.GetEarnedScoringDisplayingString(exp);
		var finalCoin = ScoringOperation.GetEarnedCoinDisplayingString(coin);
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

		user.LastAnswerDailyPuzzle = DateTime.Today;
		UserOperations.Write(user);


		static bool sequenceEquals(int[] realAnswer, int[]? userAnswered)
		{
			if ((realAnswer, userAnswered) is not ({ Length: 9 }, { Length: 9 }))
			{
				return false;
			}

			for (var i = 0; i < 9; i++)
			{
				if (realAnswer[i] != userAnswered[i] - 1)
				{
					return false;
				}
			}

			return true;
		}
	}
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
	public static int GetExperience(User user) => (int)(30 * ScoringOperation.GetGlobalRate(user.CardLevel) * ScoringOperation.GetWeekendFactor());

	/// <summary>
	/// 获得金币数。
	/// </summary>
	/// <param name="user">用户的基本信息。</param>
	/// <returns>金币数。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoin(User user)
		=> (int)((500 + Rng.Next(-30, 35)) * ScoringOperation.GetGlobalRate(user.CardLevel) * ScoringOperation.GetWeekendFactor());

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
