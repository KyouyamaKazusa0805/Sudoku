namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 往绘图盘面内填入一个数（或者去掉一个填数）。
	/// </summary>
	public static async partial Task SetOrDeleteDigitAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var puzzle = drawingContext.Puzzle;
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '0' and <= '9']
				&& GetCandidateIndex(r, c, d) is var (cell, digit))
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
						throw DefaultException;
					}
				}
			}
		}

		drawingContext.Puzzle = puzzle;

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.WithGrid(puzzle));
	}

	/// <summary>
	/// 往绘图盘面内追加一个候选数标记。
	/// </summary>
	public static async partial Task AddPencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var pencilmarks = drawingContext.Pencilmarks;
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
				&& GetCandidateIndex(r, c, d) is var (cell, digit))
			{
				switch (drawingContext.Puzzle.GetStatus(cell))
				{
					case CellStatus.Undefined:
					{
						pencilmarks.Add(cell * 9 + digit);
						break;
					}
					case not (CellStatus.Modifiable or CellStatus.Given):
					{
						throw DefaultException;
					}
				}
			}
		}

		drawingContext.Pencilmarks = pencilmarks;
		drawingContext.UpdateCandidatesViaPencilmarks();
		drawingContext.Painter.WithGrid(drawingContext.Puzzle);

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
	}

	/// <summary>
	/// 从绘图盘面内删除一个已经标记了的候选数。
	/// </summary>
	public static async partial Task RemovePencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var pencilmarks = drawingContext.Pencilmarks;
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
				&& GetCandidateIndex(r, c, d) is var (cell, digit))
			{
				switch (drawingContext.Puzzle.GetStatus(cell))
				{
					case CellStatus.Undefined:
					{
						pencilmarks.Remove(cell * 9 + digit);
						break;
					}
					case not (CellStatus.Modifiable or CellStatus.Given):
					{
						throw DefaultException;
					}
				}
			}
		}

		drawingContext.Pencilmarks = pencilmarks;
		drawingContext.UpdateCandidatesViaPencilmarks();
		drawingContext.Painter.WithGrid(drawingContext.Puzzle);

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter);
	}

	/// <summary>
	/// 从 <paramref name="gridString"/> 之中获取盘面的文本字符串，并直接解析成合适的 <see cref="Grid"/> 盘面数据后，直接导入到绘图盘面里使用。
	/// </summary>
	/// <remarks>
	/// 注意，这里除了解析之后，还得考虑候选数的情况。因为前文我们用到了一种特殊逻辑去支持用户友好的标记候选数的模式（直观模式）。这个模式下，盘面的空格
	/// 并非为 <see cref="CellStatus.Empty"/> 状态的，而是 <see cref="CellStatus.Undefined"/> 状态的。盘面默认情况是 <see cref="CellStatus.Empty"/>，
	/// 所以我们在解析成正确结果后，还需要将每一个空格的状态手动调整成 <see cref="CellStatus.Undefined"/>，来达到兼容。
	/// </remarks>
	/// <seealso cref="Grid"/>
	/// <seealso cref="CellStatus.Empty"/>
	/// <seealso cref="CellStatus.Undefined"/>
	public static async partial Task ApplyGridAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string gridString)
	{
		// 解析 gridString，看它是不是合适的盘面的文本字符串。
		if (!Grid.TryParse(gridString, out var grid))
		{
			await messageReceiver.SendMessageAsync("请输入正确的盘面文本字符串。");
			return;
		}

		// 手动处理盘面，将底层的比特位重新调整一下，将空格状态调整为 CellStatus.Undefined。
		manualUpdateCellStatus(ref grid);

		// 调整完成，赋值回去，并渲染出来即可。
		drawingContext.Puzzle = grid;
		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.WithGrid(grid));


		static void manualUpdateCellStatus(scoped ref Grid grid)
		{
			// 这里我们用到了一个 C# 7 的特性。C# 7 有一个特性（我不知道叫什么，因为官方只是说了 ref-local 和 ref-return 的概念，
			// 但这个只是用到了思想，而不是概念），它允许我们在 foreach 循环里的迭代变量上声明 ref 关键字（或 ref readonly 组合关键字），
			// 来迭代一个数据类型的成员的引用。实现逻辑是，使用自定义的迭代器类型对象（一般都叫 Enumerator）。
			// 一般来说，迭代器的实现需要两个重要成员：
			//
			//     1、Current 属性（表示当前迭代到哪里了，这个属性返回的就是迭代变量的各个值）；
			//     2、MoveNext 方法（表示每一次迭代单位下，“游标”往下移动一个单位的操作）。
			//
			// C# 7 则允许我们将 Current 属性设置为 ref 和 ref readonly 返回的属性，这样每一个成员的结果都将通过 Current 返回出对应的引用。
			// 如果是 ref readonly，则引用只读（即你不能修改变量本身的指向）；如果是 ref，则你可以通过该迭代变量改掉底层指向的那个迭代数据，它自身的数据。
			// 这里我们就用到的是这个特性。
			// C# 11 又允许我们手动控制引用的生命周期（使用的是 scoped 关键字），scoped 关键字表示，它只能在当前代码块内可用（比如方法里声明的 scoped 变量，
			// 就表示该变量只能在这个方法体内可以用；如果你要传入别的调用方法里，需要同时给这个方法的这个传入的参数的引用也追加 scoped 关键字）。
			// 这里，foreach 循环的迭代变量只能在大括号里使用，所以它其实是隐式声明了 scoped 关键字的，也就是这个 foreach 的头部其实完整写法长这样：
			//
			//     foreach (scoped ref var mask in grid.EnumerateMasks())
			//
			// 但是，因为 foreach 循环的迭代变量本身就不可能超出这个循环体，所以它标记不标记 scoped 都是一回事。写了可以，但是不写也可以。
			// 官方叫这种 scoped 情况叫“隐式 scoped 的变量”（Implicitly-Scoped Variables）。
			foreach (ref var mask in grid.EnumerateMasks())
			{
				// mask 是盘面每一个单元格的底层掩码。掩码用的是 12 个比特位表示一个单元格的信息。其中最后 9 个比特是表示这个格子里数字 1 到 9 的可能填数状态
				// （即有或无）；而高 3 个比特是表示单元格当前状态（空格、提示数、填入数）。
				// 如果一个格子从数据上来说合法，那么它的掩码里，高 3 个比特位上必然不是 0。但是，我们这里要故意让高 3 个比特位置为 0，
				// 是为了兼容前面设计的这种绘图模式，所以，我们必须舍弃掉高 3 个比特位的数字（除非它不是空格）。
				// 所以，表达式应为 “(mask >> 9 & 7) == (int)CellStatus.Empty”。
				// 不过这里我提供了一个底层的处理方法，叫 MaskToStatus，我们直接调用它就行，它和这个表达式是等价的。
				if (MaskOperations.MaskToStatus(mask) == CellStatus.Empty)
				{
					mask &= Grid.MaxCandidatesMask;
				}
			}
		}
	}

	/// <summary>
	/// 将该实例的 <see cref="DrawingContext.Pencilmarks"/> 属性，所代表的候选数覆盖到 <see cref="DrawingContext.Puzzle"/> 属性之中。
	/// </summary>
	/// <param name="this">表示作用于某实例。</param>
	/// <seealso cref="DrawingContext.Pencilmarks"/>
	/// <seealso cref="DrawingContext.Puzzle"/>
	private static void UpdateCandidatesViaPencilmarks(this DrawingContext @this)
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
