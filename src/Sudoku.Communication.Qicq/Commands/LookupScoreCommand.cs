namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// The lookup score command.
/// </summary>
[Command]
internal sealed class LookupScoreCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_LookupScore"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
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
			goto UserDataFileNotFound;
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			goto UserDataFileNotFound;
		}

		var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
		if (!File.Exists(userDataPath))
		{
			goto UserDataFileNotFound;
		}

		var userData = Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
		await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));
		return true;

	UserDataFileNotFound:
		await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));
		return true;
	}
}
