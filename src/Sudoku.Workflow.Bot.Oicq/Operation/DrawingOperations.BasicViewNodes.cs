namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
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
	public static async partial Task AddBasicViewNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string colorString)
	{
		if (colorString.GetIdentifier() is { } identifier)
		{
			var nodes = RecordBasicNodesInternal(raw, identifier);
			await receiver.SendPictureThenDeleteAsync(context.Painter.AddNodes(nodes));
		}
	}

	/// <summary>
	/// 从绘图盘面内删除指定的绘图节点。
	/// </summary>
	public static async partial Task RemoveBasicViewNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
	{
		var nodes = RecordBasicNodesInternal(raw);
		await receiver.SendPictureThenDeleteAsync(context.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 往绘图盘面里面追加代数字母的视图节点。
	/// </summary>
	public static async partial Task AddBabaGroupNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, Utf8Char character)
	{
		var nodes = new HashSet<BabaGroupViewNode>();
		foreach (var element in raw.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9'] && GetCellIndex(r, c) is var cell)
			{
				nodes.Add(
					new(
						// 这里是 Identifier。因为程序设计的缘故，代数的字母是不能调颜色的，所以这里设啥都没用。
						// 这里只是因为这个类型走 ViewNode 派生，而 ViewNode 需要一个 Identifier 罢了。
						default!,
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

		await receiver.SendPictureThenDeleteAsync(context.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 从绘图盘面内删除代数字母的视图节点。
	/// </summary>
	public static async partial Task RemoveBabaGroupNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
	{
		var nodes = new HashSet<BabaGroupViewNode>();
		foreach (var element in raw.LocalSplit())
		{
			if (element is [var r and >= '1' and <= '9', var c and >= '1' and <= '9'] && GetCellIndex(r, c) is var cell)
			{
				nodes.Add(new(default!, cell, default, default));
			}
		}

		await receiver.SendPictureThenDeleteAsync(context.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 往绘图盘面内绘制一条强或弱关系的链的连接线。
	/// </summary>
	/// <remarks>
	/// 注意，追加链的线条的时候，强或弱是必要的选项。因为它影响了绘制的逻辑。
	/// </remarks>
	public static async partial Task AddLinkNodeAsync(GroupMessageReceiver receiver, DrawingContext context, string linkTypeString, string startCandidateString, string endCandidateString)
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
		var node = new LinkViewNode(default!, new(startDigit, startCell), new(endDigit, endCell), inference);
		await receiver.SendPictureThenDeleteAsync(context.Painter.AddNodes(new[] { node }));
	}

	/// <summary>
	/// 从绘图盘面里删除一条强或弱关系的连接线。
	/// </summary>
	/// <remarks>
	/// 注意，删除链的线条的时候，是强或弱都无关紧要了。这是因为，链的强弱关系不可能在同一时刻落在同样两个候选数之上，
	/// 不论这两个节点原本是强还是弱，都不能再叠加新的重叠的线条。
	/// </remarks>
	public static async partial Task RemoveLinkNodeAsync(GroupMessageReceiver receiver, DrawingContext context, string startCandidateString, string endCandidateString)
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
		var node = new LinkViewNode(default!, new(startDigit, startCell), new(endDigit, endCell), default);
		await receiver.SendPictureThenDeleteAsync(context.Painter.RemoveNodes(new[] { node }));
	}

	/// <summary>
	/// 内部方法，记录 <see cref="BasicViewNode"/> 类型的节点。
	/// </summary>
	/// <seealso cref="BasicViewNode"/>
	private static HashSet<ViewNode> RecordBasicNodesInternal(string raw, ColorIdentifier identifier = default!)
	{
		var nodes = new HashSet<ViewNode>();
		foreach (var element in raw.LocalSplit())
		{
			var node = element switch
			{
				[var r and >= '1' and <= '9', var c and >= '1' and <= '9'] when GetCellIndex(r, c) is var cell
					=> new CellViewNode(identifier, cell), // 单元格。
				[var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9'] when GetCandidateIndex(r, c, d) is var (cell, digit)
					=> new CandidateViewNode(identifier, cell * 9 + digit), // 候选数。
				[var r and ('行' or '列' or '宫' or 'R' or 'r' or 'C' or 'c' or 'B' or 'b'), var i and >= '1' and <= '9'] when GetHouseIndex(r, i) is var house
					=> new HouseViewNode(identifier, house), // 区域。
				['大', var r and ('行' or '列'), var i and >= '1' and <= '3'] when GetChuteIndex(r, i) is var chute
					=> new ChuteViewNode(identifier, chute), // 大行列（汉字匹配）。
				['B' or 'b', var r and ('R' or 'r' or 'C' or 'c'), var i and >= '1' and <= '3'] when GetChuteIndex(r, i) is var chute
					=> new ChuteViewNode(identifier, chute), // 大行列（字母匹配）。
				_
					=> default(ViewNode?) // 其他情况。
			};

			if (node is not null)
			{
				nodes.Add(node);
			}
		}

		return nodes;
	}
}
