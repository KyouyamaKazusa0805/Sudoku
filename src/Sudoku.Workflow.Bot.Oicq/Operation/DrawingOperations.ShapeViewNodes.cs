namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 添加一个或一组平均数独里的平均线。
	/// </summary>
	public static async partial Task AddAverageBarNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		bool isHorizontal
	)
	{
		var nodes = new HashSet<AverageBarViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, isHorizontal ? AdjacentCellType.Rowish : AdjacentCellType.Columnish));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个或一组平均线。
	/// </summary>
	public static async partial Task RemoveAverageBarNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		bool isHorizontal
	)
	{
		var nodes = new HashSet<AverageBarViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, isHorizontal ? AdjacentCellType.Rowish : AdjacentCellType.Columnish));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 添加一个或一组双色蛋糕数独里的双色蛋糕标记。
	/// </summary>
	public static async partial Task AddBattenburgNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var nodes = new HashSet<BattenburgViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.Black.ToIdentifier(), cell));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个或一组双色蛋糕标记。
	/// </summary>
	public static async partial Task RemoveBattenburgNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var nodes = new HashSet<BattenburgViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.Black.ToIdentifier(), cell));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}
}
