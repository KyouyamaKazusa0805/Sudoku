namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Define a start gaming command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
internal sealed class StartGamingCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_MatchStart"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var context = RunningContexts[e.GroupId];
		context.ExecutingCommand = CommandName;
		context.AnsweringContext = new();

		await e.SendMessageAsync(R["_MessageFormat_MatchReady"]!);
		await Task.Delay(20.Seconds());

		var (puzzle, solutionData, baseExp) = GeneratePuzzle(10, 15, 20);

		// Create picture and send message.
		await e.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle)
				.WithRenderingCandidates(false)
				.WithNodes(solutionData.Select(markerNodeSelector))
		);

		var answeringContext = context.AnsweringContext;

		// Start gaming.
		for (var timeLastSeconds = 300; timeLastSeconds > 0; timeLastSeconds--)
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

					switch (resultCorrector(answeredDigits, solutionData), answeringContext.AnsweredUsers.Contains(userId))
					{
						case (false, false):
						{
							// Wrong answer and no records.
							await e.SendMessageAsync(R["_MessageFormat_WrongAnswer"]!);
							answeringContext.AnsweredUsers.Add(userId);

							break;
						}
						case (_, true):
						{
							// The user has already answered the result.
							await e.SendMessageAsync($"{new AtMessage(userId)}{" "}{R["_MessageFormat_NoChanceToAnswer"]!}");

							break;
						}
						case (true, false):
						{
							// Correct answer and first reply.
							await e.SendMessageAsync(string.Format(R["_MessageFormat_CorrectAnswer"]!, $"{userName} ({userId})", baseExp));

							var userData = InternalReadWrite.Read(userId, new() { QQ = userId, LastCheckIn = DateTime.MinValue });
							userData.Score += baseExp;

							InternalReadWrite.Write(userData);

							goto ReturnTrueAndInitializeContext;
						}
					}
				}

				answeringContext.CurrentRoundAnsweredValues.Clear();
			}
		}

		await e.SendMessageAsync(R["_MessageFormat_GameTimeUp"]!);

	ReturnTrueAndInitializeContext:
		context.ExecutingCommand = null;
		return true;


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

		static GeneratedGridData GeneratePuzzle(params int[] targetCells)
		{
			while (true)
			{
				var grid = Generator.Generate();
				var result = Solver.Solve(grid);
				if (result is { DifficultyLevel: var l and (DifficultyLevel.Easy or DifficultyLevel.Moderate), Steps: { Length: var length } s }
					&& length > targetCells.Max()
					&& new List<(int Cell, int Digit)>() is var collection)
				{
					foreach (var targetCell in targetCells)
					{
						if (s[targetCell].Conclusions is [{ Cell: var c, Digit: var d }])
						{
							collection.Add((c, d));
						}
					}

					return new(grid, collection.ToArray(), ICommandDataProvider.GetEachGamingExperiencePointCanBeEarned(targetCells, l));
				}
			}
		}

		static ViewNode markerNodeSelector((int Cell, int Digit) element, int i)
			=> new UnknownViewNode(DisplayColorKind.Normal, element.Cell, (Utf8Char)(char)(i + '1'), (short)(1 << element.Digit));
	}
}

/// <summary>
/// The generated grid data.
/// </summary>
/// <param name="Puzzle">Indicates the puzzle.</param>
/// <param name="SolutionData">Indicates all solution data that you should answer.</param>
/// <param name="BaseExperiencePointCanBeEarned">
/// Indicates how many experience point you can earn in the currently round if you've answered correctly.
/// </param>
file readonly record struct GeneratedGridData(scoped in Grid Puzzle, (int Cell, int Digit)[] SolutionData, int BaseExperiencePointCanBeEarned);
