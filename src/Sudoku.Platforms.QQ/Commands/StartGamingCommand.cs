namespace Sudoku.Platforms.QQ.Commands;

using static Constants;

/// <summary>
/// Define a start gaming command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class StartGamingCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_MatchStart"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var separator = new string(' ', 4);

		var context = RunningContexts[e.GroupId];
		context.ExecutingCommand = CommandName;
		context.AnsweringContext = new();

		var targetCells = DateTime.Today switch
		{
			{ DayOfWeek: DayOfWeek.Saturday or DayOfWeek.Sunday } => Rng.Next(1, 100) switch
			{
				<= 25 => TwoCells,
				> 25 and <= 50 => ThreeCells,
				> 50 and <= 75 => FiveCells,
				_ => SevenCells
			},
			_ => Rng.Next(1, 99) switch { <= 33 => TwoCells, > 33 and <= 66 => ThreeCells, _ => FiveCells }
		};

		var (puzzle, solutionData, timeLimit, baseExp) = generatePuzzle(targetCells);

		await e.SendMessageAsync(string.Format(R["_MessageFormat_MatchReady"]!, timeLimit.ToChineseTimeSpanString()));
		await Task.Delay(10.Seconds());

		// Create picture and send message.
		await e.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle)
				.WithRenderingCandidates(false)
				.WithNodes(solutionData.Select(markerNodeSelector))
		);

		var answeringContext = context.AnsweringContext;

		// Start gaming.
		for (int timeLastSeconds = (int)timeLimit.TotalSeconds, elapsedTime = 0; timeLastSeconds > 0; timeLastSeconds--, elapsedTime++)
		{
			for (var internalTimes = 0; internalTimes < 4; internalTimes++)
			{
				await Task.Delay(245); // Reserve 5 milliseconds for executing the following slow steps.

				if (context.AnsweringContext.IsCancelled is true)
				{
					// User cancelled.
					await e.SendMessageAsync(R["_MessageFormat_GamingIsCancelled"]!);

					goto ReturnTrueAndInitializeContext;
				}

				foreach (var data in answeringContext.CurrentRoundAnsweredValues)
				{
					if (data is not { Conclusions: var answeredDigits, User: { Id: var userId, Name: var userName } })
					{
						continue;
					}

					switch (resultCorrector(answeredDigits, solutionData), answeringContext.AnsweredUsers.ContainsKey(userId))
					{
						case (false, false):
						{
							// Wrong answer and no records.
							await e.SendMessageAsync(R["_MessageFormat_WrongAnswer"]!);
							answeringContext.AnsweredUsers.TryAdd(userId, 1);

							break;
						}
						case (false, true):
						{
							// Wrong answer but containing records.
							await e.SendMessageAsync(R["_MessageFormat_WrongAnswer"]!);
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
									R["_MessageFormat_CorrectAnswer"]!,
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
				R["_MessageFormat_GameTimeUp"]!,
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

		static bool resultCorrector(int[]? answeredDigits, (int, int Digit)[] data)
		{
			if (answeredDigits is null || answeredDigits.Length != data.Length)
			{
				return false;
			}

			for (var i = 0; i < data.Length; i++)
			{
				if (answeredDigits[i] != data[i].Digit)
				{
					return false;
				}
			}

			return true;
		}

		static GeneratedGridData generatePuzzle(params int[] targetCells)
		{
			while (true)
			{
				var grid = Generator.Generate();
				switch (Solver.Solve(grid))
				{
					case { DifficultyLevel: var l and (DifficultyLevel.Easy or DifficultyLevel.Moderate), Steps: { Length: var length } s }
					when length > targetCells.Max() && new List<(int Cell, int Digit)>() is var collection:
					{
						foreach (var targetCell in targetCells)
						{
							if (s[targetCell].Conclusions is [{ Cell: var c, Digit: var d }])
							{
								collection.Add((c, d));
							}
						}

						if (collection.Count != targetCells.Length)
						{
							// Invalid case - This is a potential bug :P
							continue;
						}

						var expEarned = Scorer.GetScoreEarnedInEachGaming(targetCells, l);
						var timeLimit = ICommandDataProvider.GetGamingTimeLimit(targetCells, l);
						return new(grid, collection.ToArray(), timeLimit, expEarned);
					}
				}
			}
		}

		static ViewNode markerNodeSelector((int Cell, int Digit) element, int i)
			=> new BabaGroupViewNode(DisplayColorKind.Normal, element.Cell, (Utf8Char)(char)(i + '1'), (short)(1 << element.Digit));
	}
}

/// <summary>
/// The generated grid data.
/// </summary>
/// <param name="Puzzle">Indicates the puzzle.</param>
/// <param name="SolutionData">Indicates all solution data that you should answer.</param>
/// <param name="TimeLimit">The whole time limit of a single gaming.</param>
/// <param name="ExperiencePoint">Indicates how many experience point you can earn in the current round if answered correctly.</param>
file readonly record struct GeneratedGridData(scoped in Grid Puzzle, (int Cell, int Digit)[] SolutionData, TimeSpan TimeLimit, int ExperiencePoint);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="constant"]'/>
file static class Constants
{
	/// <summary>
	/// Indicates the cell indices that the puzzle will be extracted.
	/// </summary>
	public static readonly int[]
		TwoCells = { 10, 20 },
		ThreeCells = { 10, 15, 20 },
		FiveCells = { 10, 15, 20, 25, 30 },
		SevenCells = { 1, 3, 6, 10, 15, 21, 28 };
}

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
