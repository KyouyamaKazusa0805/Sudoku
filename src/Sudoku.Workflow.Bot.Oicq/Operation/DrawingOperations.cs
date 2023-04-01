namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 提供一些基本绘图操作，和 <see cref="Drawing"/> 以及 <see cref="Gdip"/> 命名空间进行交互的方法集。
/// </summary>
/// <seealso cref="Drawing"/>
/// <seealso cref="Gdip"/>
internal static partial class DrawingOperations
{
	//
	// 全局方法集
	//
	public static partial Task ClearAsync(GroupMessageReceiver receiver, BotRunningContext context);

	//
	// 盘面操作方法集
	//
	public static partial Task SetOrDeleteDigitAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddPencilmarkAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task RemovePencilmarkAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task ApplyGridAsync(GroupMessageReceiver receiver, DrawingContext context, string gridString);

	//
	// BasicViewNode 节点处理方法集
	//
	public static partial Task AddBasicViewNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string colorString);
	public static partial Task RemoveBasicViewNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddBabaGroupNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, Utf8Char character);
	public static partial Task RemoveBabaGroupNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddLinkNodeAsync(GroupMessageReceiver receiver, DrawingContext context, string linkTypeString, string startCandidateString, string endCandidateString);
	public static partial Task RemoveLinkNodeAsync(GroupMessageReceiver receiver, DrawingContext context, string startCandidateString, string endCandidateString);

	//
	// IconViewNode 节点处理方法集
	//
	public static partial Task AddIconViewNodeAsync<T>(GroupMessageReceiver receiver, DrawingContext context, string raw, string colorString, Func<Identifier, int, T> nodeCreator) where T : IconViewNode;
	public static partial Task RemoveIconViewNodeAsync<T>(GroupMessageReceiver receiver, DrawingContext context, string raw, Func<int, T> nodeCreator) where T : IconViewNode;

	//
	// ShapeViewNode 节点处理方法集
	//
	public static partial Task AddAverageBarNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task RemoveAverageBarNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task AddBattenburgNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task RemoveBattenburgNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddConsecutiveNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task RemoveConsecutiveNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task AddCellArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionString);
	public static partial Task RemoveCellArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddCellCornerArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionsString);
	public static partial Task RemoveCellCornerArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddCellCornerTriangleNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionsString);
	public static partial Task RemoveCellCornerTriangleNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddClockfaceNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isClockwise);
	public static partial Task RemoveClockfaceNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isClockwise);
	public static partial Task AddEmbeddedSkyscraperArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionsString);
	public static partial Task RemoveEmbeddedSkyscraperArrowNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddGreaterThanNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal, bool isGreaterThan);
	public static partial Task RemoveGreaterThanNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task AddKropkiNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal, bool isBlack);
	public static partial Task RemoveKropkiNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task AddNeighborNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isCross);
	public static partial Task RemoveNeighborNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddAdjacentLabelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string labelString, bool isHorizontal);
	public static partial Task RemoveAdjacentLabelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, bool isHorizontal);
	public static partial Task AddCellPencilmarkNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string notationString);
	public static partial Task RemoveCellPencilmarkNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddQuadHintNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string labelString);
	public static partial Task RemoveQuadHintNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddQuadMaxNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionString);
	public static partial Task RemoveQuadMaxNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	//public static partial Task AddStarProductNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddTriangleSumNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string directionString);
	public static partial Task RemoveTriangleSumNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
	public static partial Task AddWheelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw, string digitsString);
	public static partial Task RemoveWheelNodesAsync(GroupMessageReceiver receiver, DrawingContext context, string raw);
}
