namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示一个分析指令。
/// </summary>
[Command("分析")]
internal sealed class AnalyzeCommand : Command
{
	/// <summary>
	/// 表示你输入的题目代码。代码支持各种数独可解析的文本形式，包括但不限于 Sudoku Explainer、Hodoku、Excel 的数独的输入格式。
	/// 如果文字带有空格，由于空格本身对于程序解析命令参数有意义，所以你需要将你的代码用双引号引起来，来告知机器人该文本代码包含空格。
	/// </summary>
	[DoubleArgument("题目")]
	[Hint("表示你输入的题目代码。代码支持各种数独可解析的文本形式，包括但不限于 Sudoku Explainer、Hodoku、Excel 的数独的输入格式。如果文字带有空格，由于空格本身对于程序解析命令参数有意义，所以你需要将你的代码用双引号引起来，来告知机器人该文本代码包含空格。")]
	[ValueConverter<GridConverter>]
	public Grid Puzzle { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
		=> await messageReceiver.SendMessageAsync(
			Puzzle switch
			{
				{ IsUndefined: true } => "参数“题目”缺失。请给出必需的参数“题目”作为分析的盘面信息。",
				_ => PuzzleAnalyzer.Analyze(Puzzle) switch
				{
					{ IsSolved: true } analyzerResult => analyzerResult.ToString(AnalyzerResultFormattingOptions.ShowElapsedTime),
					_ => "抱歉，题目多解或无解，无法解析。请检查文本代码是否有问题，或询问题目发布者本人是否有错误。"
				}
			}
		);
}
