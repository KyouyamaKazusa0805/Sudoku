namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the update score command.
/// </summary>
[Command(Permissions.Owner)]
file sealed class UpdateScoreCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("UpdateScore")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { Sender.Group: var group })
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

		var split = args.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (split is not [var nameOrId, var scoreAppending])
		{
			return true;
		}

		if (!int.TryParse(scoreAppending, out var value))
		{
			return true;
		}

		if (await group.GetMemberAsync(nameOrId) is not { Id: var targetId, Name: var targetName } target)
		{
			return true;
		}

		var fileName = $"""{botUsersDataFolder}\{targetId}.json""";
		var userData = File.Exists(fileName)
			? Deserialize<UserData>(await File.ReadAllTextAsync(fileName))!
			: new() { QQ = targetId, ComboCheckedIn = 0, LastCheckIn = DateTime.MinValue, Score = 0 };

		userData.Score += value;

		await File.WriteAllTextAsync(fileName, Serialize(userData));

		await e.SendMessageAsync(string.Format(R.MessageFormat("ScoreAppending")!, targetName, Scorer.GetEarnedScoringDisplayingString(value)));
		return true;
	}
}
