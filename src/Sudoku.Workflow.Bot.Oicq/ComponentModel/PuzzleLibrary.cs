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

	/// <summary>
	/// 表示题目对难度的描述。比如说这个题库的题目都需要使用 SDC 技巧，而 SDC 技巧难度在“极难”这一档，那么这个属性就传这个“极难”进去就行，
	/// 或者“极难+”表示它除了极难还有比极难还要难的题目。
	/// </summary>
	[JsonPropertyName("difficultyText")]
	public string? DifficultyText { get; set; }

	/// <summary>
	/// 表示题库的描述信息。默认值为 <see langword="null"/>。
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// 表示题库的标签，比如题库包含的使用技巧的突出、题库的风格（对称性）等。默认值为 <see langword="null"/>。
	/// </summary>
	[JsonPropertyName("tags")]
	public string[]? Tags { get; set; }
}
