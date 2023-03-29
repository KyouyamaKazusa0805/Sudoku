namespace Sudoku.Workflow.Bot.Oicq.RootCommands.Generalized;

/// <summary>
/// 艾特指令。该指令较为特殊，它不带有前缀，也不是人触发的条件。
/// 它主要是为了去处理一些其他指令里的回复情况，如开始游戏后，回答问题需要艾特的时候，就会用到这个指令来处理。
/// </summary>
[Command]
file sealed class MentioningCommand : IModule
{
	/// <summary>
	/// 字符串的分割字符，用于绘图操作输入一系列数据的时候使用。
	/// </summary>
	private static readonly char[] Separator = { ',', '，' };


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

		const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
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
				switch (message.Trim().Split(' ', splitOptions))
				{
					// 132：往 r1c3（即 A3 格）填入 2
					case [var rawString]:
					{
						var puzzle = drawingContext.Puzzle;
						foreach (var element in split(rawString))
						{
							if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '0' and <= '9']
								&& g(r, c, d) is var (cell, digit))
							{
								switch (drawingContext.Puzzle.GetStatus(cell))
								{
									case CellStatus.Undefined:
									case CellStatus.Modifiable:
									{
										if (digit == -1)
										{
											puzzle.SetMask(cell, 0);
										}
										else
										{
											puzzle[cell] = digit;
										}

										break;
									}
									case not CellStatus.Given:
									{
										@throw();
										break;
									}
								}
							}
						}

						drawingContext.Puzzle = puzzle;
						drawingContext.Painter.WithGrid(puzzle);

						await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
						break;
					}

					// 增加 132：往 r1c3（即 A3 格）增加候选数 2
					case ["增加", var rawString]:
					{
						foreach (var element in split(rawString))
						{
							if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
								&& g(r, c, d) is var (cell, digit))
							{
								switch (drawingContext.Puzzle.GetStatus(cell))
								{
									case CellStatus.Undefined:
									{
										drawingContext.Pencilmarks.Add(cell * 9 + digit);
										break;
									}
									case not (CellStatus.Modifiable or CellStatus.Given):
									{
										@throw();
										break;
									}
								}
							}
						}

						drawingContext.UpdateCandidatesViaPencilmarks();
						drawingContext.Painter.WithGrid(drawingContext.Puzzle);

						await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
						break;
					}

					// 删除 132：将 r1c3（即 A3 格）里的候选数 2 删去
					case ["删除", var rawString]:
					{
						foreach (var element in split(rawString))
						{
							if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
								&& g(r, c, d) is var (cell, digit))
							{
								switch (drawingContext.Puzzle.GetStatus(cell))
								{
									case CellStatus.Undefined:
									{
										drawingContext.Pencilmarks.Remove(cell * 9 + digit);
										break;
									}
									case not (CellStatus.Modifiable or CellStatus.Given):
									{
										@throw();
										break;
									}
								}
							}
						}

						drawingContext.UpdateCandidatesViaPencilmarks();
						drawingContext.Painter.WithGrid(drawingContext.Puzzle);

						await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
						break;
					}
				}

				break;
			}
		}


		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void @throw() => throw new InvalidOperationException("Operation failed due to internal exception.");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string[] split(string rawString) => rawString.Split(Separator, splitOptions);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static (int Cell, int Digit) g(char r, char c, char d) => ((r - '1') * 9 + (c - '1'), d - '1');
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
