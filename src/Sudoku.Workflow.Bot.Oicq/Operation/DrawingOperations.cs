namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 提供一些基本绘图操作，和 <see cref="Drawing"/> 以及 <see cref="Gdip"/> 命名空间进行交互的方法集。
/// </summary>
/// <seealso cref="Drawing"/>
/// <seealso cref="Gdip"/>
internal static partial class DrawingOperations
{
	//
	// 盘面操作方法集
	//
	public static partial Task ClearAsync(GroupMessageReceiver messageReceiver, BotRunningContext context);
	public static partial Task SetOrDeleteDigitAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task AddPencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task RemovePencilmarkAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task ApplyGridAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string gridString);

	//
	// BasicViewNode 节点处理方法集
	//
	public static partial Task AddBasicViewNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, string colorString);
	public static partial Task RemoveBasicViewNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task AddBabaGroupNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, Utf8Char character);
	public static partial Task RemoveBabaGroupNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task AddLinkNodeAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string linkTypeString, string startCandidateString, string endCandidateString);
	public static partial Task RemoveLinkNodeAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string startCandidateString, string endCandidateString);

	//
	// IconViewNode 节点处理方法集
	//
	public static partial Task AddIconViewNodeAsync<TNode>(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, string colorString, Func<Identifier, int, TNode> nodeCreator) where TNode : IconViewNode;
	public static partial Task RemoveIconViewNodeAsync<TNode>(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, Func<int, TNode> nodeCreator) where TNode : IconViewNode;

	//
	// ShapeViewNode 节点处理方法集
	//
	public static partial Task AddAverageBarNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, bool isHorizontal);
	public static partial Task RemoveAverageBarNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, bool isHorizontal);
	public static partial Task AddBattenburgNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task RemoveBattenburgNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString);
	public static partial Task AddConsecutiveNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, bool isHorizontal);
	public static partial Task RemoveConsecutiveNodesAsync(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, bool isHorizontal);
}
