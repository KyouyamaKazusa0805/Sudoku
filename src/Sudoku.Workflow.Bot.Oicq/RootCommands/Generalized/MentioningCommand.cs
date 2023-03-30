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

		// 去掉头尾的空格。
		m = m.Trim();

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
				var task = m.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) switch
				{
					["清空"] => DrawingOperations.ClearAsync(messageReceiver, context),
					[var s] => DrawingOperations.SetDigitAsync(messageReceiver, drawingContext, s),
					["添加", var s] => DrawingOperations.AddPencilmarkAsync(messageReceiver, drawingContext, s),
					["反添加", var s] => DrawingOperations.RemovePencilmarkAsync(messageReceiver, drawingContext, s),
					["涂色", var s, var c] => DrawingOperations.AddBasicViewNodesAsync(messageReceiver, drawingContext, s, c),
					["反涂色", var s] => DrawingOperations.RemoveBasicViewNodesAsync(messageReceiver, drawingContext, s),
					["代数" or "袋鼠", var s, [var c and (>= 'A' and <= 'Z' or >= 'a' and <= 'z')]] => DrawingOperations.AddBabaGroupNodesAsync(messageReceiver, drawingContext, s, (Utf8Char)c),
					["反代数" or "反袋鼠", var s] => DrawingOperations.RemoveBabaGroupNodesAsync(messageReceiver, drawingContext, s),
					[var l and ("强" or "弱"), var s, var e] => DrawingOperations.AddLinkNodeAsync(messageReceiver, drawingContext, l, s, e),
					["反强" or "反弱", var s, var e] => DrawingOperations.RemoveLinkNodeAsync(messageReceiver, drawingContext, s, e),
					_ => null
				};
				if (task is not null)
				{
					await task;
				}

				break;
			}
		}
	}
}
