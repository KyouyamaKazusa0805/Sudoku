namespace Sudoku.Workflow.Bot.Oicq.RootCommands.Periodic;

/// <summary>
/// 每日一题模块。
/// </summary>
[PeriodicCommand(12)]
[SupportedOSPlatform(OperatingSystemNames.Windows)]
internal sealed class DailyPuzzleCommand : PeriodicCommand
{
	/// <inheritdoc/>
	public override async Task ExecuteAsync()
	{
		var solver = Solver with { IgnoreSlowAlgorithms = true };

		for (var trial = 0; trial < 100; trial++)
		{
			// 出题。
			var grid = Generator.Generate();
			if (Solver.Analyze(grid) is not
				{
					IsSolved: true,
					DifficultyLevel: var diffLevel and (DifficultyLevel.Easy or DifficultyLevel.Moderate or DifficultyLevel.Hard),
					MaxDifficulty: var diff and >= 2.3M and <= 4.5M,
					Steps: var steps
				})
			{
				continue;
			}

			// 根据题目难度确定是否满足题目发送的条件。
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

			await MessageManager.SendGroupMessageAsync(
				SudokuGroupNumber,
				"""
				【每日一题】
				这是给本数独群提供的一个特殊机制。每一天都会抽取一道题目给各位完成。
				难度系数均不会大于 4.5（唯一矩形的难度），且可使用一般直观技巧和一些进阶的直观技巧完成此题目，无需标记候选数。
				"""
			);

			// 创建图片并发送出去。
			var diffString = diffLevel switch { DifficultyLevel.Easy => "容易", DifficultyLevel.Moderate => "一般", DifficultyLevel.Hard => "困难" };
			await sendPictureAsync(SudokuGroupNumber, grid.ToString(), $"#{DateTime.Today:yyyyMMdd} 难度级别：{diffString}，难度系数：{diff:0.0}");

			// 这是在循环里。这里我们要退出指令，因为已经发送了一个题目。
			return;


			[SupportedOSPlatform(OperatingSystemNames.Windows)]
			async Task sendPictureAsync(string groupId, string grid, string footerText)
			{
				var picturePath = StorageHandler.GenerateCachedPicturePath(
					() => ISudokuPainter.Create(1000)
						.WithGridCode(grid)
						.WithRenderingCandidates(false)
						.WithFooterText(footerText)
				)!;

				await MessageManager.SendGroupMessageAsync(groupId, new ImageMessage { Path = picturePath });

				File.Delete(picturePath);
			}
		}
	}
}
