namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 游戏指令。
/// </summary>
[Command("开始游戏")]
[SupportedOSPlatform(OperatingSystemNames.Windows)]
internal sealed class GameCommand : Command
{
	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		var separator = new string(' ', 4);

		var context = RunningContexts[messageReceiver.GroupId];
		context.ExecutingCommand = RaisingCommand;
		context.AnsweringContext = new();

		// 确定题目。
		var (puzzle, chosenCells, finalCellIndex, timeLimit, baseExp) = generatePuzzle();

		await messageReceiver.SendMessageAsync(
			$"""
			题目生成中，请稍候……
			游戏规则：机器人将给出一道标准数独题，你需要在 {timeLimit.ToChineseTimeSpanString()}内完成它，并找出 9 个位置里唯一一个编号和填数一致的单元格。回答结果（该编号）时请艾特机器人，格式为“@机器人 数字”；若所有格子的编号都和当前格子的填数不同，请用数字“0”表示。回答正确将自动结束游戏并获得相应积分，错误则会扣除一定积分；题目可反复作答，但连续错误会导致连续扣分哦！若无人作答完成题目，将自动作废，游戏自动结束。
			"""
		);
		await Task.Delay(2.Seconds());

		// 将问题里的对象给标注到绘图对象上去。
		var selectedNodes = chosenCells.Select(markerNodeSelector).ToArray();

