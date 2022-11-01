namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the update score command.
/// </summary>
[Command(Permissions.Owner)]
internal sealed class UpdateScoreCommand : Command
{
	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var command = R["_Command_UpdateScore"]!;
		if (!args.StartsWith(command))
		{
			return false;
		}

		args = args[command.Length..];
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
			await e.SendMessageAsync(R["_MessageFormat_RankingListIsEmpty"]!);
			return true;
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			await e.SendMessageAsync(R["_MessageFormat_RankingListIsEmpty"]!);
			return true;
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

		await e.SendMessageAsync(string.Format(R["_MessageFormat_ScoreAppending"]!, targetName, value));
		return true;
	}
}
