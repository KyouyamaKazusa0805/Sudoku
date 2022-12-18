namespace Sudoku.Platforms.QQ.Scheduler;

/// <summary>
/// Defines a daily puzzle generating operation.
/// </summary>
[SupportedOSPlatform("windows")]
file sealed class DailyPuzzleOperation : PeriodicOperation
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();

	/// <summary>
	/// Defines a default puzzle solver.
	/// </summary>
	private static readonly LogicalSolver Solver = new();


	/// <inheritdoc/>
	public override TimeOnly TriggeringTime => new(12, 0);


	/// <inheritdoc/>
	public override async Task ExecuteAsync()
	{
		var groupId = R["SudokuGroupQQ"]!;
		for (var trial = 0; trial < 100; trial++)
		{
			var grid = Generator.Generate();
			if (Solver.Solve(grid) is not
				{
					IsSolved: true,
					DifficultyLevel: var diffLevel and (DifficultyLevel.Easy or DifficultyLevel.Moderate or DifficultyLevel.Hard),
					MaxDifficulty: var diff and <= 4.5M
				})
			{
				continue;
			}

			await MessageManager.SendGroupMessageAsync(groupId, R["_MessageFormat_DailyPuzzle"]!);
			await Task.Delay(10.Seconds());

			// Create picture and send message.
			await SendPictureAsync(
				groupId,
				grid.ToString(),
				$"#{DateTime.Today:yyyyMMdd} {R["DifficultyLevelIs"]!}{diffLevel switch
				{
					DifficultyLevel.Easy => R["DiffLevelEasy"]!,
					DifficultyLevel.Moderate => R["DiffLevelModerate"]!,
					DifficultyLevel.Hard => R["DiffLevelHard"]!,
				}}{R["_Token_Comma"]!}{R["DiffRatingIs"]!}{diff:0.0}"
			);

			// Exit the command if any one time the command has been executed successfully.
			return;
		}

		//await MessageManager.SendGroupMessageAsync(groupId, R["_MessageFormat_DailyPuzzleGeneratingFailed"]!);
	}

	private async Task SendPictureAsync(string groupId, string grid, string footerText)
	{
		var picturePath = InternalReadWrite.GenerateCachedPicturePath(
			() => ISudokuPainter.Create(1000)
				.WithGridCode(grid)
				.WithRenderingCandidates(false)
				.WithFooterText(footerText)
		)!;

		await MessageManager.SendGroupMessageAsync(groupId, new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
	}
}
