namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示魔塔功能。
/// </summary>
[Command("魔塔")]
[RequiredUserLevel(10)]
internal sealed class TowerSorcererCommand : Command
{
	/// <summary>
	/// 表示你需要回答的答案。该参数可以不写。不写的时候是抽取题目；写的时候，则是你当前爬塔的层级数对应题目的回答答案。
	/// </summary>
	[DoubleArgument("答案")]
	[Hint("表示你需要回答的答案。该参数可以不写。不写的时候是抽取题目；写的时候，则是你当前爬塔的层级数对应题目的回答答案。")]
	[DisplayingIndex(0)]
	[ValueConverter<NumericArrayWithoutSeparatorConverter<int>>]
	public int[]? Answer { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId, GroupId: SudokuGroupNumber })
		{
			await messageReceiver.SendMessageAsync("抱歉，本功能暂只能在机器人设计者规定的群里使用。");
			return;
		}

		var user = UserOperations.Read(senderId)!;
		var count = user.TowerOfSorcerer;
		if (count == 130)
		{
			await messageReceiver.SendMessageAsync("恭喜你，魔塔的全部题目都已经完成！");
			return;
		}

		if (count == 0 && Answer is null)
		{
			await messageReceiver.SendMessageAsync(
				"""
				欢迎您来到魔塔！魔塔是一个全新的机制，您需要按照顺序依次回答魔塔所提供的问题。回答一题计一层。
				层数越高，奖励越丰厚，但难度也会越大哦~
				那么，开始吧！
				"""
			);

			await Task.Delay(1000);
		}

		Grid grid;
		try
		{
			grid = TowerOfSorcererOperations.GetPuzzleFor(count);
		}
		catch (NotSupportedException ex)
		{
			await messageReceiver.SendMessageAsync($"抱歉，{ex.Message}");
			return;
		}

		if (Answer is null)
		{
			// 发送题目。
			await messageReceiver.SendPictureThenDeleteAsync(
				ISudokuPainter.Create(1000)
					.WithGrid(grid)
					.WithRenderingCandidates(count >= 80)
					.WithFooterText($"魔塔 #{count + 1}")
			);

			await Task.Delay(200);
			await messageReceiver.SendMessageAsync("请享受你的题目吧！");

			return;
		}

		// 回答题目。
		// 回答之前先撤回消息，防止别人偷看。
		await messageReceiver.RecallAsync();

		var solution = grid.SolutionGrid;
		var digits = solution[HousesMap[17]];
		if (Answer.Length != 9)
		{
			await messageReceiver.SendMessageAsync("抱歉，请检查你的输入。你输入的答案数据不够或超过 9 个数字。");
			return;
		}

		var flag = true;
		for (var i = 0; i < 9; i++)
		{
			if (Answer[i] - 1 != digits[i])
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			await messageReceiver.SendMessageAsync("抱歉，你的回答有误。");
			return;
		}

		var cardLevel = user.CardLevel;
		var exp = LocalScorer.GetExperiencePoint((count + 1) << 1, cardLevel);
		var coin = LocalScorer.GetCoin((count + 1) * 30 << 1, cardLevel);
		user.ExperiencePoint += exp;
		user.Coin += coin;
		user.TowerOfSorcerer++;

		UserOperations.Write(user);

		var expFinal = ScoringOperation.GetEarnedScoringDisplayingString(exp);
		var coinFinal = ScoringOperation.GetEarnedCoinDisplayingString(coin);
		await messageReceiver.SendMessageAsync($"恭喜你回答正确！你获得了 {expFinal} 经验和 {coinFinal} 金币！");
	}
}

/// <summary>
/// 本地的计分规则的方法。
/// </summary>
file static class LocalScorer
{
	/// <summary>
	/// 获得的经验值。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <param name="cardLevel">用户的卡片级别。</param>
	/// <returns>经验值。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperiencePoint(int @base, int cardLevel)
		=> (int)Round(@base * ScoringOperation.GetGlobalRate(cardLevel)) * ScoringOperation.GetWeekendFactor();

	/// <summary>
	/// 获得的金币。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <param name="cardLevel">用户的卡片级别。</param>
	/// <returns>金币。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoin(int @base, int cardLevel)
		=> (int)Round(@base * ScoringOperation.GetGlobalRate(cardLevel)) * ScoringOperation.GetWeekendFactor();
}
