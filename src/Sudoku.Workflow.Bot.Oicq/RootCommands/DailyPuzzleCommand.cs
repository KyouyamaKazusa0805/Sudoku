namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示每日一题的回答功能。
/// </summary>
[Command("每日一题")]
[RequiredRole(BotRole = GroupRoleKind.Owner)]
internal sealed class DailyPuzzleCommand : Command
{
	/// <summary>
	/// 表示你需要回答的问题的答案。
	/// </summary>
	[DoubleArgument("答案")]
	[Hint("表示你需要回答的问题的答案。")]
	[ValueConverter<NumericArrayWithoutSeparatorConverter<Digit>>]
	[DisplayingIndex(0)]
	public Digit[]? Answer { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver.GroupId != SudokuGroupNumber)
		{
			await messageReceiver.SendMessageAsync("抱歉，每日一题仅能在指定的群里使用。");
			return;
		}

		await (Answer is null ? GeneratePuzzleCoreAsync(messageReceiver) : CheckAnswerCoreAsync(messageReceiver));
	}

	/// <summary>
	/// 生成一个题目，并将其存储到本地。
	/// </summary>
	private async Task GeneratePuzzleCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (DailyPuzzleOperations.ReadDailyPuzzleAnswer() is { Puzzle: var grid })
		{
			await messageReceiver.SendMessageAsync("当天已经发布了题目。题目如下：");

			var analyzerResult = PuzzleAnalyzer.Analyze(grid);
			var diffLevel = analyzerResult.DifficultyLevel;
			var diffString = getDifficultyLevelString(diffLevel);
			var diff = analyzerResult.MaxDifficulty;
			await sendPictureAsync(grid.ToString(), $"#{DateTime.Today:yyyyMMdd} 难度级别：{diffString}，难度系数：{diff:0.0}", diffLevel);
			return;
		}

		while (true)
		{
			// 出题。
			grid = Generator.Generate();
			if (PuzzleAnalyzer.Analyze(grid) is not
				{
					IsSolved: true,
					DifficultyLevel: var diffLevel and < DifficultyLevel.Nightmare and not 0,
					MaxDifficulty: var diff and >= 3.4M,
					Solution: var solution
				})
			{
				continue;
			}

			await messageReceiver.SendMessageAsync(
				"""
				【每日一题】
				欢迎使用每日一题功能。每一天都会抽取一道题目给各位完成。每天的题目难度分为容易、一般、困难和极难四种。
				---
				每日一题功能可回答，默认要求回答的是题目的最后一行的 9 个数字（并从左往右书写）。回答正确则可获得奖励。
				每日一题提交答案请使用“！每日一题 答案 <答案>”指令。答案的 9 个数字无需任何符号隔开。
				"""
			);

			// 创建图片并发送出去。
			var diffString = getDifficultyLevelString(diffLevel);
			await sendPictureAsync(grid.ToString(), $"#{DateTime.Today:yyyyMMdd} 难度级别：{diffString}，难度系数：{diff:0.0}", diffLevel);

			// 保存答案到本地。
			DailyPuzzleOperations.WriteDailyPuzzleAnswer(solution);

			// 这是在循环里。这里我们要退出指令，因为已经发送了一个题目。
			return;
		}


		static string getDifficultyLevelString(DifficultyLevel diffLevel)
			=> diffLevel switch
			{
				DifficultyLevel.Easy => "容易",
				DifficultyLevel.Moderate => "一般",
				DifficultyLevel.Hard => "困难",
				DifficultyLevel.Fiendish => "极难"
			};

		async Task sendPictureAsync(string grid, string footerText, DifficultyLevel diffLevel)
		{
			var picturePath = DrawingOperations.GenerateCachedPicturePath(
				ISudokuPainter.Create(1000)
					.WithGridCode(grid)
					.WithRenderingCandidates(diffLevel >= DifficultyLevel.Fiendish)
					.WithFooterText(footerText)
					.WithFontScale(1M, .4M)
			)!;

			await messageReceiver.SendMessageAsync(new ImageMessage { Path = picturePath });
			File.Delete(picturePath);
		}
	}

	/// <summary>
	/// 检查答案是否正确。
	/// </summary>
	private async Task CheckAnswerCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		// 优先撤回用户回复消息，避免后续回答的用户查看答案。
		// 当然，有些人会使用破解工具，破解撤回，但是首先它就不属于不正常的操作；其次，使用其他的验证模式（例如私聊回答答案等）都比起这种回答来说复杂特别多。
		await messageReceiver.RecallAsync();

		if (DailyPuzzleOperations.ReadDailyPuzzleAnswer() is not { Digits: var answer })
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


		static bool sequenceEquals(Digit[] realAnswer, Digit[]? userAnswered)
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
		=> (int)((50 + Rng.Next(-8, 8)) * ScoringOperation.GetGlobalRate(user.CardLevel) * ScoringOperation.GetWeekendFactor());

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
