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
			// TODO: 有个 bug，马上修
			if (!cell.IsValidCellFor2x2Cells())
			{
				// 当前单元格不满足平均数独标记使用的地方：平均数独的线条标记格，它必须包含同时左右两个格子，或上下两个格子。边界处的单元格不具备此条件。
				// 其实你让程序画也不是画不出来，但有些变型数独的标记依赖于它所处的行列的数值；有些不一定画得出来，甚至会产生异常。
				// 为了规避这种错误，也为了好习惯，我们应率先避免错误的绘制情况。
				// 特别注意，0-8 是宫索引，9 开始才是行索引。
				continue;
			}

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
			// TODO: 有个 bug，马上修
			if (!cell.IsValidCellFor2x2Cells())
			{
				// 当前单元格不满足平均数独标记使用的地方：平均数独的线条标记格，它必须包含同时左右两个格子，或上下两个格子。边界处的单元格不具备此条件。
				// 其实你让程序画也不是画不出来，但有些变型数独的标记依赖于它所处的行列的数值；有些不一定画得出来，甚至会产生异常。
				// 为了规避这种错误，也为了好习惯，我们应率先避免错误的绘制情况。
				// 特别注意，0-8 是宫索引，9 开始才是行索引。
				continue;
			}

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
			if (!cell.IsValidCellForAdjacentCell())
			{
				// 当前单元格不满足平均数独标记使用的地方。
				continue;
			}

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
			if (!cell.IsValidCellForAdjacentCell())
			{
				// 当前单元格不满足平均数独标记使用的地方。
				continue;
			}

			nodes.Add(new(Color.Black.ToIdentifier(), cell));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 添加一个或一组连续数独使用的连续挡板标记。
	/// </summary>
	/// <remarks>
	/// 注意，这里的参数 <paramref name="isHorizontal"/> 的横纵是取决于单元格相邻的关系，<b>不是挡板线条的横纵走向</b>。
	/// </remarks>
	public static async partial Task AddConsecutiveNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		bool isHorizontal
	)
	{
		var nodes = new HashSet<BorderBarViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			// TODO: 有个 bug，马上修
			if (!cell.IsValidCellForAdjacentCell())
			{
				// 当前单元格不满足平均数独标记使用的地方。
				continue;
			}

			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, cell + 1));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个或一组连续数独使用的连续挡板标记。
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="AddConsecutiveNodesAsync(GroupMessageReceiver, DrawingContext, string, bool)" path="/remarks"/>
	/// </remarks>
	public static async partial Task RemoveConsecutiveNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		bool isHorizontal
	)
	{
		var nodes = new HashSet<BorderBarViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			// TODO: 有个 bug，马上修
			if (!cell.IsValidCellForAdjacentCell())
			{
				// 当前单元格不满足平均数独标记使用的地方。
				continue;
			}

			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, cell + (isHorizontal ? 1 : 9)));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}

	/// <summary>
	/// 添加一个或一组单元格箭头图标。这种图标主要用于寻 9 数独之中。
	/// </summary>
	public static async partial Task AddCellArrowNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		string directionString
	)
	{
		if (directionString.ToDirection() is not { } direction)
		{
			return;
		}

		var nodes = new HashSet<CellArrowViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, direction));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个或一组单元格箭头图标。
	/// </summary>
	public static async partial Task RemoveCellArrowNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var nodes = new HashSet<CellArrowViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, default));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.RemoveNodes(nodes));
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 判别当前输入的数值（对应的单元格索引）是否为绘图支持的、相邻单元格之间的格线标记的指定单元格数值。
	/// </summary>
	/// <param name="this">单元格索引。</param>
	/// <returns>一个 <see cref="bool"/> 结果表示是否满足。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidCellForAdjacentCell(this int @this)
		=> !Array.Exists(new[] { HousesMap[17], HousesMap[26] }, c => c.Contains(@this));

	/// <summary>
	/// 判别当前输入的数值（对应的单元格索引）是否为绘图支持的、2x2 的四个单元格之间的标记的指定单元格数值。
	/// </summary>
	/// <param name="this">单元格索引。</param>
	/// <returns>一个 <see cref="bool"/> 结果表示是否满足。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidCellFor2x2Cells(this int @this)
		=> !Array.Exists(new[] { HousesMap[9], HousesMap[17], HousesMap[18], HousesMap[26] }, c => c.Contains(@this));

	/// <summary>
	/// 根据指定的方向字符串，转为对应的 <see cref="Direction"/> 结果；如果不合法，将返回 <see langword="null"/>。
	/// </summary>
	/// <param name="this">字符串。</param>
	/// <returns><see cref="Direction"/> 的实例。</returns>
	public static Direction? ToDirection(this string @this)
		=> @this switch
		{
			"左" => Direction.Left,
			"右" => Direction.Right,
			"上" => Direction.Up,
			"下" => Direction.Down,
			"左上" => Direction.TopLeft,
			"右上" => Direction.TopRight,
			"左下" => Direction.BottomLeft,
			"右下" => Direction.BottomRight,
			_ => null
		};
}
