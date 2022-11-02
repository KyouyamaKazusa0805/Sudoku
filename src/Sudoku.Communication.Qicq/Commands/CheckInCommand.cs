namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the check-in command.
/// </summary>
[Command]
internal sealed class CheckInCommand : Command
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

		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			// This folder is special; if the computer does not contain the folder, we should return directly.
			return true;
		}

		var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			Directory.CreateDirectory(botUsersDataFolder);
		}

		var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
		var userData = File.Exists(userDataPath) ? Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))! : new() { QQ = senderId };

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

			var expEarned = EarningExperiencePointGenerator.GenerateValue(userData.ComboCheckedIn);
			userData.Score += expEarned;
			userData.LastCheckIn = DateTime.Today;

			await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessfulAndContinuous"]!, userData.ComboCheckedIn, expEarned));
		}
		else
		{
			// Normal case.
			userData.ComboCheckedIn = 1;

			var expEarned = EarningExperiencePointGenerator.GenerateOriginalValue();
			userData.Score += expEarned;
			userData.LastCheckIn = DateTime.Today;

			await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessful"]!, expEarned));
		}

		var json = Serialize(userData);
		await File.WriteAllTextAsync(userDataPath, json);

		return true;
	}
}

/// <summary>
/// The earning experience point generator.
/// </summary>
file static class EarningExperiencePointGenerator
{
	/// <summary>
	/// Generates a value that describes the experience point that the current user can be earned.
	/// </summary>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GenerateOriginalValue()
		=> Rng.Next(0, 10000) switch
		{
			< 5000 => 2,
			>= 5000 and < 7500 => 3,
			>= 7500 and < 8750 => 4,
			>= 8750 and < 9375 => 6,
			_ => 12
		};

	/// <summary>
	/// Generates a value that describes the experience point that the current user can be earned.
	/// </summary>
	/// <param name="continuousDaysCount">The number of continuous days that the user has already been checking-in.</param>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GenerateValue(int continuousDaysCount)
	{
		var earned = GenerateOriginalValue();
		var level = continuousDaysCount / 7;
		return (int)Round(earned * (level * .2 + 1));
	}
}