		// 根据绘图对象直接创建图片，然后发送出去。
		await messageReceiver.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle)
				.WithRenderingCandidates(false)
				.WithNodes(selectedNodes)
				.WithPreferenceSettings(static pref => pref.UnknownIdentifierColor = Color.FromArgb(96, Color.Red))
		);

		var answeringContext = context.AnsweringContext;

		answeringContext.CurrentRoundAnsweredValues.Clear();

		// 使用反复的循环来等待用户回答。
		for (int timeLastSeconds = (int)timeLimit.TotalSeconds, elapsedTime = 0; timeLastSeconds > 0; timeLastSeconds--, elapsedTime++)
		{
			// 将 1 秒钟拆分为 4 个 250 毫秒为单位的循环单位，即按每 250 毫秒一次判断用户数据是否回答正确。
			for (var internalTimes = 0; internalTimes < 4; internalTimes++)
			{
				await Task.Delay(250);

				// 判别是否用户在取消指令里取消了游戏。
				if (context.AnsweringContext.IsCancelled is true)
				{
					// 用户取消了游戏。
					await messageReceiver.SendMessageAsync("游戏已中断。");

					goto ReturnTrueAndInitializeContext;
				}

				/**
					判别用户的回答是否正确。
					这里我们用到的是 <see cref="AnsweringContext.CurrentRoundAnsweredValues"/> 属性。
				*/
				foreach (var data in answeringContext.CurrentRoundAnsweredValues)
				{
					if (data is not { Conclusion: var answeredCellIndex, User: { Id: var userId, Name: var userName } })
					{
						continue;
					}

					switch (answeredCellIndex - 1 == finalCellIndex, answeringContext.AnsweredUsers.ContainsKey(userId))
					{
						case (false, false):
						{
							// 回答错误，而且这里没有记录这个用户。说明用户第一次回答。
							await messageReceiver.SendMessageAsync("回答错误~");
							answeringContext.AnsweredUsers.TryAdd(userId, 1);

							break;
						}
						case (false, true):
						{
							// 回答错误，单本地有这个人的回答数据。
							await messageReceiver.SendMessageAsync("回答错误~");
							answeringContext.AnsweredUsers[userId]++;

							break;
						}
						case (true, _):
						{
							var scoringTableLines =
								from pair in answeringContext.AnsweredUsers
								let currentUserId = pair.Key
								let times = pair.Value
								let deduct = -Enumerable.Range(1, times).Sum(LocalScorer.GetExperiencePointDeduct)
								let currentUser = messageReceiver.Sender.Group.GetMatchedMemberViaIdAsync(currentUserId).Result
								let isCorrectedUser = currentUserId == userId
								select new ResultInfo(
									currentUser.Name,
									currentUserId,
									deduct + (isCorrectedUser ? baseExp : 0),
									isCorrectedUser ? LocalScorer.GetCoinOriginal() : 0,
									isCorrectedUser ? LocalScorer.GetEarnedItem() : null,
									times,
									isCorrectedUser
								);

							if (!scoringTableLines.Any(e => e.Id == userId))
							{
								scoringTableLines = scoringTableLines.Prepend(
									new(
										userName,
										userId,
										baseExp,
										LocalScorer.GetCoinOriginal(),
										LocalScorer.GetEarnedItem(),
										1,
										true
									)
								);
							}

							// 回答正确。为正确结果进行答复，并记录经验值和金币数。
							var elapsedTimeString = TimeSpan.FromSeconds(elapsedTime).ToChineseTimeSpanString();
							await messageReceiver.SendMessageAsync(
								$"""
								恭喜玩家“{userName}”（{userId}）回答正确！耗时 {elapsedTimeString}。游戏结束！
								---
								{(
									scoringTableLines.Any()
										? string.Join(
											Environment.NewLine,
											from tuple in scoringTableLines
											let user = StorageHandler.Read(tuple.Id, new() { Number = tuple.Id })
											let scoreEarned = LocalScorer.GetExperiencePoint(tuple.ExperiencePoint, user.CardLevel)
											let coinEarned = LocalScorer.GetCoin(tuple.Coin, user.CardLevel)
											let finalScoreToDisplay = ScoreHandler.GetEarnedScoringDisplayingString(scoreEarned)
											let finalCoinToDisplay = ScoreHandler.GetEarnedCoinDisplayingString(coinEarned)
											let earnedItemName = tuple.EarnedItem is { } earnedItem ? $"，{earnedItem.GetName()} * 1" : string.Empty
											select $"{tuple.UserName}（{tuple.Id}）{separator}{finalScoreToDisplay} 经验，{finalCoinToDisplay} 金币{earnedItemName}"
										)
										: "无"
								)}
								"""
							);

							appendOrDeduceScore(scoringTableLines);

							goto ReturnTrueAndInitializeContext;
						}
					}
				}

				answeringContext.CurrentRoundAnsweredValues.Clear();
			}
		}

		// 执行扣分。
		// 执行到这里说明没有人回答正确题目。
		var scoringTableLinesDeductOnly =
			from pair in answeringContext.AnsweredUsers
			let currentUserId = pair.Key
			let times = pair.Value
			let deduct = -Enumerable.Range(1, times).Sum(LocalScorer.GetExperiencePointDeduct)
			let currentUser = messageReceiver.Sender.Group.GetMatchedMemberViaIdAsync(currentUserId).Result
			select new ResultInfo(currentUser.Name, currentUserId, deduct, 0, default, times, false);

		appendOrDeduceScore(scoringTableLinesDeductOnly);

		await messageReceiver.SendMessageAsync(
			$"""
			游戏时间到~ 游戏结束~
			---
			{(
				scoringTableLinesDeductOnly.Any()
					? string.Join(
						Environment.NewLine,
						from tuple in scoringTableLinesDeductOnly
						let user = StorageHandler.Read(tuple.Id, new() { Number = tuple.Id })
						let scoreEarned = LocalScorer.GetExperiencePoint(tuple.ExperiencePoint, user.CardLevel)
						let coinEarned = LocalScorer.GetCoin(tuple.Coin, user.CardLevel)
						let finalScoreToDisplay = ScoreHandler.GetEarnedScoringDisplayingString(scoreEarned)
						let finalCoinToDisplay = ScoreHandler.GetEarnedCoinDisplayingString(coinEarned)
						select $"{tuple.UserName}（{tuple.Id}）{separator}{finalScoreToDisplay} 经验，{finalCoinToDisplay} 金币"
					)
					: "无"
			)}
			"""
		);

		// 发送答案。
		await messageReceiver.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle.GetSolution())
				.WithRenderingCandidates(false)
				.WithNodes(
					finalCellIndex == -1
						? selectedNodes
						: selectedNodes.Prepend(new CellViewNode(DisplayColorKind.Normal, chosenCells[finalCellIndex]))
				)
				.WithPreferenceSettings(static pref => pref.UnknownIdentifierColor = Color.FromArgb(96, Color.Red))
		);

	ReturnTrueAndInitializeContext:
		context.ExecutingCommand = null;


		static void appendOrDeduceScore(IEnumerable<ResultInfo> scoringTableLines)
		{
			foreach (var (_, id, score, coin, earnedItemNullable, times, isCorrectedUser) in scoringTableLines)
			{
				var user = StorageHandler.Read(id, new() { Number = id, LastCheckIn = DateTime.MinValue });
				user.ExperiencePoint += LocalScorer.GetExperiencePoint(score, user.CardLevel);
				user.Coin += LocalScorer.GetCoin(coin, user.CardLevel);

				if (isCorrectedUser && !user.CorrectedCount.TryAdd(GameMode.FindDifference, 1))
				{
					user.CorrectedCount[GameMode.FindDifference]++;
				}

				if (!user.TriedCount.TryAdd(GameMode.FindDifference, times))
				{
					user.TriedCount[GameMode.FindDifference] += times;
				}

				if (!user.TotalPlayingCount.TryAdd(GameMode.FindDifference, 1))
				{
					user.TotalPlayingCount[GameMode.FindDifference]++;
				}

				if (earnedItemNullable is { } earnedItem && !user.Items.TryAdd(earnedItem, 1))
				{
					user.Items[earnedItem]++;
				}

				StorageHandler.Write(user);
			}
		}

		static GeneratedGridData generatePuzzle()
		{
			scoped var finalCellsChosen = (stackalloc int[9]);
			while (true)
			{
				var grid = Generator.Generate();
				switch (Solver.Analyze(grid))
				{
					case { IsSolved: true, Solution: var solution, DifficultyLevel: var l and <= DifficultyLevel.Hard }:
					{
						var expEarned = LocalScorer.GetExperiencePointPerPuzzle(l);
						var timeLimit = GetGameTimeLimit(l);

						finalCellsChosen.Clear();
						var chosenCells = CellMap.Empty;
						var emptyCells = grid.EmptyCells;
						var puzzleIsInvalid = false;
						for (var digit = 0; digit < 9; digit++)
						{
							var tempMap = emptyCells & grid.CandidatesMap[digit];
							if (!tempMap)
							{
								// 本题目不包含任何空格包含这个数。
								puzzleIsInvalid = true;
								goto CheckPuzzleValidity;
							}

							var copiedMap = tempMap;
							foreach (var tempCell in tempMap)
							{
								if (solution[tempCell] == digit || chosenCells.Contains(tempCell))
								{
									copiedMap.Remove(tempCell);
								}
							}
							if (!copiedMap)
							{
								// 无法选择合适的单元格。
								puzzleIsInvalid = true;
								goto CheckPuzzleValidity;
							}

							var cell = copiedMap[Rng.Next(0, copiedMap.Count)];
							chosenCells.Add(cell);
							finalCellsChosen[digit] = cell;
						}

					CheckPuzzleValidity:
						if (puzzleIsInvalid)
						{
							continue;
						}

						if (Rng.Next(0, 1000) < 666)
						{
							var digitChosen = Rng.Next(0, 9);
							foreach (var cell in emptyCells & grid.CandidatesMap[digitChosen])
							{
								if (solution[cell] == digitChosen && !chosenCells.Contains(cell))
								{
									finalCellsChosen[digitChosen] = cell;
									break;
								}
							}

							return new(grid, finalCellsChosen.ToArray(), digitChosen, timeLimit, expEarned);
						}

						return new(grid, finalCellsChosen.ToArray(), -1, timeLimit, expEarned);
					}
				}
			}
		}

		static ViewNode markerNodeSelector(int element, int i)
			=> new BabaGroupViewNode(DisplayColorKind.Normal, element, (Utf8Char)(char)(i + '1'), 511);
	}


	/// <summary>
	/// 根据题目难度，求得题目限时。
	/// </summary>
	/// <param name="difficultyLevel">难度的级别。</param>
	/// <returns>该题目的限时。</returns>
	/// <exception cref="NotSupportedException">如果题目难度未知，或超过了 <see cref="DifficultyLevel.Hard"/>，就会产生此异常。</exception>
	private static TimeSpan GetGameTimeLimit(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => 5.Minutes(),
			DifficultyLevel.Moderate => new TimeSpan(0, 5, 30),
			DifficultyLevel.Hard => 6.Minutes(),
			_ => throw new NotSupportedException("The specified difficulty is not supported.")
		};
}

