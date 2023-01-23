namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Define a start gaming command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class StartGamingCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("MatchStart")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var separator = new string(' ', 4);

		var context = RunningContexts[e.GroupId];
		context.ExecutingCommand = CommandName;
		context.AnsweringContext = new();

		var (puzzle, chosenCells, finalCellIndex, timeLimit, baseExp) = generatePuzzle();

		await e.SendMessageAsync(string.Format(R.MessageFormat("MatchReady")!, timeLimit.ToChineseTimeSpanString()));
		await Task.Delay(5.Seconds());

		var selectedNodes = chosenCells.Select(markerNodeSelector).ToArray();

		// Create picture and send message.
		await e.SendPictureThenDeleteAsync(
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
					await e.SendMessageAsync(R.MessageFormat("GamingIsCancelled")!);

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
							await e.SendMessageAsync(R.MessageFormat("WrongAnswer")!);
							answeringContext.AnsweredUsers.TryAdd(userId, 1);

							break;
						}
						case (false, true):
						{
							// Wrong answer but containing records.
							await e.SendMessageAsync(R.MessageFormat("WrongAnswer")!);
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
								let currentUser = e.Sender.Group.GetMemberFromQQAsync(currentUserId).Result
								select (currentUser.Name, Id: currentUserId, Score: deduct + (currentUserId == userId ? baseExp : 0));

							if (!scoringTableLines.Any(e => e.Id == userId))
							{
								scoringTableLines = scoringTableLines.Prepend((userName, userId, baseExp));
							}

							// Correct answer and first reply.
							await e.SendMessageAsync(
								string.Format(
									R.MessageFormat("CorrectAnswer")!,
									$"{userName} ({userId})",
									baseExp,
									TimeSpan.FromSeconds(elapsedTime).ToChineseTimeSpanString(),
									scoringTableLines.Any()
										? string.Join(
											Environment.NewLine,
											from triplet in scoringTableLines
											let finalScoreToDisplay = Scorer.GetEarnedScoringDisplayingString(triplet.Score)
											select $"{triplet.Name} ({triplet.Id}){separator}{finalScoreToDisplay}"
										)
										: R["None"]!
								)
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
			let currentUser = e.Sender.Group.GetMemberFromQQAsync(currentUserId).Result
			select (currentUser.Name, Id: currentUserId, Score: deduct);

		appendOrDeduceScore(scoringTableLinesDeductOnly);

		await e.SendMessageAsync(
			string.Format(
				R.MessageFormat("GameTimeUp")!,
				scoringTableLinesDeductOnly.Any()
					? string.Join(
						Environment.NewLine,
						from triplet in scoringTableLinesDeductOnly
						let finalScoreToDisplay = Scorer.GetEarnedScoringDisplayingString(triplet.Score)
						select $"{triplet.Name} ({triplet.Id}){separator}{finalScoreToDisplay}"
					)
					: R["None"]!
			)
		);

		// Create picture and send message.
		await e.SendPictureThenDeleteAsync(
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
		return true;


		static void appendOrDeduceScore(IEnumerable<(string, string Id, int Score)> scoringTableLines)
		{
			foreach (var (_, id, score) in scoringTableLines)
			{
				var userData = InternalReadWrite.Read(id, new() { QQ = id, LastCheckIn = DateTime.MinValue });
				userData.Score += score;

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
						var timeLimit = ICommandDataProvider.GetGamingTimeLimit(l);

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
			{ Minutes: 0 } => $"{@this:s' \u79d2'}",
			{ Seconds: 0 } => $"{@this:m' \u5206\u949f'}",
			_ => $"{@this:m' \u5206 'ss' \u79d2'}",
		};
}
