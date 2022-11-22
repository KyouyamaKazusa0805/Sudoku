namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the complex lookup score command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator)]
file sealed class ComplexLookupScoreCommand : Command
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

		var satisfiedMembers = (from member in await @group.GetGroupMembersAsync() where member.Id == args || member.Name == args select member).ToArray();
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
				if (InternalReadWrite.Read(foundMemberId) is not { } userData)
				{
					await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));
					return true;
				}

				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));
				return true;
			}
		}

		return true;
	}
}