/// <summary>
/// 表示生成的题目数据。
/// </summary>
/// <param name="Puzzle">题目。</param>
/// <param name="ChosenCells">选取出来的单元格。</param>
/// <param name="FinalIndex">最终定下来的单元格（答案）。</param>
/// <param name="TimeLimit">这个题目的限时。</param>
/// <param name="ExperiencePoint">该题目可获得的经验值。</param>
file sealed record GeneratedGridData(scoped in Grid Puzzle, int[] ChosenCells, int FinalIndex, TimeSpan TimeLimit, int ExperiencePoint);

/// <summary>
/// 表示题目的结果信息。
/// </summary>
/// <param name="UserName">表示该结果是谁回答的。</param>
/// <param name="Id">该用户的 QQ 号码。</param>
/// <param name="ExperiencePoint">该题目可获得的经验值。</param>
/// <param name="Coin">该题目可获得的金币。</param>
/// <param name="EarnedItem">获得的物品。可能为 <see langword="null"/>。</param>
/// <param name="Times">本题该用户的用时。</param>
/// <param name="IsCorrectedUser">表示这个人是否回答正确。</param>
file sealed record ResultInfo(string UserName, string Id, int ExperiencePoint, int Coin, Item? EarnedItem, int Times, bool IsCorrectedUser);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 将 <see cref="TimeSpan"/> 实例用中文汉字“时分秒”的形式表达为字符串写法。
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToChineseTimeSpanString(this TimeSpan @this)
		=> @this switch
		{
			{ Minutes: 0 } => $"{@this:s' 秒'}",
			{ Seconds: 0 } => $"{@this:m' 分钟'}",
			_ => $"{@this:m' 分 'ss' 秒'}",
		};

	/// <summary>
	/// 根据指定的回调函数生成 <see cref="ISudokuPainter"/> 对象，将该对象产生的图片保存至本地，然后将其发送出去，然后自动删掉本地的图片缓存。
	/// </summary>
	/// <param name="this">用来发送消息的对象。</param>
	/// <param name="painterCreator">
	/// <inheritdoc cref="StorageHandler.GenerateCachedPicturePath(Func{ISudokuPainter}?)" path="/param[@name='painterCreator']"/>
	/// </param>
	/// <returns>一个 <see cref="Task"/> 对象，包裹了异步执行的基本信息。</returns>
	[SupportedOSPlatform(OperatingSystemNames.Windows)]
	public static async Task SendPictureThenDeleteAsync(this GroupMessageReceiver @this, Func<ISudokuPainter>? painterCreator = null)
	{
		painterCreator ??= () => RunningContexts[@this.GroupId].DrawingContext.Painter!;
		var picturePath = StorageHandler.GenerateCachedPicturePath(painterCreator)!;

		await @this.SendMessageAsync(new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
	}
}

