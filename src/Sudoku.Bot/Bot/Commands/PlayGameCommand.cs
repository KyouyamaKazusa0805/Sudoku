namespace Sudoku.Bot.Commands;

using GeneratedGridData = (Grid Puzzle, Cell[] ChosenCells, Digit ChosenDigit, TimeSpan LimitedTimeSpan, int ExperienceValueCanEarn);
using ResultInfo = (string Id, int ExperiencePoint, int Coin, int Times, bool IsCorrectedUser);

/// <summary>
/// 表示开始游戏的指令。
/// </summary>
[Command("PK", IsDebugging = true)]
[CommandDescription("和朋友开始一局游戏。")]
[CommandUsage("PK <模式>", IsSyntax = true)]
[CommandUsage("PK 找编号")]
public sealed class PlayGameCommand : Command
{
	/// <summary>
	/// 用变量记录游戏游玩过程之中产生的回复状态。默认情况为 1。每次消息回应后，这个数值都会增大一个单位。
	/// </summary>
	private int _answerId = 1;


	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		var args = message.GetPlainArguments();
		switch (Program.GameModes.FirstOrDefault(f => f.GetModeName() == args))
		{
			case GameMode.NineMatch:
			{
				_answerId = 1;
				await NineMatchAsync(api, message);
				break;
			}
			default:
			{
				await api.SendGroupMessageAsync(message, HelpUsageString);
				break;
			}
		}
	}

	private async Task NineMatchAsync(ChatMessageApi api, ChatMessage message)
	{
		var separator = new string(' ', 4);
		var rng = Random.Shared;
		var context = new BotRunningContext
		{
			ExecutingCommand = CommandName,
			AnsweringContext = new(),
			Tag = GameMode.NineMatch
		};
		Program.RunningContexts.TryAdd(message.GroupOpenId, context);

		// 确定题目。
		var (puzzle, chosenCells, finalCellIndex, timeLimit, baseExp) = generatePuzzle();

		await api.SendGroupMessageAsync(
			message,
			$"""
			题目生成中，请稍候……
			游戏规则：机器人将给出一道标准数独题，你需要在 {timeLimit.ToChineseTimeSpanString()}内完成它，并找出 9 个位置里唯一一个编号和填数一致的单元格。回答结果（该编号）时请艾特机器人，格式为“@机器人 数字”；若所有格子的编号都和当前格子的填数不同，请用数字“0”表示。回答正确将自动结束游戏并获得相应积分，错误则会扣除一定积分；题目可反复作答，但连续错误会导致连续扣分哦！若无人作答完成题目，将自动作废，游戏自动结束。
			""",
			_answerId++
		);

		// 将题目里的对象给标注到绘图对象上去。
		var selectedNodes = chosenCells.Select(markerNodeSelector).ToArray();

		var outputPictureFileName = GenerateOutputPictureFileName();
		using var pictureCanvas = new GridCanvas(1000, 10);
		pictureCanvas.Settings.ShowCandidates = false;
		pictureCanvas.Settings.BabaGroupingCharacterColor = Color.FromArgb(96, Color.Red);
		pictureCanvas.Clear();
		pictureCanvas.DrawGrid(in puzzle);
		pictureCanvas.DrawBabaGroupingViewNodes(selectedNodes);
		pictureCanvas.SavePictureTo(outputPictureFileName);

		// 根据绘图对象直接创建图片，然后发送出去。
		var (pictureLink, _) = await CloudObjectStorage.UploadFileAsync(outputPictureFileName);
		await api.SendGroupImageAsync(message, pictureLink, _answerId++);

		var answeringContext = context.AnsweringContext;
		answeringContext.CurrentTimesliceAnswers.Clear();

		var userId = message.Author.MemberOpenId;

		// 使用反复的循环来等待用户回答。
		for (
			var (timeLastSeconds, elapsedTime) = ((int)timeLimit.TotalSeconds, 0);
			timeLastSeconds > 0;
			timeLastSeconds--, elapsedTime++
		)
		{
			// 将 1 秒钟拆分为 4 个 250 毫秒为单位的循环单位，即按每 250 毫秒一次判断用户数据是否回答正确。
			for (var internalTimes = 0; internalTimes < 4; internalTimes++)
			{
				await Task.Delay(250);

				// 判别是否用户在取消指令里取消了游戏。
				if (context.AnsweringContext.IsCancelled is true)
				{
					// 用户取消了游戏。
					await api.SendGroupMessageAsync(message, "游戏已取消。", _answerId++);

					goto ReturnTrueAndInitializeContext;
				}

				// 判别用户的回答是否正确。
				// 这里我们用到的是 'AnsweringContext.CurrentTimesliceAnswers' 属性。
				foreach (NineMatchUserAnswer data in answeringContext.CurrentTimesliceAnswers)
				{
					var answeredCellIndex = data.IsValidAnswer(out var r) ? (Cell)r : -1;
					switch (answeredCellIndex - 1 == finalCellIndex, answeringContext.AnsweredUsers.ContainsKey(userId))
					{
						case (false, var userHasAnswered):
						{
							await api.SendGroupMessageAsync(message, "回答错误~", _answerId++);

							if (!userHasAnswered)
							{
								answeringContext.AnsweredUsers.TryAdd(userId, 1);
							}
							else
							{
								answeringContext.AnsweredUsers[userId]++;
							}
							break;
						}
						case (true, _):
						{
							var scoringTableLines =
								from pair in answeringContext.AnsweredUsers
								let currentUserId = pair.Key
								let times = pair.Value
								let deduct = -Enumerable.Range(1, times).Sum(ScoreCalculator.GetExperiencePointDeduct)
								let isCorrectedUser = currentUserId == userId
								select (ResultInfo)(
									currentUserId,
									deduct + (isCorrectedUser ? baseExp : 0),
									isCorrectedUser ? NineMatchGameScoreCalculator.GetCoinOriginal(rng) : 0,
									times,
									isCorrectedUser
								);

							if (!scoringTableLines.Any(e => e.Id == userId))
							{
								scoringTableLines = scoringTableLines.Prepend(
									(
										userId,
										baseExp,
										NineMatchGameScoreCalculator.GetCoinOriginal(rng),
										1,
										true
									)
								);
							}

							// 回答正确。为正确结果进行答复，并记录经验值和金币数。
							var elapsedTimeString = TimeSpan.FromSeconds(elapsedTime).ToChineseTimeSpanString();
							var currentUserData = UserData.Read(userId);
							await api.SendGroupMessageAsync(
								message,
								$"""
								恭喜玩家“{currentUserData.VirtualNickname}”回答正确！耗时 {elapsedTimeString}。游戏结束！
								---
								{getMessage(scoringTableLines)}
								""",
								_answerId++
							);

							appendOrDeduceScore(scoringTableLines);

							goto ReturnTrueAndInitializeContext;
						}
					}
				}

				answeringContext.CurrentTimesliceAnswers.Clear();
			}
		}

		// 执行扣分。
		// 执行到这里说明没有人回答正确题目。
		var scoringTableLinesDeductOnly =
			from pair in answeringContext.AnsweredUsers
			let currentUserId = pair.Key
			let times = pair.Value
			let deduct = -Enumerable.Range(1, times).Sum(ScoreCalculator.GetExperiencePointDeduct)
			select (ResultInfo)(currentUserId, deduct, 0, times, false);

		appendOrDeduceScore(scoringTableLinesDeductOnly);

		await api.SendGroupMessageAsync(
			message,
			$"""
			游戏时间到~ 游戏结束~
			---
			{getMessage(scoringTableLinesDeductOnly)}
			""",
			_answerId++
		);

		outputPictureFileName = GenerateOutputPictureFileName();
		pictureCanvas.Clear();
		pictureCanvas.DrawGrid(puzzle.GetSolutionGrid());
		if (finalCellIndex != -1)
		{
			pictureCanvas.DrawCellViewNodes([new(WellKnownColorIdentifierKind.Normal, chosenCells[finalCellIndex])]);
		}

		pictureCanvas.DrawBabaGroupingViewNodes(selectedNodes);
		pictureCanvas.SavePictureTo(outputPictureFileName);

		// 发送答案。
		(pictureLink, _) = await CloudObjectStorage.UploadFileAsync(outputPictureFileName);
		await api.SendGroupImageAsync(message, pictureLink, _answerId++);

	ReturnTrueAndInitializeContext:
		Program.RunningContexts.TryRemove(KeyValuePair.Create(message.GroupOpenId, context));


		static void appendOrDeduceScore(IEnumerable<ResultInfo> scoringTableLines)
		{
			foreach (var (id, exp, coin, _, isCorrectedUser) in scoringTableLines)
			{
				var userData = UserData.Read(id);
				userData.ExperienceValue += NineMatchGameScoreCalculator.GetExperiencePoint(exp);
				userData.CoinValue += NineMatchGameScoreCalculator.GetCoin(coin);

				if (isCorrectedUser && !userData.GameModeWonData.TryAdd(GameMode.NineMatch, 1))
				{
					userData.GameModeWonData[GameMode.NineMatch]++;
				}
				if (!userData.GameModeTriedData.TryAdd(GameMode.NineMatch, 1))
				{
					userData.GameModeTriedData[GameMode.NineMatch]++;
				}

				UserData.Write(userData);
			}
		}

		string getMessage(IEnumerable<ResultInfo> values)
			=> values.Any()
				? string.Join(
					Environment.NewLine,
					from tuple in values
					let userName = UserData.Read(tuple.Id).VirtualNickname
					let expEarned = NineMatchGameScoreCalculator.GetExperiencePoint(tuple.ExperiencePoint)
					let coinEarned = NineMatchGameScoreCalculator.GetCoin(tuple.Coin)
					select $"{userName}{separator}{expEarned} 经验，{coinEarned} 金币"
				)
				: "无";

		GeneratedGridData generatePuzzle()
		{
			var finalCellsChosen = (stackalloc Cell[9]);
			var generator = Program.Generator;
			while (true)
			{
				var grid = generator.Generate();
				switch (Program.Analyzer.Analyze(in grid))
				{
					case { IsSolved: true, Solution: var solution, DifficultyLevel: var l and <= DifficultyLevel.Hard }:
					{
						var candsMap = grid.CandidatesMap;
						var emptyCells = grid.EmptyCells;

						var expEarned = NineMatchGameScoreCalculator.GetExperiencePointPerPuzzle(l);
						var timeLimit = GetGameTimeLimit(l);

						finalCellsChosen.Clear();

						var (chosenCells, puzzleIsInvalid) = (CellMap.Empty, false);
						for (var digit = 0; digit < 9; digit++)
						{
							var tempMap = emptyCells & candsMap[digit];
							if (!tempMap)
							{
								// 本题目不包含任何空格包含这个数。
								puzzleIsInvalid = true;
								goto CheckPuzzleValidity;
							}

							var copiedMap = tempMap;
							foreach (var tempCell in tempMap)
							{
								if (solution.GetDigit(tempCell) == digit || chosenCells.Contains(tempCell))
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

							var cell = copiedMap[rng.Next(0, copiedMap.Count)];
							chosenCells.Add(cell);
							finalCellsChosen[digit] = cell;
						}

					CheckPuzzleValidity:
						if (puzzleIsInvalid)
						{
							continue;
						}

						if (rng.Next(0, 1000) < 666)
						{
							// 随机确定一个数字，然后去取盘面里解等于这个数字的格子，还得是格子尚未录入的状态。
							// 将格子作为新结果替换掉原来不同的单元格索引。
							var digitChosen = rng.Next(0, 9);
							foreach (var cell in emptyCells & candsMap[digitChosen])
							{
								if (solution.GetDigit(cell) == digitChosen && !chosenCells.Contains(cell))
								{
									finalCellsChosen[digitChosen] = cell;
									break;
								}
							}
							return (grid, finalCellsChosen.ToArray(), digitChosen, timeLimit, expEarned);
						}

						// 不用改，直接返回。
						return (grid, finalCellsChosen.ToArray(), -1, timeLimit, expEarned);
					}
				}
			}
		}


		static BabaGroupViewNode markerNodeSelector(Cell cell, int i) => new(cell, (char)(i + '1'), 511);
	}


	/// <summary>
	/// 随机产生一个图片的路径地址。
	/// </summary>
	/// <returns>图片的临时路径。</returns>
	private static string GenerateOutputPictureFileName()
	{
		var f = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
		return $@"{Program.BotCachePath}\{f}.png";
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
			DifficultyLevel.Easy => TimeSpan.FromMinutes(5),
			DifficultyLevel.Moderate => new(0, 5, 30),
			DifficultyLevel.Hard => TimeSpan.FromMinutes(6),
			_ => throw new NotSupportedException("当前难度尚不支持作为题目出题。")
		};
}

