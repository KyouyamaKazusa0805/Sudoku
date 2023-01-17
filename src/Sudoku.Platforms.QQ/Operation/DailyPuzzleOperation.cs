namespace Sudoku.Platforms.QQ.Operation;

/// <summary>
/// Defines a daily puzzle generating operation.
/// </summary>
[SupportedOSPlatform("windows")]
file sealed record DailyPuzzleOperation() : PeriodicOperation(new TimeOnly(12, 0))
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <inheritdoc/>
	public override async Task ExecuteAsync()
	{
		var solver = Solver with { IgnoreSlowAlgorithms = true };

		var groupId = R["SudokuGroupQQ"]!;
		for (var trial = 0; trial < 100; trial++)
		{
			var grid = Generator.Generate();
			if (Solver.Solve(grid) is not
				{
					IsSolved: true,
					DifficultyLevel: var diffLevel and (DifficultyLevel.Easy or DifficultyLevel.Moderate or DifficultyLevel.Hard),
					MaxDifficulty: var diff and >= 2.3M and <= 4.5M,
					Steps: var steps
				})
			{
				continue;
			}

			switch (diffLevel)
			{
				case DifficultyLevel.Easy when steps.Count(easyPredicate) <= 2:
				case DifficultyLevel.Moderate:
				case DifficultyLevel.Hard when steps.Count(hardPredicate) > 2:
				{
					continue;
				}


				static bool easyPredicate(IStep step) => step.Difficulty == 2.3M;

				static bool hardPredicate(IStep step)
					=> step is
					{
						DifficultyLevel: DifficultyLevel.Hard,
						TechniqueGroup: not (
							TechniqueGroup.Wing or TechniqueGroup.SingleDigitPattern
							or TechniqueGroup.UniqueRectangle
							or TechniqueGroup.AlmostLockedCandidates
						)
					};
			}

			await MessageManager.SendGroupMessageAsync(groupId, R.MessageFormat("DailyPuzzle")!);

			// Create picture and send message.
			await SendPictureAsync(
				groupId,
				grid.ToString(),
				$"#{DateTime.Today:yyyyMMdd} {R["DifficultyLevelIs"]!}{diffLevel switch
				{
					DifficultyLevel.Easy => R["DiffLevelEasy"]!,
					DifficultyLevel.Moderate => R["DiffLevelModerate"]!,
					DifficultyLevel.Hard => R["DiffLevelHard"]!,
				}}{R["_Token_Comma"]!}{R["DiffRatingIs"]!} {diff:0.0}"
			);

			// Exit the command if any one time the command has been executed successfully.
			return;
		}

		//await MessageManager.SendGroupMessageAsync(groupId, R.MessageFormat("DailyPuzzleGeneratingFailed")!);
	}

	/// <summary>
	/// Sends a picture using the specified text and grid code.
	/// </summary>
	/// <param name="groupId">The group QQ number.</param>
	/// <param name="grid">The grid code.</param>
	/// <param name="footerText">The footer text.</param>
	/// <returns>The result.</returns>
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
