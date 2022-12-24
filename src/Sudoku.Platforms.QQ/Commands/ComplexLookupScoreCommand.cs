namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the complex lookup score command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator, IsDeprecated = true)]
[Obsolete("The type is being deprecated.", false)]
file sealed class ComplexLookupScoreCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("ComplexLookupScore")!;

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
			where new[] { member.Id, member.Name }.Any(e => e == args)
			select member
		).ToArray();
		switch (satisfiedMembers)
		{
			case []:
			{
				await e.SendMessageAsync(R.MessageFormat("LookupNameOrIdInvalid"));
				break;
			}
			case { Length: >= 2 }:
			{
				await e.SendMessageAsync(R.MessageFormat("LookupNameOrIdAmbiguous"));
				break;
			}
			case [{ Id: var foundMemberId }]:
			{
				if (InternalReadWrite.Read(foundMemberId) is not { Score: var score } userData)
				{
					await e.SendMessageAsync(string.Format(R.MessageFormat("UserScoreNotFound")!, senderName, senderOriginalName));
					return true;
				}

				var grade = Scorer.GetGrade(score);
				await e.SendMessageAsync(string.Format(R.MessageFormat("UserScoreIs")!, senderName, score, senderOriginalName, grade));
				return true;
			}
		}

		return true;
	}
}
