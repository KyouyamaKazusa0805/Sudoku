namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示一个分析指令。
/// </summary>
[Command("分析")]
[RequiredUserLevel(20)]
internal sealed class AnalyzeCommand : Command
{
	/// <summary>
	/// 表示你输入的题目代码。代码支持各种数独可解析的文本形式，包括但不限于 Sudoku Explainer、Hodoku、Excel 的数独的输入格式。
	/// 如果文字带有空格，由于空格本身对于程序解析命令参数有意义，所以你需要将你的代码用双引号引起来，来告知机器人该文本代码包含空格。
	/// </summary>
	[DoubleArgument("题目")]
	[Hint("表示你输入的题目代码。代码支持各种数独可解析的文本形式，包括但不限于 Sudoku Explainer、Hodoku、Excel 的数独的输入格式。如果文字带有空格，由于空格本身对于程序解析命令参数有意义，所以你需要将你的代码用双引号引起来，来告知机器人该文本代码包含空格。")]
	[ValueConverter<GridConverter>]
	[DisplayingIndex(0)]
	public Grid Puzzle { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (Puzzle.IsUndefined)
		{
			await messageReceiver.SendMessageAsync("参数“题目”缺失。请给出必需的参数“题目”作为分析的盘面信息。");
			return;
		}

		const string errorMessageHeader =
			"""
			抱歉，由于程序算法设计的错误导致的 bug，导致这个题无法正确分析，得到正确结果。
			请将该错误步骤截图或图文信息反馈给程序设计者，并修复此 bug。
			""";
		switch (PuzzleAnalyzer.Analyze(Puzzle))
		{
			case { IsSolved: true } analyzerResult:
			{
				await messageReceiver.SendMessageAsync(analyzerResult.ToString(AnalyzerResultFormattingOptions.ShowElapsedTime));
				break;
			}
			case
			{
				FailedReason: AnalyzerFailedReason.WrongStep,
				UnhandledException: WrongStepException { WrongStep: var step, CurrentInvalidGrid: var currentGrid }
			}:
			{
				await messageReceiver.SendMessageAsync(
					$"""
					{errorMessageHeader}
					---
					错误盘面代码：{currentGrid:#}
					错误步骤描述：{step}
					---
					错误步骤对应图（如果该步骤包含多个视图，则只生成第一个视图）：
					"""
				);
				await Task.Delay(1000);

				await messageReceiver.SendPictureThenDeleteAsync(
					ISudokuPainter.Create(1000)
						.WithGrid(currentGrid)
						.WithRenderingCandidates(true)
						.WithStep(step)
						.WithPreferenceSettings(static pref => pref.CandidateScale = .4M)
				);

				break;
			}
			case { FailedReason: AnalyzerFailedReason.ExceptionThrown, UnhandledException: { Message: var message } exception }:
			{
				await messageReceiver.SendMessageAsync(
					$"""
					{errorMessageHeader}
					---
					错误信息：{message}
					异常类型：{exception.GetType()}
					"""
				);

				break;
			}
			case { FailedReason: var failedReason }:
			{
				await messageReceiver.SendMessageAsync(
					$"""
					{errorMessageHeader}
					---
					内部错误代码：{failedReason}（{failedReason.GetFailedReasonText()}）
					"""
				);

				break;
			}
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 获取 <see cref="AnalyzerFailedReason"/> 对应的错误文字。
	/// </summary>
	/// <param name="this"><see cref="AnalyzerFailedReason"/> 实例。</param>
	/// <returns>字符串写法。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetFailedReasonText(this AnalyzerFailedReason @this)
		=> @this switch
		{
			AnalyzerFailedReason.Nothing => "没有错误",
			AnalyzerFailedReason.PuzzleIsInvalid => "题目无解或多解",
			AnalyzerFailedReason.PuzzleHasNoSolution => "题目无解",
			AnalyzerFailedReason.PuzzleHasMultipleSolutions => "题目多解",
			AnalyzerFailedReason.UserCancelled => "用户取消了分析",
			AnalyzerFailedReason.NotImplemented => "当前步骤搜索实例尚未实现",
			AnalyzerFailedReason.ExceptionThrown => "有异常抛出",
			AnalyzerFailedReason.WrongStep => "分析出现错误步骤",
			AnalyzerFailedReason.PuzzleIsTooHard => "题目超过了分析算法可计算的难度"
		};
}
