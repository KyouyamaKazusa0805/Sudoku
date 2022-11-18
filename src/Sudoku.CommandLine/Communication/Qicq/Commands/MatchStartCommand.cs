namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Define a match start command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
internal sealed class MatchStartCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_MatchStart"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		EnvironmentCommandExecuting = CommandName;

		await e.SendMessageAsync(R["_MessageFormat_MatchReady"]!);
		await Task.Delay(10 * 1000);

		var (puzzle, (resultCell, resultDigit), baseExp) = GeneratePuzzle();

		// Create picture and send message.
		if (!await CreatePictureAndSendAsync(puzzle, resultCell, e))
		{
			return true;
		}

		// Start gaming.
		for (var timeLastSeconds = 300; timeLastSeconds > 0; timeLastSeconds--)
		{
			for (var internalTimes = 0; internalTimes < 4; internalTimes++)
			{
				await Task.Delay(245); // Reserve 5 milliseconds for executing the following slow steps.

				foreach (var data in AnswerData)
				{
					if (data is not { Conclusion: var answeredDigit and not -1, User: var userId })
					{
						continue;
					}

					switch (answeredDigit == resultDigit, AnsweredUsers.Contains(userId))
					{
						case (false, false):
						{
							// Wrong answer and no records.
							await e.SendMessageAsync(R["_MessageFormat_WrongAnswer"]!);
							AnsweredUsers.Add(userId);

							break;
						}
						case (_, true):
						{
							// The user has already answered the result.
							await e.SendMessageAsync(
								new AtMessage(userId)
									+ new PlainMessage(" ")
									+ new PlainMessage(R["_MessageFormat_NoChanceToAnswer"]!)
							);

							break;
						}
						case (true, false):
						{
							// Correct answer and first reply.
							await e.SendMessageAsync(string.Format(R["_MessageFormat_CorrectAnswer"]!, $"{e.Sender.Name} ({userId})", baseExp));

							var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
							if (!Directory.Exists(folder))
							{
								// Error. The computer does not contain "My Documents" folder.
								// This folder is special; if the computer does not contain the folder, we should return directly.
								return true;
							}

							var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
							if (!Directory.Exists(botDataFolder))
							{
								Directory.CreateDirectory(botDataFolder);
							}

							var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
							if (!Directory.Exists(botUsersDataFolder))
							{
								Directory.CreateDirectory(botUsersDataFolder);
							}

							var fileName = $"""{botUsersDataFolder}\{userId}.json""";
							var userData = File.Exists(fileName)
								? Deserialize<UserData>(await File.ReadAllTextAsync(fileName))!
								: new() { QQ = userId, ComboCheckedIn = 0, LastCheckIn = DateTime.MinValue, Score = 0 };

							userData.Score += baseExp;

							await File.WriteAllTextAsync(fileName, Serialize(userData));

							goto DefaultReturning;
						}
					}
				}

				AnswerData.Clear();
			}
		}

		await e.SendMessageAsync(R["_MessageFormat_GameTimeUp"]!);

	DefaultReturning:
		AnsweredUsers.Clear();
		AnswerData.Clear();
		EnvironmentCommandExecuting = null;
		return true;
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
			switch (Solver.Solve(grid))
			{
				// If the puzzle is easy, we will get the 20th step, treating it as the real game puzzle.
				case { DifficultyLevel: DifficultyLevel.Easy, Steps: { Length: > 20 } steps }
				when steps[20] is { Conclusions: [{ Cell: var targetCell, Digit: var targetDigit }] }:
				{
					return (grid, (targetCell, targetDigit), 12);
				}

				// If the puzzle is moderate, we will get the first single step whose previous step is moderate.
				case { DifficultyLevel: DifficultyLevel.Moderate, Steps: var steps }:
				{
					for (var index = 1; index <= steps.Length; index++)
					{
#pragma warning disable format
						if ((steps[index], steps[index - 1]) is (
							{ TechniqueGroup: TechniqueGroup.Single, Conclusions: [{ Cell: var targetCell, Digit: var targetDigit }] },
							{ DifficultyLevel: DifficultyLevel.Moderate }
						))
#pragma warning restore format
						{
							return (grid, (targetCell, targetDigit), 18);
						}
					}

					break;
				}

				// Otherwise, the puzzle is too hard to be solved as a game puzzle.
				default:
				{
					continue;
				}
			}
		}
	}

	/// <summary>
	/// Creates a picture and send it as message.
	/// </summary>
	/// <param name="puzzle">The target grid.</param>
	/// <param name="resultCell">The result cell.</param>
	/// <param name="e">The group message receiver.</param>
	/// <returns>The task that hold the details of the current operation.</returns>
	private async Task<bool> CreatePictureAndSendAsync(Grid puzzle, int resultCell, GroupMessageReceiver e)
	{
		var painter = ISudokuPainter.Create(1000)
			.WithGrid(puzzle)
			.WithRenderingCandidates(false)
			.WithNodes(new[] { new CellViewNode(DisplayColorKind.Normal, resultCell) });

		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			// This folder is special; if the computer does not contain the folder, we should return directly.
			return false;
		}

		var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var cachedPictureFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
		if (!Directory.Exists(cachedPictureFolder))
		{
			Directory.CreateDirectory(cachedPictureFolder);
		}

		var picturePath = $"""{cachedPictureFolder}\temp.png""";
		painter.SaveTo(picturePath);

		await e.SendMessageAsync(new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
		return true;
	}
}
