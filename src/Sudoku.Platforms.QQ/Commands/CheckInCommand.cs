namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the check-in command.
/// </summary>
[Command]
file sealed class CheckInCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("CheckIn")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { Sender.Id: var senderId })
		{
			return false;
		}

		var userData = InternalReadWrite.Read(senderId, new() { QQ = senderId });
		switch (userData)
		{
			case { LastCheckIn: { Date: var date, TimeOfDay: var time } } when date == DateTime.Today:
			{
				// Disallow user checking in multiple times in a same day.
				var timeStr = time.ToString("hh' \u70b9 'mm' \u5206'");
				await e.SendMessageAsync(string.Format(R.MessageFormat("CheckInFailedDueToMultipleInSameDay")!, timeStr));

				return true;
			}
			case { LastCheckIn: var dateTime } when (DateTime.Today - dateTime.Date).Days == 1:
			{
				// Continuous.
				userData.ComboCheckedIn++;

				var expEarned = Scorer.GenerateValueEarned(userData.ComboCheckedIn);
				userData.Score += expEarned;
				userData.LastCheckIn = DateTime.Now;

				await e.SendMessageAsync(
					string.Format(
						R.MessageFormat("CheckInSuccessfulAndContinuous")!,
						userData.ComboCheckedIn,
						Scorer.GetEarnedScoringDisplayingString(expEarned)
					)
				);

				break;
			}
			default:
			{
				// Normal case.
				userData.ComboCheckedIn = 1;

				var expEarned = Scorer.GenerateOriginalValueEarned();
				userData.Score += expEarned;
				userData.LastCheckIn = DateTime.Now;

				await e.SendMessageAsync(string.Format(R.MessageFormat("CheckInSuccessful")!, Scorer.GetEarnedScoringDisplayingString(expEarned)));
				break;
			}
		}

		InternalReadWrite.Write(userData);
		return true;
	}
}
