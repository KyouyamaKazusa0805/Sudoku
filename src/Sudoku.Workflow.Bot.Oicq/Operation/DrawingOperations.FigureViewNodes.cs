namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 添加一个单元格图标。
	/// </summary>
	public static async partial Task AddIconViewNodeAsync<TNode>(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		string colorString,
		Func<Identifier, int, TNode> nodeCreator
	) where TNode : IconViewNode
	{
		if (colorString.GetIdentifier() is not { } identifier)
		{
			return;
		}

		var nodes = new HashSet<IconViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(nodeCreator(identifier, cell));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个单元格图标。
	/// </summary>
	public static async partial Task RemoveIconViewNodeAsync<TNode>(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		Func<int, TNode> nodeCreator
	) where TNode : IconViewNode
	{
		var nodes = new HashSet<IconViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(nodeCreator(cell));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}
}
