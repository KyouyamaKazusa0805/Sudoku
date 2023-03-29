namespace Sudoku.Workflow.Bot.Oicq.Drawing;

partial class DrawingOperations
{
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
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
				&& GetCandidateIndex(r, c, d) is var (cell, digit))
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
						throw DefaultException;
					}
				}
			}
		}

		drawingContext.UpdateCandidatesViaPencilmarks();

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.WithGrid(drawingContext.Puzzle));
	}

	/// <summary>
	/// 从绘图盘面内删除一个已经标记了的候选数。
	/// </summary>
	public static async Task RemovePencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		foreach (var element in rawString.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
				&& GetCandidateIndex(r, c, d) is var (cell, digit))
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
						throw DefaultException;
					}
				}
			}
		}

		drawingContext.UpdateCandidatesViaPencilmarks();

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.WithGrid(drawingContext.Puzzle));
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
		if (colorString.GetIdentifier() is not { } identifier)
		{
			return;
		}

		var nodes = new HashSet<ViewNode>(ViewNodeComparer.Instance);
		foreach (var element in rawString.LocalSplit())
		{
			if (element switch
			{
				[var r and >= '1' and <= '9', var c and >= '1' and <= '9']
				when GetCellIndex(r, c) is var cell
					=> new CellViewNode(identifier, cell), // 单元格。
				[var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
				when GetCandidateIndex(r, c, d) is var (cell, digit)
					=> new CandidateViewNode(identifier, cell * 9 + digit), // 候选数。
				[var r and ('行' or '列' or '宫' or 'R' or 'r' or 'C' or 'c' or 'B' or 'b'), var i and >= '1' and <= '9']
				when GetHouseIndex(r, i) is var house
					=> new HouseViewNode(identifier, house), // 区域。
				['大', var r and ('行' or '列'), var i and >= '1' and <= '3']
				when GetChuteIndex(r, i) is var chute
					=> new ChuteViewNode(identifier, chute), // 大行列（汉字匹配）。
				['B' or 'b', var r and ('R' or 'r' or 'C' or 'c'), var i and >= '1' and <= '3']
				when GetChuteIndex(r, i) is var chute
					=> new ChuteViewNode(identifier, chute), // 大行列（字母匹配）。
				_
					=> default(ViewNode?) // 其他情况。
			} is { } nodeToBeAdded)
			{
				nodes.Add(nodeToBeAdded);
			}
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
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
		var nodes = new HashSet<BabaGroupViewNode>(ViewNodeComparer.Instance); // 这里用到的是接口泛型参数的逆变性。
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(
				new(
					// 这里是 Identifier。因为程序设计的缘故，代数的字母是不能调颜色的，所以这里设啥都没用。
					// 这里只是因为这个类型走 ViewNode 派生，而 ViewNode 需要一个 Identifier 罢了。
					default,
					cell,
					character,
					// 这个参数是保留给找代数技巧的时候用的，表示的是这个代数字母代表哪些数字。
					// 比如远程数对技巧里的字母 x 可以表示两种数字，这个参数传入的就是这两个数字的掩码信息（1 << digit1 | 1 << digit2)。
					// 这里是绘图，所以跟这个参数没有任何关系——它在绘图操作期间是用不上的，这里我们设置啥都可以。
					Grid.MaxCandidatesMask
				)
			);
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}
}

/// <summary>
/// 提供一个比较 <see cref="ViewNode"/> 相等性的实例类型。
/// </summary>
/// <remarks>
/// <para>
/// 说实话我是很不想写这个类型的。这种设计有一个毛病就是，我每次更新一个 <see cref="ViewNode"/> 的创建，就得在这里新添加一个判断。
/// 这种设计很不好，尤其是在公司上班的时候，用这种设计。
/// 不过，因为 <see cref="ViewNode"/> 的设计不太理想，它的 <see cref="ViewNode.Equals(ViewNode?)"/> 方法比较的对象，包含了颜色标识符
/// <see cref="Identifier"/> 的判断。而实际上，我们在追加颜色到绘图面板上的时候，只判断单元格啊、候选数之类的，有没有重复使用的情况。
/// 如果有，就说明这里已经绘图的时候被用过了，就不再继续了；但是由于 <see cref="ViewNode"/> 的相等性比较包含了 <see cref="Identifier"/>，
/// 就导致这种“模糊的”判断反倒大概率不成立，于是就被视为不同的 <see cref="ViewNode"/> 并按两次计算在内。
/// </para>
/// <para>
/// 所以，我们为了避免这种比较 <see cref="Identifier"/> 的情况，我们不得不自己写一个比较器对象，来避免运行时 <see cref="HashSet{T}"/> 这种
/// 要使用 <see cref="IEqualityComparer{T}"/> 对象的比较的类型，自动去获取默认的 <c>Equals</c> 比较规则。
/// </para>
/// <para>
/// 顺带一说，放在这里并使用 <see langword="file"/> 修饰符，是 C# 11 提供的一个新特性，叫“<see href="https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/file-local-types.md">基于文件的本地类型</see>”
/// （File-Local Types）。
/// 被 <see langword="file"/> 修饰符修饰的类型，只能在当前文件内可以访问，出了这个文件就看不到这个类型了。
/// 换言之，如果你想在不同文件创建同一个名字的类型，可以使用这个机制，将不同的类型执行逻辑分为多个文件存在，用 <see langword="file"/> 修饰符去修饰它们，
/// 这样它们每一个类型都是独立的，也不会因为重名而导致编译器或运行时冲突。
/// </para>
/// </remarks>
/// <seealso cref="ViewNode"/>
/// <seealso cref="ViewNode.Equals(ViewNode?)"/>
/// <seealso cref="Identifier"/>
/// <seealso cref="HashSet{T}"/>
/// <seealso cref="IEqualityComparer{T}"/>
/// <seealso cref="EqualityComparer{T}.Default"/>
/// <seealso cref="EqualityComparer{T}.Equals(T, T)"/>
file sealed class ViewNodeComparer : IEqualityComparer<ViewNode>
{
	/// <summary>
	/// 提供一个默认的实例。
	/// </summary>
	public static readonly ViewNodeComparer Instance = new();


	/// <summary>
	/// 无参构造器。该构造器设置为 <see langword="private"/> 修饰是为了防止外部调用该构造器；因为我们这里为了考虑性能的问题，
	/// 使用到的是一个 <see langword="static readonly"/> 组合修饰的字段，它在程序运行期间只创建一次。
	/// </summary>
	private ViewNodeComparer()
	{
	}


	/// <inheritdoc/>
	public bool Equals(ViewNode? x, ViewNode? y)
		=> (x, y) switch
		{
			(CellViewNode a, CellViewNode b) => a.Cell == b.Cell,
			(CandidateViewNode a, CandidateViewNode b) => a.Candidate == b.Candidate,
			(HouseViewNode a, HouseViewNode b) => a.House == b.House,
			(ChuteViewNode a, ChuteViewNode b) => a.ChuteIndex == b.ChuteIndex,
			(BabaGroupViewNode a, BabaGroupViewNode b) => a.Cell == b.Cell,
			_ => false
		};

	/// <inheritdoc/>
	public int GetHashCode(ViewNode obj)
		=> obj switch
		{
			CellViewNode o => o.Cell,
			CandidateViewNode o => o.Candidate,
			HouseViewNode o => o.House,
			ChuteViewNode o => o.ChuteIndex,
			BabaGroupViewNode o => o.Cell,
			_ => 0
		};
}
