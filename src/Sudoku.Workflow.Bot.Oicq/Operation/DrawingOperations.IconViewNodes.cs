namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 添加一个单元格图标。
	/// </summary>
	public static async partial Task AddIconViewNodeAsync<T>(GroupMessageReceiver receiver, DrawingContext context, string raw, string colorString, Func<Identifier, int, T> nodeCreator)
		where T : IconViewNode
	{
		if (colorString.GetIdentifier() is not { } identifier)
		{
			return;
		}

		var nodes = new HashSet<IconViewNode>();
		foreach (var element in raw.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(nodeCreator(identifier, cell));
		}

		await receiver.SendPictureThenDeleteAsync(context.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个单元格图标。
	/// </summary>
	public static async partial Task RemoveIconViewNodeAsync<T>(GroupMessageReceiver receiver, DrawingContext context, string raw, Func<int, T> nodeCreator)
		where T : IconViewNode
	{
		var nodes = new HashSet<IconViewNode>();
		foreach (var element in raw.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(nodeCreator(cell));
		}

		await receiver.SendPictureThenDeleteAsync(context.Painter.RemoveNodes(nodes));
	}
}
