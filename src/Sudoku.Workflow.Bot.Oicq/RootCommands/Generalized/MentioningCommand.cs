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
				MessageChain: [SourceMessage, AtMessage { Target: BotNumber }, PlainMessage { Text: var message }]
			} messageReceiver)
		{
			return;
		}

		if (!RunningContexts.TryGetValue(groupId, out var context))
		{
			return;
		}

		switch (context)
		{
			// 开始游戏指令。
			case { AnsweringContext.CurrentRoundAnsweredValues: { } answeredValues, ExecutingCommand: "开始游戏" }
			when int.TryParse(message.Trim(), out var validInteger):
			{
				answeredValues.Add(new(sender, validInteger));
				break;
			}

			// 开始绘图指令。
			case { DrawingContext: var drawingContext, ExecutingCommand: "开始绘图" }:
			{
				switch (message.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				{
					// 132：往 r1c3（即 A3 格）填入 2
					case [var rawString]:
					{
						await DrawingOperations.SetDigitAsync(messageReceiver, drawingContext, rawString);
						break;
					}

					// 增加 132：往 r1c3（即 A3 格）增加候选数 2
					case ["增加", var rawString]:
					{
						await DrawingOperations.AddPencilmarkAsync(messageReceiver, drawingContext, rawString);
						break;
					}

					// 删除 132：将 r1c3（即 A3 格）里的候选数 2 删去
					case ["删除", var rawString]:
					{
						await DrawingOperations.RemovePencilmarkAsync(messageReceiver, drawingContext, rawString);
						break;
					}
				}

				break;
			}
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 将该实例的 <see cref="DrawingContext.Pencilmarks"/> 属性，所代表的候选数覆盖到 <see cref="DrawingContext.Puzzle"/> 属性之中。
	/// </summary>
	/// <param name="this">表示作用于某实例。</param>
	/// <seealso cref="DrawingContext.Pencilmarks"/>
	/// <seealso cref="DrawingContext.Puzzle"/>
	public static void UpdateCandidatesViaPencilmarks(this DrawingContext @this)
	{
		var puzzle = @this.Puzzle;
		for (var c = 0; c < 81; c++)
		{
			if (puzzle.GetStatus(c) is CellStatus.Given or CellStatus.Modifiable)
			{
				continue;
			}

			for (var d = 0; d < 9; d++)
			{
				puzzle[c, d] = @this.Pencilmarks.Contains(c * 9 + d);
			}
		}

		@this.Puzzle = puzzle;
	}
}
