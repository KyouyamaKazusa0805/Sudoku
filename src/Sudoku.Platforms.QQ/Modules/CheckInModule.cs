namespace Sudoku.Platforms.QQ.Modules;

[BuiltInModule]
file sealed class CheckInModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "签到";


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		var userData = InternalReadWrite.Read(senderId, new() { QQ = senderId });
		switch (userData)
		{
			case { LastCheckIn: { Date: var date, TimeOfDay: var time } } when date == DateTime.Today:
			{
				// Disallow user checking in multiple times in a same day.
				await messageReceiver.SendMessageAsync($"签到失败~ 你今天 {time:hh' 点 'mm' 分'}的时候已经签过到了，明天再来试试吧~");

				return;
			}
			case { LastCheckIn: var dateTime } when (DateTime.Today - dateTime.Date).Days == 1:
			{
				// Continuous.
				userData.ComboCheckedIn++;

				var expEarned = Scorer.GenerateValueEarned(userData.ComboCheckedIn);
				userData.ExperiencePoint += expEarned;
				userData.LastCheckIn = DateTime.Now;

				var finalScore = Scorer.GetEarnedScoringDisplayingString(expEarned);
				await messageReceiver.SendMessageAsync($"签到成功！已连续签到 {userData.ComboCheckedIn} 天~ 恭喜你获得 {finalScore} 积分。一天只能签到一次哦~");

				break;
			}
			default:
			{
				// Normal case.
				userData.ComboCheckedIn = 1;

				var expEarned = Scorer.GenerateOriginalValueEarned();
				userData.ExperiencePoint += expEarned;
				userData.LastCheckIn = DateTime.Now;

				var finalScore = Scorer.GetEarnedScoringDisplayingString(expEarned);
				await messageReceiver.SendMessageAsync($"签到成功！恭喜你获得 {finalScore} 积分。一天只能签到一次哦~");

				break;
			}
		}

		InternalReadWrite.Write(userData);
	}
}
