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
}
