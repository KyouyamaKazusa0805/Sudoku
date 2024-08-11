namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示签到指令。
/// </summary>
[Command("签到")]
[CommandDescription("每天签到，可以获得奖励哦！")]
[CommandUsage("签到", IsSyntax = true)]
public sealed class CheckInCommand : Command
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		var random = Random.Shared;
		var exp = random.GetCheckInExperienceValue();
		var coin = random.GetCheckInCoinValue();

		var qq = message.Author.MemberOpenId;
		var d = UserData.Read(qq);
		switch (d)
		{
			case { LastCheckIn: { Date: var date, TimeOfDay: var time } } when date == DateTime.Today:
			{
				// 禁用用户重复同一天签到多次。
				await api.SendGroupMessageAsync(message, $@"{Emoji.Happy1} 你在今天的 {time:hh\:mm} 签过了，不要重复签到嗷！");
				return;
			}
			case { LastCheckIn: var dateTime } when (DateTime.Today - dateTime.Date).Days == 1:
			{
				// 连续签到。
				var extraRate = (ScoreCalculator.TodayIsWeekend() ? 2 : 1) + ScoreCalculator.GetComboDaysRate(d.ComboCheckedInDays);
				var finalExp = (int)(exp * extraRate);
				var finalCoin = (int)(coin * extraRate);
				d.ComboCheckedInDays++;
				d.ExperienceValue += finalExp;
				d.CoinValue += finalCoin;
				d.LastCheckIn = DateTime.Now;
				await api.SendGroupMessageAsync(
					message,
					$"""
					{Emoji.Happy2} 签到成功！已连续签到 {d.ComboCheckedInDays} 天~ 恭喜获得：
					・{finalExp} 经验值
					・{finalCoin} 金币
					"""
				);
				break;
			}
			default:
			{
				// 断签，或者第一天签到。
				var extraRate = ScoreCalculator.TodayIsWeekend() ? 2 : 1;
				d.ComboCheckedInDays = 1;
				d.ExperienceValue += exp * extraRate;
				d.CoinValue += coin * extraRate;
				d.LastCheckIn = DateTime.Now;
				await api.SendGroupMessageAsync(
					message,
					$"""
					{Emoji.Happy3} 签到成功！已连续签到 {d.ComboCheckedInDays} 天~ 恭喜获得：
					・{exp} 经验值
					・{coin} 金币
					"""
				);
				break;
			}
		}

		UserData.Write(d);
	}
}
