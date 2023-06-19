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
	/// 表示你需要操作的类型。目前支持的操作有“整题”和“单步”。默认为“整题”。
	/// </summary>
	[DoubleArgument("操作")]
	[Hint("表示你需要操作的类型。目前支持的操作有“整题”和“单步”。默认为“整题”。")]
	[DisplayingIndex(3)]
	[DefaultValue<string>(Operations.Full)]
	public string? Operation { get; set; }

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
		=> await (this switch
		{
			{ Type: Types.Tower, TowerStage: var stage and (-1 or >= 1 and <= 130) }
			when UserOperations.Read(messageReceiver.Sender.Id) is var user
				=> AnalyzePuzzleCoreAsync(
					TowerOfSorcererOperations.GetPuzzleFor((stage, user) switch { (-1, { TowerOfSorcerer: var s }) => s, _ => stage - 1 }),
					Operation,
					messageReceiver
				),
			{ Type: null or Types.Text, Puzzle.IsUndefined: false } => AnalyzePuzzleCoreAsync(Puzzle, Operation, messageReceiver),
			{ Type: Types.DailyPuzzle } => DailyPuzzleOperations.ReadDailyPuzzleAnswer() switch
			{
				{ Puzzle: var p } => AnalyzePuzzleCoreAsync(p, Operation, messageReceiver),
				_ => messageReceiver.SendMessageAsync("当天没有每日一题。")
			},
			_ => messageReceiver.SendMessageAsync("参数状态不合法（比如输入的题目参数是无解或多解题，等等），请检查参数的录入。")
		});

	/// <summary>
	/// 分析指定的 <paramref name="grid"/> 参数的题目。
	/// </summary>
	/// <param name="grid">等待分析的题目。</param>
	/// <param name="operation">表示分析操作。</param>
	/// <param name="messageReceiver">发送信息的机器人消息接收器对象。</param>
	private async Task AnalyzePuzzleCoreAsync(Grid grid, string? operation, GroupMessageReceiver messageReceiver)
	{
		switch (operation ?? Operations.Full)
		{
			case Operations.Full:
			{
				switch (PuzzleAnalyzer.Analyze(grid))
				{
					case { IsSolved: true } analyzerResult:
					{
						switch (TechniqueName)
						{
							case null:
							{
								var analyzerResultStr = analyzerResult.ToString(AnalyzerResultFormattingOptions.ShowElapsedTime);
								await messageReceiver.SendMessageAsync(analyzerResultStr);
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
				break;
			}
			case Operations.SingleStep:
			{
				switch (PuzzleStepCollector.Search(grid)?.ToArray() ?? Array.Empty<Step>())
				{
					case []:
					{
						await messageReceiver.SendMessageAsync("当前题目尚不存在同级别的步骤。");
						break;
					}
					case var steps:
					{
						var grouped =
							from step in steps
							group step by step.Code into stepGroup
							orderby stepGroup.Key
							select (Code: stepGroup.Key, Steps: stepGroup.ToArray());

						if (TechniqueName is null)
						{
							await messageReceiver.SendMessageAsync(
								$"""
								当前盘面存在如下一些技巧：
								---
								{string.Join(Environment.NewLine, from @group in grouped select $"  * {@group.Code.GetName()} * {group.Steps.Length}")}
								"""
							);
						}
						else
						{
							var foundStepGroups = (
								from stepGroup in grouped
								where rawNameMatcher(stepGroup, TechniqueName) && stepGroup.Steps.Length != 0
								select stepGroup
							).ToArray();
							await messageReceiver.SendMessageAsync(
								foundStepGroups switch
								{
									[] => $"""
									当前盘面不存在名为“{TechniqueName}”的技巧。原因主要可能为：
									---
									  * 确实不存在该技巧
									  * 输入的技巧名（如技巧本身的中文名、简写、缩写、大小写字母等信息）不正确
									""",
									[(_, { Length: var count and not 0 })] => $"当前盘面存在一共 {count} 个步骤。建议你使用筛选表达式确定展示具体哪一个步骤。",
									_ => $"""
									当前盘面存在如下一些技巧：
									---
									{string.Join(Environment.NewLine, from @group in foundStepGroups select $"  * {@group.Code.GetName()} * {group.Steps.Length}")}
									"""
								}
							);
						}
						break;
					}
				}
				break;
			}
			default:
			{
				await messageReceiver.SendMessageAsync("参数“操作”只能是“单步”和“整题”。请检查输入。");
				break;
			}
		}


		static bool rawNameMatcher((Technique Code, Step[] Steps) stepGroup, string techniqueName)
		{
			var code = stepGroup.Code;
			if (nameEquality(code.GetName()))
			{
				return true;
			}

			var aliases = code.GetAliases();
			if (aliases is not null && Array.Exists(aliases, nameEquality))
			{
				return true;
			}

			var abbr = code.GetAbbreviation();
			if (abbr is not null && nameEquality(abbr))
			{
				return true;
			}

			return false;


			bool nameEquality(string name) => name == techniqueName || name.Contains(techniqueName, StringComparison.OrdinalIgnoreCase);
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

/// <summary>
/// 为属性 <see cref="AnalyzeCommand.Operation"/> 提供数据。
/// </summary>
/// <seealso cref="AnalyzeCommand.Operation"/>
file static class Operations
{
	/// <summary>
	/// 表示分析整个题目的所有步骤。
	/// </summary>
	public const string Full = "整题";

	/// <summary>
	/// 表示分析当前盘面下的所有步骤。
	/// </summary>
	public const string SingleStep = "单步";
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
