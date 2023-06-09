namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示一个分析指令。
/// </summary>
[Command("分析")]
[RequiredUserLevel(5)]
internal sealed class AnalyzeCommand : Command
{
	/// <summary>
	/// 错误信息，提示用户程序存在分析 bug。
	/// </summary>
	private const string ErrorMessageHeader =
		"""
		抱歉，由于程序算法设计的错误导致的 bug，导致这个题无法正确分析，得到正确结果。
		请将该错误步骤截图或图文信息反馈给程序设计者，并修复此 bug。
		""";

	/// <summary>
	/// 表示你要查询的魔塔的层数。支持 1 到 130。默认数值为 -1。
	/// </summary>
	[DoubleArgument("层数")]
	[Hint("表示你要查询的魔塔的层数。支持 1 到 130。默认数值为 -1。")]
	[DisplayingIndex(1)]
	[ArgumentDisplayer("1-130")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue<int>(-1)]
	public int TowerStage { get; set; }

	/// <summary>
	/// 表示你需要查询的技巧名称或它的英文名。该参数支持技巧的中文名、英文简称等写法。
	/// </summary>
	[DoubleArgument("技巧")]
	[Hint("表示你需要查询的技巧名称或它的英文名。该参数支持技巧的中文名、英文简称等写法。")]
	[DisplayingIndex(2)]
	public string? TechniqueName { get; set; }

	/// <summary>
	/// 表示你需要分析的题目是引用自哪个地方。目前暂时支持“每日一题”、“魔塔”和“题目”。
	/// </summary>
	[DoubleArgument("类型")]
	[Hint("表示你需要分析的题目是引用自哪个地方。目前暂时支持“每日一题”、“魔塔”和“题目”。")]
	[DisplayingIndex(0)]
	public string? Type { get; set; }

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
		switch (this)
		{
			case { Type: null or Types.Tower, TowerStage: var stage and (-1 or >= 1 and <= 130) }:
			{
				var senderId = messageReceiver.Sender.Id;
				await AnalyzePuzzleCoreAsync(
					TowerOfSorcererOperations.GetPuzzleFor(
						(stage, UserOperations.Read(senderId)) switch { (-1, { TowerOfSorcerer: var s }) => s, _ => stage - 1 }
					),
					messageReceiver
				);
				break;
			}
			case { Type: null or Types.Text, Puzzle.IsUndefined: false }:
			{
				await AnalyzePuzzleCoreAsync(Puzzle, messageReceiver);
				break;
			}
			case { Type: { } type }:
			{
				await (
					type switch
					{
						Types.DailyPuzzle => DailyPuzzleOperations.ReadDailyPuzzleAnswer() switch
						{
							{ Puzzle: var p } => AnalyzePuzzleCoreAsync(p, messageReceiver),
							_ => messageReceiver.SendMessageAsync("抱歉，当天没有每日一题。")
						},
						Types.Text => messageReceiver.SendMessageAsync("抱歉，参数为“题目”，但实际没有传入“题目”参数。"),
						Types.Tower => messageReceiver.SendMessageAsync("抱歉，参数为“魔塔”，但是尚未指定“层数”参数。"),
						_ => messageReceiver.SendMessageAsync("抱歉，输入的参数“类型”不合法。")
					}
				);
				break;
			}
			default:
			{
				await messageReceiver.SendMessageAsync("参数状态不合法（比如输入的题目参数是无解或多解题，等等），请检查参数的录入。");
				break;
			}
		}
	}

	/// <summary>
	/// 分析指定的 <paramref name="grid"/> 参数的题目。
	/// </summary>
	/// <param name="grid">等待分析的题目。</param>
	/// <param name="messageReceiver">发送信息的机器人消息接收器对象。</param>
	private async Task AnalyzePuzzleCoreAsync(Grid grid, GroupMessageReceiver messageReceiver)
	{
		switch (PuzzleAnalyzer.Analyze(grid))
		{
			case { IsSolved: true } analyzerResult:
			{
				switch (TechniqueName)
				{
					case null:
					{
						await messageReceiver.SendMessageAsync(analyzerResult.ToString(AnalyzerResultFormattingOptions.ShowElapsedTime));
						break;
					}
					default:
					{
						if (analyzerResult[TechniqueName] is not var (g, s))
						{
							await messageReceiver.SendMessageAsync(
								$"""
								名为“{TechniqueName}”的技巧步骤在本题里尚未找到。没有找到的原因可能是如下的这些：
									* 输入的技巧名称（或它的别名）有误
									* 输入的技巧确实在这个题目里没有
									* 该技巧存在于其他不在按次序解题的主线步骤序列上
								"""
							);
							break;
						}

						await messageReceiver.SendMessageAsync(s.ToString());
						await Task.Delay(1000);

						await messageReceiver.SendPictureThenDeleteAsync(
							ISudokuPainter.Create(1000)
								.WithGrid(g)
								.WithRenderingCandidates(true)
								.WithStep(s)
								.WithPreferenceSettings(static pref => pref.CandidateScale = .4M)
						);

						break;
					}
				}
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
					{ErrorMessageHeader}
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
					{ErrorMessageHeader}
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
					{ErrorMessageHeader}
					---
					内部错误代码：{failedReason}（{failedReason.GetFailedReasonText()}）
					"""
				);

				break;
			}
		}
	}
}

/// <summary>
/// 为属性 <see cref="AnalyzeCommand.Type"/> 提供数据。
/// </summary>
/// <seealso cref="AnalyzeCommand.Type"/>
file static class Types
{
	/// <summary>
	/// 表示分析每日一题的数据。
	/// </summary>
	public const string DailyPuzzle = "每日一题";

	/// <summary>
	/// 表示分析固定的文本题目。
	/// </summary>
	public const string Text = "题目";

	/// <summary>
	/// 表示分析的是魔塔的题目。
	/// </summary>
	public const string Tower = "魔塔";
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
