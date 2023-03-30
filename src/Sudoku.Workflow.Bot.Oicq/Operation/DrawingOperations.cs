namespace Sudoku.Workflow.Bot.Oicq.Operation;

/// <summary>
/// 提供一些基本绘图操作，和 <see cref="Drawing"/> 以及 <see cref="Gdip"/> 命名空间进行交互的方法集。
/// </summary>
/// <seealso cref="Drawing"/>
/// <seealso cref="Gdip"/>
internal static partial class DrawingOperations
{
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
	// FigureViewNode 节点处理方法集
	//
	public static partial Task AddFigureViewNodeAsync<TNode>(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, string colorString, Func<Identifier, int, TNode> nodeCreator) where TNode : FigureViewNode;
	public static partial Task RemoveFigureViewNodeAsync<TNode>(GroupMessageReceiver messageReceiver, DrawingContext drawingContext, string rawString, Func<int, TNode> nodeCreator) where TNode : FigureViewNode;
}
