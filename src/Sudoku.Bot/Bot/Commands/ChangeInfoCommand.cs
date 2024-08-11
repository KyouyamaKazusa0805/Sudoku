namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示修改信息的指令。
/// </summary>
[Command("更新")]
[CommandDescription("更新用户自己的配置信息。")]
[CommandUsage("更新 昵称 <昵称>", IsSyntax = true)]
[CommandUsage("更新 昵称 Sunnie")]
public sealed class ChangeInfoCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		var qq = message.Author.MemberOpenId;
		switch (message.GetContentArguments())
		{
			case ["昵称", var newName]:
			{
				UserData.Update(qq, d => d.VirtualNickname = newName);
				await api.SendGroupMessageAsync(message, $"你的名称已经更新为“{newName}”。");
				break;
			}
			case ["昵称", .. { Length: >= 2 }]:
			{
				await api.SendGroupMessageAsync(message, "昵称不能带空格。请重试。");
				break;
			}
			default:
			{
				await api.SendGroupMessageAsync(message, HelpUsageString);
				break;
			}
		}
	}
}
