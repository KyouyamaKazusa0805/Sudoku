namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 表示从题库里抽取题目的指令。
/// </summary>
[Command("抽题")]
internal sealed class PuzzleLibraryExtractCommand : Command
{
	/// <summary>
	/// 表示你需要抽取的题库的所在群（即跨群抽取题目）。
	/// </summary>
	[DoubleArgument("群")]
	[Hint("表示你需要抽取的题库的所在群（即跨群抽取题目）。")]
	[DisplayingIndex(0)]
	public string? GroupName { get; set; }

	/// <summary>
	/// 表示你需要抽取的题库的所在群的 QQ 号码。
	/// </summary>
	[DoubleArgument("QQ")]
	[Hint("表示你需要抽取的题库的所在群的 QQ 号码。")]
	[DisplayingIndex(0)]
	public string? GroupId { get; set; }

	/// <summary>
	/// 表示你需要抽取的题库的题库名称。
	/// </summary>
	[DoubleArgument("名称")]
	[Hint("表示你需要抽取的题库的题库名称。")]
	[DisplayingIndex(1)]
	public string? LibraryName { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		switch (GroupName, GroupId, LibraryName)
		{
			case (null, null, null):
			{
				// 抽取此群里唯一一个题库的题目。
				break;
			}
			case (null, null, { } name):
			{
				// 抽取此群里指定名称的题目。
				break;
			}
			default:
			{
				await messageReceiver.SendMessageAsync("传入的参数有误。请检查后重试。");
				return;
			}
		}
	}
}