/// <summary>
/// 对 <see cref="GameMode.NineMatch"/> 进行计分处理。
/// </summary>
/// <seealso cref="GameMode.NineMatch"/>
file static class NineMatchGameScoreCalculator
{
	/// <summary>
	/// 获得经验值的基础数值。
	/// </summary>
	/// <param name="difficultyLevel">题目难度。</param>
	/// <returns>经验值。</returns>
	/// <exception cref="NotSupportedException">如果 <paramref name="difficultyLevel"/> 未定义，就会产生此异常。</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 如果 <paramref name="difficultyLevel"/> 超过了 <see cref="DifficultyLevel.Hard"/> 难度，就会产生此异常。
	/// </exception>
	public static int GetExperiencePointPerPuzzle(DifficultyLevel difficultyLevel)
	{
		var @base = difficultyLevel switch
		{
			DifficultyLevel.Easy => 16,
			DifficultyLevel.Moderate => 20,
			DifficultyLevel.Hard => 28,
			_ when Enum.IsDefined(difficultyLevel) => throw new NotSupportedException("当前难度级别尚不支持作为题目出题。"),
			_ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel))
		};
		return @base * (ScoreCalculator.TodayIsWeekend() ? 2 : 1);
	}

	/// <summary>
	/// 获得的经验值。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <param name="cardLevel">用户的卡片级别。</param>
	/// <returns>经验值。</returns>
	public static int GetExperiencePoint(int @base)
		=> (int)Round((decimal)@base) * (ScoreCalculator.TodayIsWeekend() ? 2 : 1);

	/// <summary>
	/// 获得金币获得的基础数值。
	/// </summary>
	/// <param name="rng">随机数生成器对象。</param>
	/// <returns>金币数。</returns>
	public static int GetCoinOriginal(Random rng) => 48 + rng.Next(-6, 7);

	/// <summary>
	/// 获得的金币。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <param name="cardLevel">用户的卡片级别。</param>
	/// <returns>金币。</returns>
	public static int GetCoin(int @base)
		=> (int)Round((decimal)@base) * (ScoreCalculator.TodayIsWeekend() ? 2 : 1);
}
