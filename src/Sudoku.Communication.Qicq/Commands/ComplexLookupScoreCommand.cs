namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the complex lookup score command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator)]
internal sealed class ComplexLookupScoreCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_ComplexLookupScore"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e.Sender is not { Name: var senderName, MmeberProfile.NickName: var senderOriginalName, Group: var group })
		{
			return false;
		}

		if (args is [])
		{
			return false;
		}

		var satisfiedMembers = (
			from member in await @group.GetGroupMembersAsync()
			where member.Id == args || member.Name == args
			select member
		).ToArray();
		switch (satisfiedMembers)
		{
			case []:
			{
				await e.SendMessageAsync(R["_MessageFormat_LookupNameOrIdInvalid"]);
				break;
			}
			case { Length: >= 2 }:
			{
				await e.SendMessageAsync(R["_MessageFormat_LookupNameOrIdAmbiguous"]);
				break;
			}
			case [{ Id: var foundMemberId }]:
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					goto DirectlyReturn;
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

				var userDataPath = $"""{botUsersDataFolder}\{foundMemberId}.json""";
				if (!File.Exists(userDataPath))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var userData = Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));

				goto DirectlyReturn;

			SpecialCase_UserDataFileNotFound:
				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));

			DirectlyReturn:
				return true;
			}
		}

		return true;
	}
}
