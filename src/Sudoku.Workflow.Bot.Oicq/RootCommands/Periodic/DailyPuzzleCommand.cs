namespace Sudoku.Workflow.Bot.Oicq.RootCommands.Periodic;

/// <summary>
/// 每日一题模块。
/// </summary>
[PeriodicCommand(12)]
internal sealed class DailyPuzzleCommand : PeriodicCommand
{
	/// <inheritdoc/>
	public override async Task ExecuteAsync()
	{
		var analyzer = PuzzleAnalyzer.WithAlgorithmLimits(true, true);
		for (var trial = 0; trial < 100; trial++)
		{
			// 出题。
			var grid = Generator.Generate();
			if (PuzzleAnalyzer.Analyze(grid) is not
				{
					IsSolved: true,
					DifficultyLevel: var diffLevel and <= DifficultyLevel.Hard and not 0,
					MaxDifficulty: var diff and >= 2.3M and <= 4.5M,
					Solution: var solution
				} analyzerResult)
			{
				continue;
			}

			// 根据题目难度确定是否满足题目发送的条件。
			switch (diffLevel)
			{
				case DifficultyLevel.Easy when analyzerResult[2.3M]!.Length <= 2:
				case DifficultyLevel.Moderate:
				case DifficultyLevel.Hard
				when (
					from step in analyzerResult[DifficultyLevel.Hard]!
					where step is WingStep or SingleDigitPatternStep or UniqueRectangleStep or AlmostLockedCandidatesStep
					select step
				).Take(2).Count() > 2:
				{
					continue;
				}
			}

			await MessageManager.SendGroupMessageAsync(
				SudokuGroupNumber,
				"""
				【每日一题】
				这是给本数独群提供的一个特殊机制。每一天都会抽取一道题目给各位完成。
				难度系数均不会大于 4.5（唯一矩形的难度），且可使用一般直观技巧和一些进阶的直观技巧完成此题目，无需标记候选数。
				---
				每日一题功能可回答，默认要求回答的是题目的最后一行的 9 个数字（并从左往右书写）。回答正确则可获得奖励。
				每日一题提交答案请使用“！每日一题 答案 <答案>”指令。答案的 9 个数字无需任何符号隔开。
				"""
			);

			// 创建图片并发送出去。
			var diffString = diffLevel switch { DifficultyLevel.Easy => "容易", DifficultyLevel.Moderate => "一般", DifficultyLevel.Hard => "困难" };
			await sendPictureAsync(SudokuGroupNumber, grid.ToString(), $"#{DateTime.Today:yyyyMMdd} 难度级别：{diffString}，难度系数：{diff:0.0}");

			// 保存答案到本地。
			DailyPuzzleOperations.WriteDailyPuzzleAnswer(solution);

			// 这是在循环里。这里我们要退出指令，因为已经发送了一个题目。
			return;
		}


		static async Task sendPictureAsync(string groupId, string grid, string footerText)
		{
			var picturePath = DrawingOperations.GenerateCachedPicturePath(
				ISudokuPainter.Create(1000)
					.WithGridCode(grid)
					.WithRenderingCandidates(false)
					.WithFooterText(footerText)
			)!;

			await MessageManager.SendGroupMessageAsync(groupId, new ImageMessage { Path = picturePath });

			File.Delete(picturePath);
		}
	}
}
