namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一个题库的数据。
/// </summary>
public sealed class PuzzleLibrary
{
	/// <summary>
	/// 表示题库在目标群里已经完成了多少个了。
	/// </summary>
	/// <remarks>
	/// 由于题库机制的设计，我们抽取题目是固定按照次序抽取，而不是随机抽取题目。这样的话，可以只需要这一个属性就可以控制完成题目的基本数据信息，比较方便代码书写。
	/// </remarks>
	[JsonPropertyName("finishedCount")]
	public int FinishedPuzzlesCount { get; set; }

	/// <summary>
	/// 表示题库的题库名称。
	/// </summary>
	[JsonPropertyName("name")]
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }

	/// <summary>
	/// 表示题库的文件路径。
	/// </summary>
	[JsonPropertyName("path")]
	public required string Path { get; set; }

	/// <summary>
	/// 表示题库的所在群。
	/// </summary>
	[JsonPropertyName("id")]
	public required string GroupId { get; set; }
}
