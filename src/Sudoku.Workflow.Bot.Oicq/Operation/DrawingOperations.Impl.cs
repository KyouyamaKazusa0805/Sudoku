using Sudoku.Runtime.MaskServices;

namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 直接清空整个绘图操作里已经设置了的视图节点，恢复为空盘面。
	/// </summary>
	public static async Task ClearAsync(GroupMessageReceiver messageReceiver, BotRunningContext context)
	{
		var painter = ISudokuPainter.Create(1000, 20)
			.WithRenderingCandidates(true)
			.WithPreferenceSettings(static pref => pref.CandidateScale = .4M)
			.WithGrid(Grid.Undefined);
		context.DrawingContext = new() { Painter = painter };

		// 利用前面创建的 painter 对象，直接产生图片并发送。这里的 painter 是初始化的结果，所以发送出去的图片也是空盘，没有标记，没有候选数，啥都没有。
		// 注意，这个指令到这里就结束了。后面没有代码了。
		// 因为这里随后的操作是艾特机器人，在别的指令里完成，这里我们不处理它（实际上也处理不了，因为 API 是这么设计的）。
		await messageReceiver.SendPictureThenDeleteAsync(painter);
	}

	/// <summary>
	/// 往绘图盘面内填入一个数（或者去掉一个填数）。
	/// </summary>
	public static async Task SetDigitAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
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
	public static async Task AddPencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
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
	public static async Task RemovePencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
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
	/// 往绘图盘面里追加涂色的视图节点。这些基本节点均走 <see cref="BasicViewNode"/> 类型进行派生，
	/// 这个方法也是为了处理所有走该类型派生的具体实例的绘图逻辑。
	/// </summary>
	/// <remarks>
	/// 这个方法处理的视图节点包含如下这些：
	/// <list type="bullet">
	/// <item>单元格 <see cref="CellViewNode"/></item>
	/// <item>候选数 <see cref="CandidateViewNode"/></item>
	/// <item>区域 <see cref="HouseViewNode"/></item>
	/// <item>大行列 <see cref="ChuteViewNode"/></item>
	/// <item>链的强弱关系 <see cref="LinkViewNode"/></item>
	/// <item>
	/// 代数字母 <see cref="BabaGroupViewNode"/>
	/// （这个在 <see cref="AddBabaGroupNodesAsync(GroupMessageReceiver, DrawingContext, string, Utf8Char)"/> 方法里处理）
	/// </item>
	/// </list>
	/// 顺带一提，之所以给代数取名叫 <c>BabaGroup</c>，是因为有一个游戏叫《<see href="https://www.hempuli.com/baba/">Baba Is You</see>》。
	/// 这游戏里有一个 Group 单词，表示放在谓词 Is 左边的词语属于同一组处理逻辑。代数思想和这一点非常相似（也是一组数字按同一种思路推导），
	/// 恰好这技巧又没有英语名字，就干脆取了个这个名字。
	/// </remarks>
	/// <seealso cref="BasicViewNode"/>
	public static async Task AddBasicViewNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		string colorString
	)
	{
		if (colorString.GetIdentifier() is { } identifier)
		{
			var nodes = RecordBasicNodesInternal(rawString, identifier);
			await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
		}
	}

	/// <summary>
	/// 从绘图盘面内删除指定的绘图节点。
	/// </summary>
	public static async Task RemoveBasicViewNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var nodes = RecordBasicNodesInternal(rawString);
		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 往绘图盘面里面追加代数字母的视图节点。
	/// </summary>
	public static async Task AddBabaGroupNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		Utf8Char character
	)
	{
		var nodes = new HashSet<BabaGroupViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9'] && GetCellIndex(r, c) is var cell)
			{
				nodes.Add(
					new(
						// 这里是 Identifier。因为程序设计的缘故，代数的字母是不能调颜色的，所以这里设啥都没用。
						// 这里只是因为这个类型走 ViewNode 派生，而 ViewNode 需要一个 Identifier 罢了。
						default,
						cell,
						character,
						// 这个参数是保留给找代数技巧的时候用的，表示的是这个代数字母代表哪些数字。
						// 比如远程数对技巧里的字母 x 可以表示两种数字，这个参数传入的就是这两个数字的掩码信息（1 << digit1 | 1 << digit2）。
						// 这里是绘图，所以跟这个参数没有任何关系——它在绘图操作期间是用不上的，这里我们设置啥都可以。
						Grid.MaxCandidatesMask
					)
				);
			}
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 从绘图盘面内删除代数字母的视图节点。
	/// </summary>
	public static async Task RemoveBabaGroupNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var nodes = new HashSet<BabaGroupViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9'] && GetCellIndex(r, c) is var cell)
			{
				nodes.Add(new(default, cell, default, default));
			}
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 往绘图盘面内绘制一条强或弱关系的链的连接线。
	/// </summary>
	/// <remarks>
	/// 注意，追加链的线条的时候，强或弱是必要的选项。因为它影响了绘制的逻辑。
	/// </remarks>
	public static async Task AddLinkNodeAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string linkTypeString,
		string startCandidateString,
		string endCandidateString
	)
	{
		if (
			(startCandidateString, endCandidateString) is not (
				[var sr and >= '1' and <= '9', var sc and >= '1' and <= '9', var sd and >= '1' and <= '9'],
				[var er and >= '1' and <= '9', var ec and >= '1' and <= '9', var ed and >= '1' and <= '9']
			)
		)
		{
			return;
		}

		var (startCell, startDigit) = GetCandidateIndex(sr, sc, sd);
		var (endCell, endDigit) = GetCandidateIndex(er, ec, ed);
		var inference = linkTypeString switch { "强" => Inference.Strong, "弱" => Inference.Weak };

		// 根据前面给的数据创建实例。
		// 和代数一样，链的强弱线条的颜色也是不配置的。
		var node = new LinkViewNode(default, new(startDigit, startCell), new(endDigit, endCell), inference);
		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(new[] { node }));
	}

	/// <summary>
	/// 从绘图盘面里删除一条强或弱关系的连接线。
	/// </summary>
	/// <remarks>
	/// 注意，删除链的线条的时候，是强或弱都无关紧要了。这是因为，链的强弱关系不可能在同一时刻落在同样两个候选数之上，
	/// 不论这两个节点原本是强还是弱，都不能再叠加新的重叠的线条。
	/// </remarks>
	public static async Task RemoveLinkNodeAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string startCandidateString,
		string endCandidateString
	)
	{
		if (
			(startCandidateString, endCandidateString) is not (
				[var sr and >= '1' and <= '9', var sc and >= '1' and <= '9', var sd and >= '1' and <= '9'],
				[var er and >= '1' and <= '9', var ec and >= '1' and <= '9', var ed and >= '1' and <= '9']
			)
		)
		{
			return;
		}

		var (startCell, startDigit) = GetCandidateIndex(sr, sc, sd);
		var (endCell, endDigit) = GetCandidateIndex(er, ec, ed);

		// 这里是用作删除比较的节点。
		// 节点只比较起始点和结束点是否一致，所以跟它是什么关系（强关系还是弱关系）没有关系。这里设置成 default 就行。
		// 注意顺序。强弱关系本身是没有方向性的，但是在绘制链的时候，是有方向性的，所以如果删除的方向反了，也是不可以正确删除掉的。
		var node = new LinkViewNode(default, new(startDigit, startCell), new(endDigit, endCell), default);
		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(new[] { node }));
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
	public static async Task ApplyGridAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string gridString)
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
			// 官方叫这种 scoped 情况叫“隐式 scoped”（Implicitly-Scoped）。
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
}
