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
#pragma warning disable format
					case [[var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '0' and <= '9']]:
#pragma warning restore format
					{
						var (cell, digit) = g(r, c, d);
						switch (drawingContext.Puzzle.GetStatus(cell))
						{
							case CellStatus.Undefined:
							case CellStatus.Modifiable:
							{
								var puzzle = drawingContext.Puzzle;
								if (digit == -1)
								{
									puzzle.SetMask(cell, 0);
								}
								else
								{
									puzzle[cell] = digit;
								}

								drawingContext.Puzzle = puzzle;
								drawingContext.Painter.WithGrid(puzzle);

								await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
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

					// 增加 132：往 r1c3（即 A3 格）增加候选数 2
					case ["增加", [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']]:
					{
						var (cell, digit) = g(r, c, d);
						switch (drawingContext.Puzzle.GetStatus(cell))
						{
							case CellStatus.Undefined:
							{
								if (drawingContext.Pencilmarks.Contains(cell * 9 + digit))
								{
									await messageReceiver.SendMessageAsync($"单元格 r{r}c{c} 已包含此候选数。");
									break;
								}

								drawingContext.AddCandidate(cell, digit);

								// puzzle 这里虽然传入了 lambda 里使用，还是异步的，但它毕竟不是引用，所以编译器不会告诉你不能这么用。
								// 只有当异步环境下，往 lambda 传入的值类型的值带有引用的时候（比如 ref 啊、in 之类的修饰符的参数往 lambda 里传）
								// 才会产生编译器错误。
								await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
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

					// 删除 132：将 r1c3（即 A3 格）里的候选数 2 删去
					case ["删除", [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']]:
					{
						var (cell, digit) = g(r, c, d);
						switch (drawingContext.Puzzle.GetStatus(cell))
						{
							case CellStatus.Undefined:
							{
								drawingContext.RemoveCandidate(cell, digit);

								await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
								break;
							}
							case CellStatus.Modifiable:
							case CellStatus.Given:
							{
								await messageReceiver.SendMessageAsync($"单元格 r{r}c{c} 为非空格，无法删除候选数。");
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

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 追加新候选数。
	/// </summary>
	/// <param name="this">表示作用于某实例。</param>
	/// <param name="cell">需要追加的候选数单元格。</param>
	/// <param name="digit">需要追加的候选数数值。</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddCandidate(this DrawingContext @this, int cell, int digit)
	{
		var pencilmark = @this.Pencilmarks;
		pencilmark.Add(cell * 9 + digit);
		@this.Pencilmarks = pencilmark;

		@this.UpdateCandidates();

		@this.Painter.WithGrid(@this.Puzzle);
	}

	/// <summary>
	/// 删除候选数。
	/// </summary>
	/// <param name="this">表示作用于某实例。</param>
	/// <param name="cell">需要追加的候选数单元格。</param>
	/// <param name="digit">需要追加的候选数数值。</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveCandidate(this DrawingContext @this, int cell, int digit)
	{
		var pencilmark = @this.Pencilmarks;
		pencilmark.Remove(cell * 9 + digit);
		@this.Pencilmarks = pencilmark;

		@this.UpdateCandidates();

		@this.Painter.WithGrid(@this.Puzzle);
	}

	/// <summary>
	/// 将该实例的 <see cref="DrawingContext.Pencilmarks"/> 属性，所代表的候选数覆盖到 <see cref="DrawingContext.Puzzle"/> 属性之中。
	/// </summary>
	/// <param name="this">表示作用于某实例。</param>
	/// <seealso cref="DrawingContext.Pencilmarks"/>
	/// <seealso cref="DrawingContext.Puzzle"/>
	private static void UpdateCandidates(this DrawingContext @this)
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
