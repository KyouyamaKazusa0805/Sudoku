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

#if !DEBUG
		await e.SendMessageAsync(R["_MessageFormat_MatchReady"]!);
#endif
		await e.SendMessageAsync(R["_MessageFormat_PuzzleIsGenerating"]!);
#if !DEBUG
		await Task.Delay(15000);
#endif

		var (puzzle, resultCell, resultDigit, baseExp) = GeneratePuzzle();

		// Create picture and send message.
		await e.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle)
				.WithRenderingCandidates(false)
				.WithNodes(new[] { new CellViewNode(DisplayColorKind.Normal, resultCell) })
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
					if (data is not { Conclusion: var answeredDigit and not -1, User: { Id: var userId, Name: var userName } })
					{
						continue;
					}

					switch (answeredDigit == resultDigit, answeringContext.AnsweredUsers.Contains(userId))
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


		static GeneratedGridData GeneratePuzzle()
		{
			while (true)
			{
				var grid = Generator.Generate();
				if (Solver.Solve(grid) is
					{
						DifficultyLevel: var diffLevel and (DifficultyLevel.Easy or DifficultyLevel.Moderate),
						Steps: [.., { Conclusions: [{ Cell: var targetCell, Digit: var targetDigit }] }] steps
					})
				{
					return new(grid, targetCell, targetDigit, diffLevel switch { DifficultyLevel.Easy => 12, DifficultyLevel.Moderate => 18 });
				}
			}
		}
	}
}

/// <summary>
/// The generated grid data.
/// </summary>
file readonly record struct GeneratedGridData(scoped in Grid Puzzle, int SolutionCell, int SolutionDigit, int BaseExperiencePointCanBeEarned);
