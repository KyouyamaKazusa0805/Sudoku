namespace Sudoku.Workflow.Bot.Oicq.RootCommands.Generalized;

/// <summary>
/// 艾特指令。该指令较为特殊，它不带有前缀，也不是人触发的条件。
/// 它主要是为了去处理一些其他指令里的回复情况，如开始游戏后，回答问题需要艾特的时候，就会用到这个指令来处理。
/// </summary>
[Command]
file sealed class MentioningCommand : IModule
{
	/// <inheritdoc/>
	bool? IModule.IsEnable { get; set; } = true;


	/// <inheritdoc/>
	public async void Execute(MessageReceiverBase @base)
	{
		if (@base is not GroupMessageReceiver
			{
				GroupId: var groupId,
				Sender: var sender,
				MessageChain: [SourceMessage, AtMessage { Target: BotNumber }, PlainMessage { Text: var m }]
			} messageReceiver)
		{
			return;
		}

		if (!RunningContexts.TryGetValue(groupId, out var context))
		{
			return;
		}

		// 去掉头尾的空格，然后将字符串按命令行的模式进行解析。
		// 命令行的基本规则：按空格分隔参数；如果参数需要带空格，那参数必须带双引号。
		// 这里稍微推广一下，因为是中文环境，所以引号也得用中文的（当然也兼容英语的引号输入）。
		// 这个正则表达式 [\""“”].+?[\""“”]|[^ ]+ 有点难理解，它其实就是专门匹配命令行参数的核心处理规则。
		// 主要用到的思路是非贪婪匹配，这里的“.+?”就是非贪婪匹配：“.”表示匹配非空白字符，“+”表示至少得有一个字符，而“+?”配在一起表示非贪婪匹配。
		// 如果一个序列特别长，那么按照基本的正则表达式处理，这一段都会被算成一个匹配项，这就叫贪婪匹配；加个问号防止正则匹配的时候匹配长文本（也就是越短越好）。
		// 给你个参考链接：
		// https://sharplab.io/#v2:EYLgtghglgdgNAFxAJwK4wD4AEBMBGAWACgsAGAAizwDoAZWARwG5izKaAVAUwA8FqASlwDmqADYRkAUR4AHZFwDOiqAHsYilkWIBjdYoTsKCJQgCCyYYvIBeYuQfkARC/uPqAHXmrhyCGGpeLnIAWghyPAoQ4HIAZiidZwALLjExVThyAHdVZDEAEydQ/OcAeQBrcgAzXPJJK2yoBCTyRVkIHSUnNwcXJy0AN0lyBWFeW3IYLizyITGeAAo+gG0PFwBdagBqAH5VjYxlgD1yda2+gEpB4cgEHRTrGxGRXmoAWQg7h4WTAwsrK7EGoKDotBZDZDkW73ciwKGfe5KagAYQgBgAPB8vgA+BYXC49dgATgW0KS1AAahAxKguFcgA===
		// 你照着这个链接看下就知道了。
		// 注意，匹配之后的命令行参数，如果本身用了引号，那么结果也带引号，这里需要手动把引号去除。这里就比较简单了，用的是 string.Trim(char[]) 方法。
		var args = m.Trim().ParseAsCommandLine("""[\""“”].+?[\""“”]|[^ ]+""", new[] { '"', '“', '”' });
		switch (context)
		{
			// 开始游戏指令。
			case { AnsweringContext.CurrentTimesliceAnswered: { } answered, ExecutingCommand: "开始游戏" } when int.TryParse(m, out var value):
			{
				answered.Add(new(sender, value));
				break;
			}

			// 开始绘图指令。
			case { DrawingContext: var drawingContext, ExecutingCommand: "开始绘图" }:
			{
				var task = args switch
				{
					// 盘面基本操作
					["清空"] => DrawingOperations.ClearAsync(messageReceiver, context),
					[var s] => DrawingOperations.SetOrDeleteDigitAsync(messageReceiver, drawingContext, s),
					["添加", var s] => DrawingOperations.AddPencilmarkAsync(messageReceiver, drawingContext, s),
					["反添加", var s] => DrawingOperations.RemovePencilmarkAsync(messageReceiver, drawingContext, s),
					["盘面", var g] => DrawingOperations.ApplyGridAsync(messageReceiver, drawingContext, g),

					// BasicViewNode 节点
					["涂色", var s, var c] => DrawingOperations.AddBasicViewNodesAsync(messageReceiver, drawingContext, s, c),
					["代数" or "袋鼠", var s, [var c and (>= 'A' and <= 'Z' or >= 'a' and <= 'z')]] => DrawingOperations.AddBabaGroupNodesAsync(messageReceiver, drawingContext, s, (Utf8Char)c),
					[("强" or "弱") and var l, var s, var e] => DrawingOperations.AddLinkNodeAsync(messageReceiver, drawingContext, l, s, e),
					["反涂色", var s] => DrawingOperations.RemoveBasicViewNodesAsync(messageReceiver, drawingContext, s),
					["反代数" or "反袋鼠", var s] => DrawingOperations.RemoveBabaGroupNodesAsync(messageReceiver, drawingContext, s),
					["反强" or "反弱", var s, var e] => DrawingOperations.RemoveLinkNodeAsync(messageReceiver, drawingContext, s, e),

					// IconViewNode 节点
					["圆形", var s, var c] => DrawingOperations.AddIconViewNodeAsync(messageReceiver, drawingContext, s, c, f1<CircleViewNode>),
					["菱形", var s, var c] => DrawingOperations.AddIconViewNodeAsync(messageReceiver, drawingContext, s, c, f1<DiamondViewNode>),
					["心形", var s, var c] => DrawingOperations.AddIconViewNodeAsync(messageReceiver, drawingContext, s, c, f1<HeartViewNode>),
					["正方形", var s, var c] => DrawingOperations.AddIconViewNodeAsync(messageReceiver, drawingContext, s, c, f1<SquareViewNode>),
					["五角星", var s, var c] => DrawingOperations.AddIconViewNodeAsync(messageReceiver, drawingContext, s, c, f1<StarViewNode>),
					["三角形", var s, var c] => DrawingOperations.AddIconViewNodeAsync(messageReceiver, drawingContext, s, c, f1<TriangleViewNode>),
					["反圆形", var s] => DrawingOperations.RemoveIconViewNodeAsync(messageReceiver, drawingContext, s, f2<CircleViewNode>),
					["反菱形", var s] => DrawingOperations.RemoveIconViewNodeAsync(messageReceiver, drawingContext, s, f2<DiamondViewNode>),
					["反心形", var s] => DrawingOperations.RemoveIconViewNodeAsync(messageReceiver, drawingContext, s, f2<HeartViewNode>),
					["反正方形", var s] => DrawingOperations.RemoveIconViewNodeAsync(messageReceiver, drawingContext, s, f2<SquareViewNode>),
					["反五角星", var s] => DrawingOperations.RemoveIconViewNodeAsync(messageReceiver, drawingContext, s, f2<StarViewNode>),
					["反三角形", var s] => DrawingOperations.RemoveIconViewNodeAsync(messageReceiver, drawingContext, s, f2<TriangleViewNode>),

					// ShapeViewNode 节点
					["横平均", var s] => DrawingOperations.AddAverageBarNodesAsync(messageReceiver, drawingContext, s, true),
					["竖平均", var s] => DrawingOperations.AddAverageBarNodesAsync(messageReceiver, drawingContext, s, false),
					["反横平均", var s] => DrawingOperations.RemoveAverageBarNodesAsync(messageReceiver, drawingContext, s, true),
					["反竖平均", var s] => DrawingOperations.RemoveAverageBarNodesAsync(messageReceiver, drawingContext, s, false),
					["双色蛋糕", var s] => DrawingOperations.AddBattenburgNodesAsync(messageReceiver, drawingContext, s),
					["反双色蛋糕", var s] => DrawingOperations.RemoveBattenburgNodesAsync(messageReceiver, drawingContext, s),
					["横连续", var s] => DrawingOperations.AddConsecutiveNodesAsync(messageReceiver, drawingContext, s, true),
					["竖连续", var s] => DrawingOperations.AddConsecutiveNodesAsync(messageReceiver, drawingContext, s, false),
					["反横连续", var s] => DrawingOperations.RemoveConsecutiveNodesAsync(messageReceiver, drawingContext, s, true),
					["反竖连续", var s] => DrawingOperations.RemoveConsecutiveNodesAsync(messageReceiver, drawingContext, s, false),

					// 其他情况。这里要返回 null。如果不写的话，是会默认产生 SwitchExpressionException 的异常的。
					_ => null
				};
				if (task is not null)
				{
					await task;
				}

				break;


				static T f1<T>(Identifier i, int c) where T : IconViewNode => (T)Activator.CreateInstance(typeof(T), new[] { i, c })!;

				static T f2<T>(int c) where T : IconViewNode => (T)Activator.CreateInstance(typeof(T), new[] { default(Identifier), c })!;
			}
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 用来将一个字符串直接拆解成一个一个的参数序列。以空格分隔。如果带有引号，则引号是一个整体，里面可包含空格。
	/// </summary>
	/// <param name="s">字符串。</param>
	/// <param name="argumentMatcher">匹配字符串参数的正则表达式。</param>
	/// <param name="trimmedCharacters">表示最终拆解字符串的时候，需要额外去除的字符。比如引号。</param>
	/// <returns>解析后的参数序列，按次序排列。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] ParseAsCommandLine(
		this string s,
		[StringSyntax(StringSyntax.Regex)] string argumentMatcher,
		char[]? trimmedCharacters
	) => from match in new Regex(argumentMatcher, RegexOptions.Singleline).Matches(s) select match.Value.Trim(trimmedCharacters);
}