/// <summary>
/// 本地的计分规则的方法。
/// </summary>
file static class LocalScorer
{
	/// <summary>
	/// 根据连续答错的次数确定扣分。
	/// </summary>
	/// <param name="times">同一个题目答错的次数。</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">如果 <paramref name="times"/> 为负数，就会产生此异常。</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperiencePointDeduct(int times)
		=> times switch
		{
			0 => 0,
			1 => 6,
			2 => 12,
			>= 3 => 18,
			_ => throw new ArgumentOutOfRangeException(nameof(times))
		} * ScoreHandler.GetWeekendFactor();

	/// <summary>
	/// 获得金币获得的基础数值。
	/// </summary>
	/// <returns>金币数。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoinOriginal() => 480 + Rng.Next(-6, 7);

	/// <summary>
	/// 获得经验值的基础数值。
	/// </summary>
	/// <param name="difficultyLevel">题目难度。</param>
	/// <returns>经验值。</returns>
	/// <exception cref="NotSupportedException">如果 <paramref name="difficultyLevel"/> 未定义，就会产生此异常。</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 如果 <paramref name="difficultyLevel"/> 超过了 <see cref="DifficultyLevel.Hard"/> 难度，就会产生此异常。
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperiencePointPerPuzzle(DifficultyLevel difficultyLevel)
	{
		var @base = difficultyLevel switch
		{
			DifficultyLevel.Easy => 16,
			DifficultyLevel.Moderate => 20,
			DifficultyLevel.Hard => 28,
			_ when Enum.IsDefined(difficultyLevel) => throw new NotSupportedException("Other kinds of difficulty levels are not supported."),
			_ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel))
		};

		return @base * ScoreHandler.GetWeekendFactor();
	}

	/// <summary>
	/// 获得的经验值。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <param name="cardLevel">用户的卡片级别。</param>
	/// <returns>经验值。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperiencePoint(int @base, int cardLevel)
		=> (int)Round(@base * ScoreHandler.GetGlobalRate(cardLevel)) * ScoreHandler.GetWeekendFactor();

	/// <summary>
	/// 获得的金币。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <param name="cardLevel">用户的卡片级别。</param>
	/// <returns>金币。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoin(int @base, int cardLevel) => (int)Round(@base * ScoreHandler.GetGlobalRate(cardLevel)) * ScoreHandler.GetWeekendFactor();

	/// <summary>
	/// 获得的物品。
	/// </summary>
	/// <returns>获得的物品。可能为 <see langword="null"/>。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Item? GetEarnedItem()
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
