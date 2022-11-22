namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the check-in command.
/// </summary>
[Command]
file sealed class CheckInCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_CheckIn"]!;

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
		if (userData.LastCheckIn == DateTime.Today)
		{
			// Disallow user checking in multiple times in a same day.
			await e.SendMessageAsync(R["_MessageFormat_CheckInFailedDueToMultipleInSameDay"]!);
			return true;
		}

		if ((DateTime.Today - userData.LastCheckIn).Days == 1)
		{
			// Continuous.
			userData.ComboCheckedIn++;

			var expEarned = ICommandDataProvider.GenerateValueEarned(userData.ComboCheckedIn);
			userData.Score += expEarned;
			userData.LastCheckIn = DateTime.Today;

			await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessfulAndContinuous"]!, userData.ComboCheckedIn, expEarned));
		}
		else
		{
			// Normal case.
			userData.ComboCheckedIn = 1;

			var expEarned = ICommandDataProvider.GenerateOriginalValueEarned();
			userData.Score += expEarned;
			userData.LastCheckIn = DateTime.Today;

			await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessful"]!, expEarned));
		}

		InternalReadWrite.Write(userData);

		return true;
	}
}
