namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltInModule]
[SupportedOSPlatform("windows")]
file sealed class StartGamingModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "开始游戏";


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		var separator = new string(' ', 4);

		var context = RunningContexts[messageReceiver.GroupId];
		context.ExecutingCommand = RaisingCommand;
		context.AnsweringContext = new();

		var (puzzle, chosenCells, finalCellIndex, timeLimit, baseExp) = generatePuzzle();

		await messageReceiver.SendMessageAsync(
			$"""
			题目生成中，请稍候……
			游戏规则：机器人将给出一道标准数独题，你需要在 {timeLimit.ToChineseTimeSpanString()}内完成它，并找出 9 个位置里唯一一个编号和填数一致的单元格。回答结果（该编号）时请艾特机器人，格式为“@机器人 数字”；若所有格子的编号都和当前格子的填数不同，请用数字“0”表示。回答正确将自动结束游戏并获得相应积分，错误则会扣除一定积分；题目可反复作答，但连续错误会导致连续扣分哦！若无人作答完成题目，将自动作废，游戏自动结束。
			"""
		);
		await Task.Delay(2.Seconds());

		var selectedNodes = chosenCells.Select(markerNodeSelector).ToArray();

		// Create picture and send message.
		await messageReceiver.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle)
				.WithRenderingCandidates(false)
				.WithNodes(selectedNodes)
				.WithPreferenceSettings(static pref => pref.UnknownIdentifierColor = Color.FromArgb(96, Color.Red))
		);

		var answeringContext = context.AnsweringContext;

		answeringContext.CurrentRoundAnsweredValues.Clear();

		// Start gaming.
		for (int timeLastSeconds = (int)timeLimit.TotalSeconds, elapsedTime = 0; timeLastSeconds > 0; timeLastSeconds--, elapsedTime++)
		{
			for (var internalTimes = 0; internalTimes < 4; internalTimes++)
			{
				await Task.Delay(245); // Reserve 5 milliseconds for executing the following slow steps.

				if (context.AnsweringContext.IsCancelled is true)
				{
					// User cancelled.
					await messageReceiver.SendMessageAsync("游戏已中断。");

					goto ReturnTrueAndInitializeContext;
				}

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
							// Wrong answer and no records.
							await messageReceiver.SendMessageAsync("回答错误~");
							answeringContext.AnsweredUsers.TryAdd(userId, 1);

							break;
						}
						case (false, true):
						{
							// Wrong answer but containing records.
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
								let deduct = -Enumerable.Range(1, times).Sum(Scorer.GetDeduct)
								let currentUser = messageReceiver.Sender.Group.GetMatchedMemberViaIdAsync(currentUserId).Result
								select (currentUser.Name, Id: currentUserId, Score: deduct + (currentUserId == userId ? baseExp : 0), Times: times);

							if (!scoringTableLines.Any(e => e.Id == userId))
							{
								scoringTableLines = scoringTableLines.Prepend((userName, userId, baseExp, 1));
							}

							// Correct answer and first reply.
							var elapsedTimeString = TimeSpan.FromSeconds(elapsedTime).ToChineseTimeSpanString();
							await messageReceiver.SendMessageAsync(
								$"""
								回答正确~ 恭喜玩家 {userName}（{userId}） 获得 {baseExp} 积分！耗时 {elapsedTimeString}。游戏结束！
								---
								得分情况：
								{(
									scoringTableLines.Any()
										? string.Join(
											Environment.NewLine,
											from triplet in scoringTableLines
											let finalScoreToDisplay = Scorer.GetEarnedScoringDisplayingString(triplet.Score)
											select $"{triplet.Name}（{triplet.Id}）{separator}{finalScoreToDisplay}"
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

		// Deduce scores if worth.
		var scoringTableLinesDeductOnly =
			from pair in answeringContext.AnsweredUsers
			let currentUserId = pair.Key
			let times = pair.Value
			let deduct = -Enumerable.Range(1, times).Sum(Scorer.GetDeduct)
			let currentUser = messageReceiver.Sender.Group.GetMatchedMemberViaIdAsync(currentUserId).Result
			select (currentUser.Name, Id: currentUserId, Score: deduct, Times: times);

		appendOrDeduceScore(scoringTableLinesDeductOnly);

		await messageReceiver.SendMessageAsync(
			$"""
			游戏时间到~ 游戏结束~
			---
			得分：
			{(
				scoringTableLinesDeductOnly.Any()
					? string.Join(
						Environment.NewLine,
						from triplet in scoringTableLinesDeductOnly
						let finalScoreToDisplay = Scorer.GetEarnedScoringDisplayingString(triplet.Score)
						select $"{triplet.Name}（{triplet.Id}）{separator}{finalScoreToDisplay}"
					)
					: "无"
			)}
			"""
		);

		// Create picture and send message.
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


		static void appendOrDeduceScore(IEnumerable<(string, string Id, int Score, int Times)> scoringTableLines)
		{
			foreach (var (_, id, score, times) in scoringTableLines)
			{
				var userData = InternalReadWrite.Read(id, new() { QQ = id, LastCheckIn = DateTime.MinValue });
				userData.ExperiencePoint += score;
				if (!userData.CorrectedCount.TryAdd(GamingMode.FindDifference, 1))
				{
					userData.CorrectedCount[GamingMode.FindDifference] += 1;
				}

				if (!userData.TriedCount.TryAdd(GamingMode.FindDifference, times))
				{
					userData.TriedCount[GamingMode.FindDifference] += times;
				}

				if (!userData.TotalPlayingCount.TryAdd(GamingMode.FindDifference, 1))
				{
					userData.TotalPlayingCount[GamingMode.FindDifference]++;
				}

				InternalReadWrite.Write(userData);
			}
		}

		static GeneratedGridData generatePuzzle()
		{
			var finalCellsChosen = (stackalloc int[9]);
			while (true)
			{
				var grid = Generator.Generate();
				switch (Solver.Solve(grid))
				{
					case { IsSolved: true, Solution: var solution, DifficultyLevel: var l and <= DifficultyLevel.Hard }:
					{
						var expEarned = Scorer.GetScoreEarnedInEachGaming(l);
						var timeLimit = GetGamingTimeLimit(l);

						finalCellsChosen.Clear();
						var chosenCells = CellMap.Empty;
						var emptyCells = grid.EmptyCells;
						var puzzleIsInvalid = false;
						for (var digit = 0; digit < 9; digit++)
						{
							var tempMap = emptyCells & grid.CandidatesMap[digit];
							if (!tempMap)
							{
								// The current grid does not contain any possible empty cells holding this candidate.
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
								// The current grid does not contain any possible empty cells that can be chosen.
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
	/// Gets the time limit for a single gaming.
	/// </summary>
	/// <param name="difficultyLevel">The difficulty level of the puzzle.</param>
	/// <returns>The time limit.</returns>
	/// <exception cref="NotSupportedException">Throws when the specified argument value is not supported.</exception>
	private static TimeSpan GetGamingTimeLimit(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => 5.Minutes(),
			DifficultyLevel.Moderate => new TimeSpan(0, 5, 30),
			DifficultyLevel.Hard => 6.Minutes(),
			_ => throw new NotSupportedException("The specified difficulty is not supported.")
		};
}

/// <summary>
/// The generated grid data.
/// </summary>
/// <param name="Puzzle">Indicates the puzzle.</param>
/// <param name="ChosenCells">Indicates the chosen 9 cells.</param>
/// <param name="FinalIndex">Indicates the final index.</param>
/// <param name="TimeLimit">The whole time limit of a single gaming.</param>
/// <param name="ExperiencePoint">Indicates how many experience point you can earn in the current round if answered correctly.</param>
file readonly record struct GeneratedGridData(scoped in Grid Puzzle, int[] ChosenCells, int FinalIndex, TimeSpan TimeLimit, int ExperiencePoint);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Converts the current <see cref="TimeSpan"/> instance into a Chinese text string that represents the current instance.
	/// </summary>
	/// <param name="this">The <see cref="TimeSpan"/> instance.</param>
	/// <returns>The target string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToChineseTimeSpanString(this TimeSpan @this)
		=> @this switch
		{
			{ Minutes: 0 } => $"{@this:s' 秒'}",
			{ Seconds: 0 } => $"{@this:m' 分钟'}",
			_ => $"{@this:m' 分 'ss' 秒'}",
		};
}
