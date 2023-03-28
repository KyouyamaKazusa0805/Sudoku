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
			case { DrawingContext: { Puzzle: var puzzle, Painter: var painter }, ExecutingCommand: "开始绘图" }:
			{
				switch (message.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				{
					// 132：往 r1c3（即 A3 格）增加候选数 2
					// 双层中括号表示整个 switch 括起来的那个表达式结果是一个字符串数组，其中只包含一个字符串元素，然后将该字符串继续进行列表模式匹配。
#pragma warning disable format
					case [[var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']]:
#pragma warning restore format
					{
						var (cell, digit) = g(r, c, d);
						switch (puzzle.GetStatus(cell))
						{
							case CellStatus.Empty:
							{
								if ((puzzle.GetCandidates(cell) >> digit & 1) != 0)
								{
									await messageReceiver.SendMessageAsync($"单元格 r{r}c{c} 已包含此候选数。");
									break;
								}

								puzzle[cell, digit] = true;

								// puzzle 这里虽然传入了 lambda 里使用，还是异步的，但它毕竟不是引用，所以编译器不会告诉你不能这么用。
								// 只有当异步环境下，往 lambda 传入的值类型的值带有引用的时候（比如 ref 啊、in 之类的修饰符的参数往 lambda 里传）
								// 才会产生编译器错误。
								await messageReceiver.SendPictureThenDeleteAsync(() => painter.WithGrid(puzzle));
								break;
							}
							case CellStatus.Modifiable:
							case CellStatus.Given:
							{
								await messageReceiver.SendMessageAsync($"单元格 r{r}c{c} 为非空格，无法添加候选数。");
								break;
							}
							default:
							{
								@throw();
								break;
							}
						}
						break;
					}

					// 填 132：往 r1c3（即 A3 格）填入 2
					case ["填", [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '0' and <= '9']]:
					{
						var (cell, digit) = g(r, c, d);
						switch (puzzle.GetStatus(cell))
						{
							case CellStatus.Empty:
							{
								puzzle[cell] = digit;
								await messageReceiver.SendPictureThenDeleteAsync(() => painter.WithGrid(puzzle));
								break;
							}
							case CellStatus.Modifiable:
							{
								puzzle[cell] = -1; // 这里要重置候选数，避免下回输入的数字导致候选数出现内部矛盾，引发 bug。
								puzzle[cell] = digit;
								await messageReceiver.SendPictureThenDeleteAsync(() => painter.WithGrid(puzzle));
								break;
							}
							case CellStatus.Given:
							{
								await messageReceiver.SendMessageAsync($"单元格 r{r}c{c} 是提示数。无法修改。");
								break;
							}
							default:
							{
								@throw();
								break;
							}
						}
						break;
					}

					// 删 132：将 r1c3（即 A3 格）里的候选数 2 删去
					case ["删", [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']]:
					{
						var (cell, digit) = g(r, c, d);
						puzzle[cell, digit] = false;
						await messageReceiver.SendPictureThenDeleteAsync(() => painter.WithGrid(puzzle));
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
		static (int Cell, int Digit) g(char r, char c, char d) => ((r - '1') * 9 + (c - '1'), d - '1');
	}
}
