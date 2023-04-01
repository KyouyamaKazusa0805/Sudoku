namespace Sudoku.Workflow.Bot.Oicq.Operation;

partial class DrawingOperations
{
	/// <summary>
	/// 添加一个或一组平均数独里的平均线。
	/// </summary>
	public static async partial Task AddAverageBarNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new AverageBarViewNode(Color.DimGray.ToIdentifier(), cell, isHorizontal ? AdjacentCellType.Rowish : AdjacentCellType.Columnish),
			cell => cell.IsValidCellForAverage(isHorizontal)
		);

	/// <summary>
	/// 删除一个或一组平均线。
	/// </summary>
	public static async partial Task RemoveAverageBarNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new AverageBarViewNode(default, cell, isHorizontal ? AdjacentCellType.Rowish : AdjacentCellType.Columnish),
			cell => cell.IsValidCellForAverage(isHorizontal)
		);

	/// <summary>
	/// 添加一个或一组双色蛋糕数独里的双色蛋糕标记。
	/// </summary>
	public static async partial Task AddBattenburgNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			static cell => new BattenburgViewNode(Color.Black.ToIdentifier(), cell),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 删除一个或一组双色蛋糕标记。
	/// </summary>
	public static async partial Task RemoveBattenburgNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new BattenburgViewNode(default, cell),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 添加一个或一组连续数独使用的连续挡板标记。
	/// </summary>
	/// <remarks>
	/// 注意，这里的参数 <paramref name="isHorizontal"/> 的横纵是取决于单元格相邻的关系，<b>不是挡板线条的横纵走向</b>。
	/// </remarks>
	public static async partial Task AddConsecutiveNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new BorderBarViewNode(Color.DimGray.ToIdentifier(), cell, cell + (isHorizontal ? 1 : 9)),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 删除一个或一组连续数独使用的连续挡板标记。
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="AddConsecutiveNodesAsync(GroupMessageReceiver, DrawingContext, string, bool)" path="/remarks"/>
	/// </remarks>
	public static async partial Task RemoveConsecutiveNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new BorderBarViewNode(default, cell, cell + (isHorizontal ? 1 : 9)),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 添加一个或一组单元格箭头图标。这种图标主要用于寻 9 数独之中。
	/// </summary>
	public static async partial Task AddCellArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionString)
	{
		if (directionString.ToDirection() is not { } direction)
		{
			return;
		}

		await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new CellArrowViewNode(Color.DimGray.ToIdentifier(), cell, direction),
			null
		);
	}

	/// <summary>
	/// 删除一个或一组单元格箭头图标。
	/// </summary>
	public static async partial Task RemoveCellArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new CellArrowViewNode(default, cell, default),
			null
		);

	/// <summary>
	/// 添加一个或一组单元格内的小箭头图标。这些小箭头图标不会整体占据整个单元格，而是标记在单元格的格线周围的 8 个方向。
	/// </summary>
	public static async partial Task AddCellCornerArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionsString)
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

		await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new CellCornerArrowViewNode(Color.DimGray.ToIdentifier(), cell, directions),
			null
		);
	}

	/// <summary>
	/// 删除一个或一组单元格内使用的小箭头图标。
	/// </summary>
	public static async partial Task RemoveCellCornerArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new CellCornerArrowViewNode(default, cell, default),
			null
		);

	/// <summary>
	/// 添加一个或一组单元格边角处的三角形图标。这个三角形和前面的箭头样貌不同，使用的变型数独也不同。
	/// 这种专门用于斜向的大小比较类数独，比如斜不等号数独。
	/// </summary>
	public static async partial Task AddCellCornerTriangleNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionsString)
	{
		if (directionsString.ToDirections() is not (var directions and not 0))
		{
			return;
		}

		// 要去掉基本的四个方向。因为这个三角形图标不能用于标准的上下左右四个方向。
		directions &= ~(Direction.Up | Direction.Down | Direction.Left | Direction.Right);

		await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new CellCornerTriangleViewNode(Color.DimGray.ToIdentifier(), cell, directions),
			null
		);
	}

	/// <summary>
	/// 删除一个或一组单元格边角处的三角形图标。
	/// </summary>
	public static async partial Task RemoveCellCornerTriangleNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new CellCornerTriangleViewNode(default, cell, default),
			null
		);

	/// <summary>
	/// 添加一个或一组钟面数独的黑或白色圆点。
	/// </summary>
	public static async partial Task AddClockfaceNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isClockwise)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new ClockfaceDotViewNode(Color.DimGray.ToIdentifier(), cell, isClockwise),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 删除一个或一组钟面数独的黑色或白色圆点。
	/// </summary>
	public static async partial Task RemoveClockfaceNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isClockwise)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new ClockfaceDotViewNode(default, cell, isClockwise),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 添加一个或一组内摩天楼箭头。
	/// </summary>
	public static async partial Task AddEmbeddedSkyscraperArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionsString)
	{
		if (directionsString.ToDirections() is not (var directions and not 0))
		{
			return;
		}

		// 内摩天楼箭头不允许斜向指向。
		directions &= ~(Direction.TopLeft | Direction.TopRight | Direction.BottomLeft | Direction.BottomRight);

		await GeneratePictureAsync(
		   receiver,
		   context,
		   raw,
		   true,
		   cell => new EmbeddedSkyscraperArrowViewNode(Color.Black.ToIdentifier(), cell, directions),
		   null
	   );
	}

	/// <summary>
	/// 删除一个或一组内摩天楼箭头。
	/// </summary>
	public static async partial Task RemoveEmbeddedSkyscraperArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new EmbeddedSkyscraperArrowViewNode(default, cell, default),
			null
		);

	/// <summary>
	/// 添加一个或一组不等号数独（数比数独）的大于或小于符号。
	/// </summary>
	public static async partial Task AddGreaterThanNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal, bool isGreaterThan)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new GreaterThanSignViewNode(Color.DimGray.ToIdentifier(), cell, cell + (isHorizontal ? 1 : 9), isGreaterThan),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 删除一个或一组不等号数独（数比数独）的大小于符号。
	/// </summary>
	public static async partial Task RemoveGreaterThanNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new GreaterThanSignViewNode(default, cell, cell + (isHorizontal ? 1 : 9), default),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 添加一个或一组黑白点数独里使用的黑白点。
	/// </summary>
	public static async partial Task AddKropkiNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal, bool isBlack)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new KropkiDotViewNode(Color.DimGray.ToIdentifier(), cell, cell + (isHorizontal ? 1 : 9), isBlack),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 删除一个或一组黑白点数独里使用的黑白点。
	/// </summary>
	public static async partial Task RemoveKropkiNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new KropkiDotViewNode(default, cell, cell + (isHorizontal ? 1 : 9), default),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 添加一个或一组邻居数独里使用的圆圈和叉叉图标。
	/// </summary>
	public static async partial Task AddNeighborNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isCross)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new NeighborSignViewNode(Color.FromArgb(128, Color.Black).ToIdentifier(), cell, isCross),
			null
		);

	/// <summary>
	/// 删除一个或一组邻居数独里使用的圆圈和叉叉图标。
	/// </summary>
	public static async partial Task RemoveNeighborNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new NeighborSignViewNode(default, cell, default),
			null
		);

	/// <summary>
	/// 往相邻的两个单元格的格线上添加标签信息。这里的标签一般是数字，比如用于四则运算数独；但也可以输入一些其他的字符，比如字母之类的。
	/// </summary>
	public static async partial Task AddAdjacentLabelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string labelString, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new NumberLabelViewNode(Color.Red.ToIdentifier(), cell, cell + (isHorizontal ? 1 : 9), labelString),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 从盘面上删除一个或一组相邻单元格标记的标签。
	/// </summary>
	public static async partial Task RemoveAdjacentLabelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new NumberLabelViewNode(default, cell, cell + (isHorizontal ? 1 : 9), null!),
			cell => cell.IsValidCellForAdjacentCell(isHorizontal)
		);

	/// <summary>
	/// 往单元格里追加一个标记。这个标记放在单元格的顶部，类似于注释一样的内容，你可以拿来当手动输入的候选数的记号信息。
	/// </summary>
	public static async partial Task AddCellPencilmarkNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string notationString)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new PencilMarkViewNode(cell, notationString),
			null
		);

	/// <summary>
	/// 从单元格里删除用户自定义的注释标记。
	/// </summary>
	public static async partial Task RemoveCellPencilmarkNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new PencilMarkViewNode(cell, null!),
			null
		);

	/// <summary>
	/// 添加一个或一组四个提示数数独的四格提示信息的文本。
	/// </summary>
	public static async partial Task AddQuadHintNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string labelString)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new QuadrupleHintViewNode(Color.DimGray.ToIdentifier(), cell, labelString),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 删除一个或一组四格提示数数独的提示信息文本。
	/// </summary>
	public static async partial Task RemoveQuadHintNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			cell => new QuadrupleHintViewNode(default, cell, null!),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 添加一个或一组四格最大值指向数独（“田大”数独）的箭头。
	/// </summary>
	public static async partial Task AddQuadMaxNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionString)
	{
		if (directionString.ToDirection() is not { } direction)
		{
			return;
		}

		// 去掉上下左右四个标准方向。因为箭头是放在四个格子的正中间的，它只能斜着指向这四个格子。
		direction &= ~(Direction.Up | Direction.Down | Direction.Left | Direction.Right);

		await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new QuadrupleMaxArrowViewNode(Color.DimGray.ToIdentifier(), cell, direction),
			static cell => cell.IsValidCellFor2x2Cells()
		);
	}

	/// <summary>
	/// 删除一个或一组田大数独的箭头。
	/// </summary>
	public static async partial Task RemoveQuadMaxNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new QuadrupleMaxArrowViewNode(default, cell, default),
			static cell => cell.IsValidCellFor2x2Cells()
		);

	/// <summary>
	/// 添加一个三角形求和数独的三角形。
	/// </summary>
	public static async partial Task AddTriangleSumNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionString)
	{
		if (directionString.ToDirection2() is not { } direction)
		{
			return;
		}

		await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new TriangleSumViewNode(Color.FromArgb(128, Color.Black).ToIdentifier(), cell, direction),
			null
		);
	}

	/// <summary>
	/// 删除一个三角形求和数独的三角形。
	/// </summary>
	public static async partial Task RemoveTriangleSumNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new TriangleSumViewNode(default, cell, default),
			null
		);

	/// <summary>
	/// 添加一个转轮数独的转轮。
	/// </summary>
	public static async partial Task AddWheelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string digitsString)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			true,
			cell => new WheelViewNode(Color.DimGray.ToIdentifier(), cell, digitsString),
			static cell => cell.IsValidCellForWheel()
		);

	/// <summary>
	/// 删除一个转轮数独的转轮。
	/// </summary>
	public static async partial Task RemoveWheelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw)
		=> await GeneratePictureAsync(
			receiver,
			context,
			raw,
			false,
			static cell => new WheelViewNode(default, cell, null!),
			static cell => cell.IsValidCellForWheel()
		);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// 判别当前输入的数值（对应的单元格索引）是否为绘图支持的、转轮数独的指定单元格数值。
	/// </summary>
	/// <param name="this">单元格索引。</param>
	/// <returns>一个 <see cref="bool"/> 结果表示是否满足。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValidCellForWheel(this int @this)
		=> !Array.Exists(new[] { HousesMap[9], HousesMap[17], HousesMap[18], HousesMap[26] }, c => c.Contains(@this));

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
	public static bool IsValidCellFor2x2Cells(this int @this) => !Array.Exists(new[] { HousesMap[17], HousesMap[26] }, c => c.Contains(@this));

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

	/// <inheritdoc cref="ToDirection(string)"/>
	public static Direction? ToDirection2(this string @this)
		=> @this switch
		{
			"左上" => Direction.TopLeft,
			"右上" => Direction.TopRight,
			"左下" => Direction.BottomLeft,
			"右下" => Direction.BottomRight,
			"全" => Direction.TopLeft | Direction.BottomRight
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
