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
		for (var trial = 0; trial < 100; trial++)
		{
			// 出题。
			var grid = Generator.Generate();
			if (PuzzleAnalyzer.Analyze(grid) is not
				{
					IsSolved: true,
					DifficultyLevel: var diffLevel and < DifficultyLevel.Nightmare and not 0,
					MaxDifficulty: var diff and >= 3.4M,
					Solution: var solution
				} analyzerResult)
			{
				continue;
			}

			await MessageManager.SendGroupMessageAsync(
				SudokuGroupNumber,
				"""
				【每日一题】
				这是给本数独群提供的一个特殊机制。每一天都会抽取一道题目给各位完成。每天的题目难度分为容易、一般、困难和极难四种。
				---
				每日一题功能可回答，默认要求回答的是题目的最后一行的 9 个数字（并从左往右书写）。回答正确则可获得奖励。
				每日一题提交答案请使用“！每日一题 答案 <答案>”指令。答案的 9 个数字无需任何符号隔开。
				"""
			);

			// 创建图片并发送出去。
			await sendPictureAsync(
				SudokuGroupNumber,
				grid.ToString(),
				$"#{DateTime.Today:yyyyMMdd} 难度级别：{diffLevel switch
				{
					DifficultyLevel.Easy => "容易",
					DifficultyLevel.Moderate => "一般",
					DifficultyLevel.Hard => "困难",
					DifficultyLevel.Fiendish => "极难"
				}}，难度系数：{diff:0.0}",
				diffLevel
			);

			// 保存答案到本地。
			DailyPuzzleOperations.WriteDailyPuzzleAnswer(solution);

			// 这是在循环里。这里我们要退出指令，因为已经发送了一个题目。
			return;
		}

		await MessageManager.SendGroupMessageAsync(SudokuGroupNumber, "抱歉，题目生成失败。如果看到该消息，请联系程序设计者以修复此错误。");


		static async Task sendPictureAsync(string groupId, string grid, string footerText, DifficultyLevel diffLevel)
		{
			var picturePath = DrawingOperations.GenerateCachedPicturePath(
				ISudokuPainter.Create(1000)
					.WithGridCode(grid)
					.WithRenderingCandidates(diffLevel >= DifficultyLevel.Hard)
					.WithFooterText(footerText)
					.WithFontScale(1M, .4M)
			)!;

			await MessageManager.SendGroupMessageAsync(groupId, new ImageMessage { Path = picturePath });

			File.Delete(picturePath);
		}
	}
}
