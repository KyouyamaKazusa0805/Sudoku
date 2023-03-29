namespace Sudoku.Workflow.Bot.Oicq.Lifecycle;

/// <summary>
/// 这是之前定义出来的一个绘图工具的上下文。这个上下文记录了当前绘图操作进行的环节，和已经保存起来的数据。一般取消或退出绘图，
/// 则重置这些内容。
/// </summary>
internal sealed class DrawingContext
{
	/// <summary>
	/// 表示你操作的盘面是什么。
	/// </summary>
	/// <remarks>
	/// 这个属性实际上没有什么特别大的用途，其实 <see cref="Painter"/> 属性里可以取出该数值结果，但这里先考虑单独提出来，因为可能以后有用。
	/// </remarks>
	/// <seealso cref="Painter"/>
	public Grid Puzzle { get; set; } = Grid.Undefined;

	/// <summary>
	/// 表示用户标记的候选数。
	/// </summary>
	public CandidateMap Pencilmarks { get; set; } = CandidateMap.Empty;

	/// <summary>
	/// 绘图的对象。这个对象专门用来绘图，带有初始化画布等基本的绘图操作。
	/// </summary>
	public ISudokuPainter Painter { get; set; } =
		ISudokuPainter.Create(1000, 20)
			.WithRenderingCandidates(true)
			.WithGrid(Grid.Undefined);
}
