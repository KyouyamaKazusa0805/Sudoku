namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示解析的指令。
/// </summary>
public sealed class AnalysisCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => "解析";

	/// <inheritdoc/>
	public override string HelpCommandString => "/解析 题目代码串";


	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		if (message.GetPlainArguments() is var str && Grid.TryParse(str, out var grid))
		{
			var analysisResult = AnalyzerPool.Analyzer.Analyze(in grid);
			var resultString = analysisResult.ToString();
			await api.SendGroupMessageAsync(message, resultString);
		}
		else
		{
			await api.SendGroupMessageAsync(message, DefaultInfoString);
		}
	}
}
