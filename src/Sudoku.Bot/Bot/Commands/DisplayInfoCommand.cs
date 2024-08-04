namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示展示信息的指令。
/// </summary>
[Command("信息")]
[CommandUsage("信息", IsSyntax = true)]
public sealed class DisplayInfoCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		var qq = message.Author.MemberOpenId;
		var userData = UserData.Read(qq);
		var extraMessage = userData.VirtualNickname == "<匿名>"
			? """
			
			※ 建议你为自己设置一个昵称，以后方便使用。使用“/更新 昵称 昵称内容”设置。
			"""
			: string.Empty;
		await api.SendGroupMessageAsync(
			message,
			$"""
			这是 {userData.VirtualNickname} 的信息：
			・经验值：{userData.ExperienceValue}
			・金币：{userData.CoinValue}
			・连续签到天数：{userData.ComboCheckedInDays} 天{extraMessage}
			"""
		);
	}
}
