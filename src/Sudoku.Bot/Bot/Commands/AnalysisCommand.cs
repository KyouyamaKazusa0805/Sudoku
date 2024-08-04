namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示解析的指令。
/// </summary>
[Command("解析")]
[CommandUsage("解析 <题目字符串>", IsSyntax = true)]
public sealed class AnalysisCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		if (message.GetPlainArguments() is var str && Grid.TryParse(str, out var grid))
		{
			var analysisResult = AnalyzerPool.Analyzer.Analyze(in grid);
			var resultString = analysisResult.ToString(
				AnalysisResultFormattingOptions.ShowDifficulty
					| AnalysisResultFormattingOptions.ShowGridAndSolutionCode
					| AnalysisResultFormattingOptions.ShowElapsedTime,
				null
			);
			await api.SendGroupMessageAsync(message, resultString);
		}
		else
		{
			await api.SendGroupMessageAsync(message, DefaultInfoString);
		}
	}
}
