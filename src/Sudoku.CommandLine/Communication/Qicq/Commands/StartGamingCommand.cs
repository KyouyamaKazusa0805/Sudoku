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
#if false
		EnvironmentCommandExecuting = CommandName;
		AnsweringContexts.TryAdd(e.GroupId, new(new(), new()));

#if DEBUG
		await Task.Delay(5000);
#else
		await e.SendMessageAsync(R["_MessageFormat_MatchReady"]!);
		await Task.Delay(10 * 1000);
#endif

		var (puzzle, (resultCell, resultDigit), baseExp) = GeneratePuzzle();

		// Create picture and send message.
		await e.SendPictureThenDeleteAsync(
			() => ISudokuPainter.Create(1000)
				.WithGrid(puzzle)
				.WithRenderingCandidates(false)
				.WithNodes(new[] { new CellViewNode(DisplayColorKind.Normal, resultCell) })
		);

		var context = AnsweringContexts[e.GroupId];

		// Start gaming.
		for (var timeLastSeconds = 300; timeLastSeconds > 0; timeLastSeconds--)
		{
			for (var internalTimes = 0; internalTimes < 4; internalTimes++)
			{
				await Task.Delay(245); // Reserve 5 milliseconds for executing the following slow steps.

				foreach (var data in context.CurrentRoundAnsweredValues)
				{
					if (data is not { Conclusion: var answeredDigit and not -1, User: { Id: var userId, Name: var userName } })
					{
						continue;
					}

					switch (answeredDigit == resultDigit, context.AnsweredUsers.Contains(userId))
					{
						case (false, false):
						{
							// Wrong answer and no records.
							await e.SendMessageAsync(R["_MessageFormat_WrongAnswer"]!);
							context.AnsweredUsers.Add(userId);

							break;
						}
						case (_, true):
						{
							// The user has already answered the result.
							await e.SendMessageAsync(new AtMessage(userId) + new PlainMessage(" ") + new PlainMessage(R["_MessageFormat_NoChanceToAnswer"]!));

							break;
						}
						case (true, false):
						{
							// Correct answer and first reply.
							await e.SendMessageAsync(string.Format(R["_MessageFormat_CorrectAnswer"]!, $"{userName} ({userId})", baseExp));

							var userData = InternalReadWrite.Read(userId, new() { QQ = userId, LastCheckIn = DateTime.MinValue });
							userData.Score += baseExp;

							InternalReadWrite.Write(userData);

							goto DefaultReturning;
						}
					}
				}

				context.CurrentRoundAnsweredValues.Clear();
			}
		}

		await e.SendMessageAsync(R["_MessageFormat_GameTimeUp"]!);

	DefaultReturning:
		EnvironmentCommandExecuting = null;
		AnsweringContexts.TryRemove(e.GroupId, out _);
		return true;
#else
#if true
		await e.SendMessageAsync(R["_MessageFormat_CurrentCommandIsUnderConstruction"]!);
		return true;
#else
		return false;
#endif
#endif
	}

	/// <summary>
	/// Generates a random puzzle.
	/// </summary>
	/// <returns>The random puzzle.</returns>
	private (Grid Puzzle, (int Cell, int Digit) Solution, int BaseExperiencePointCanBeEarned) GeneratePuzzle()
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
				return (
					grid,
					(targetCell, targetDigit),
					diffLevel switch { DifficultyLevel.Easy => 12, DifficultyLevel.Moderate => 18 }
				);
			}
		}
	}
}
