namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// The lookup score command.
/// </summary>
[Command]
internal sealed class LookupScoreCommand : Command
{
	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (args != R["_Command_LookupScore"])
		{
			return false;
		}

		if (e is not { Sender: { Id: var senderId, Name: var senderName, MmeberProfile.NickName: var senderOriginalName } })
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
			goto SpecialCase_UserDataFileNotFound;
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			goto SpecialCase_UserDataFileNotFound;
		}

		var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
		if (!File.Exists(userDataPath))
		{
			goto SpecialCase_UserDataFileNotFound;
		}

		var userData = Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
		await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));
		return true;

	SpecialCase_UserDataFileNotFound:
		await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));
		return true;
	}
}
