namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示帮助指令。
/// </summary>
[Command("帮助")]
[CommandDescription("提示帮助信息。")]
[CommandUsage("帮助 <指令名称>", IsSyntax = true)]
[CommandUsage("帮助 查询")]
public sealed class HelpCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		await api.SendGroupMessageAsync(
			message,
			message.GetPlainArguments() is { Length: not 0 } str
				? handle(str)
				: HelpUsageString
		);


		static string handle(string str)
		{
			foreach (var command in Program.RegisteredCommands)
			{
				if (command.CommandName == str)
				{
					return $"""
						📕 帮助信息
						指令名称：{command.CommandName}
						描述：{command.Description ?? "没有描述。"}
						{command.HelpUsageString}
						""";
				}
			}
			return $"查不到名称为“{str}”的指令，请检查输入后重试。";
		}
	}
}
