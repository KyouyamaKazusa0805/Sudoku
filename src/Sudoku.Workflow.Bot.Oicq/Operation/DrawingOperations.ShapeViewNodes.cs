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
			if (!cell.IsValidCellForAverage(isHorizontal))
			{
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
			if (!cell.IsValidCellForAverage(isHorizontal))
			{
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
			if (!cell.IsValidCellFor2x2Cells())
			{
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
			if (!cell.IsValidCellFor2x2Cells())
			{
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
			if (!cell.IsValidCellForAdjacentCell(isHorizontal))
			{
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
			if (!cell.IsValidCellForAdjacentCell(isHorizontal))
			{
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

	/// <summary>
	/// 添加一个或一组单元格内的小箭头图标。这些小箭头图标不会整体占据整个单元格，而是标记在单元格的格线周围的 8 个方向。
	/// </summary>
	public static async partial Task AddCellCornerArrowNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		string directionsString
	)
	{
		// 模式匹配冷知识：
		// 模式匹配在使用期间，如果表达式（is 左边这个）和模式（is 右边这些）里的其中一个子模式（常量模式）里的值可进行隐式转换，
		// 那么这个隐式转换是可以不写的——它会被自动转换。也就是说，这里的 and not 0 的 0 其实等价于 default(Direction)，即 Direction 类型的默认值。
		// 虽然这个数是 0，而表达式结果是一个枚举类型的数值，看起来似乎这俩根本就不匹配，但模式匹配允许这一点。
		// 这个经常被用于枚举在模式匹配的时候和它的默认数值进行比较，比如这里。这种用法以后还会有很多。请格外留意。
		if (directionsString.ToDirections() is not (var directions and not 0))
		{
			return;
		}

		var nodes = new HashSet<CellCornerArrowViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, directions));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个或一组单元格内使用的小箭头图标。
	/// </summary>
	public static async partial Task RemoveCellCornerArrowNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString)
	{
		var nodes = new HashSet<CellCornerArrowViewNode>();
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

	/// <summary>
	/// 添加一个或一组单元格边角处的三角形图标。这个三角形和前面的箭头样貌不同，使用的变型数独也不同。
	/// 这种专门用于斜向的大小比较类数独，比如斜不等号数独。
	/// </summary>
	public static async partial Task AddCellCornerTriangleNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString,
		string directionsString
	)
	{
		if (directionsString.ToDirections() is not (var directions and not 0))
		{
			return;
		}

		// 要去掉基本的四个方向。因为这个三角形图标不能用于标准的上下左右四个方向。
		directions &= ~(Direction.Up | Direction.Down | Direction.Left | Direction.Right);

		var nodes = new HashSet<CellCornerTriangleViewNode>();
		foreach (var element in rawString.LocalSplit())
		{
			if (element is not [var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				continue;
			}

			var cell = GetCellIndex(r, c);
			nodes.Add(new(Color.DimGray.ToIdentifier(), cell, directions));
		}

		await messageReceiver.SendPictureThenDeleteAsync(drawingContext.Painter.AddNodes(nodes));
	}

	/// <summary>
	/// 删除一个或一组单元格边角处的三角形图标。
	/// </summary>
	public static async partial Task RemoveCellCornerTriangleNodesAsync(
		GroupMessageReceiver messageReceiver,
		DrawingContext drawingContext,
		string rawString
	)
	{
		var nodes = new HashSet<CellCornerTriangleViewNode>();
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
	/// <param name="isHorizontal">表示标记是否用于横向。如果不是，该参数传 <see langword="false"/>。</param>
	/// <returns>一个 <see cref="bool"/> 结果表示是否满足。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidCellForAdjacentCell(this int @this, bool isHorizontal) => !HousesMap[isHorizontal ? 26 : 17].Contains(@this);

	/// <summary>
	/// 判别当前输入的数值（对应的单元格索引）是否为绘图支持的、2x2 的四个单元格之间的标记的指定单元格数值。
	/// </summary>
	/// <param name="this">单元格索引。</param>
	/// <returns>一个 <see cref="bool"/> 结果表示是否满足。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidCellFor2x2Cells(this int @this)
		=> !Array.Exists(new[] { HousesMap[17], HousesMap[26] }, c => c.Contains(@this));

	/// <summary>
	/// 判别当前输入的数值（对应的单元格索引）是否为绘图支持的、平均数独线条的指定单元格数值。
	/// </summary>
	/// <param name="this">单元格索引。</param>
	/// <param name="isHorizontal">表示标记是否用于横向。如果不是，该参数传 <see langword="false"/>。</param>
	/// <returns>一个 <see cref="bool"/> 结果表示是否满足。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidCellForAverage(this int @this, bool isHorizontal)
		=> !Array.Exists(isHorizontal ? new[] { HousesMap[18], HousesMap[26] } : new[] { HousesMap[9], HousesMap[17] }, c => c.Contains(@this));

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

	/// <summary>
	/// 根据指定的方向字符串，转换为对应的 <see cref="Direction"/>[] 结果；如果内部出现的文字不合法，则会被忽略。
	/// </summary>
	/// <param name="this">方向字符串。</param>
	/// <returns>一个 <see cref="Direction"/> 数组结果。</returns>
	public static Direction ToDirections(this string @this)
	{
		var split = @this.Split(DrawingOperations.Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var result = Direction.None;
		foreach (var element in split)
		{
			if (element.ToDirection() is { } direction)
			{
				result |= direction;
			}
		}

		return result;
	}
}
