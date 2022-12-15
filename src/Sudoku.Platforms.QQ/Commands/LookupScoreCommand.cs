namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// The lookup score command.
/// </summary>
[Command]
file sealed class LookupScoreCommand : Command
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

		if (InternalReadWrite.Read(senderId) is not { Score: var score } userData)
		{
			await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));
			return true;
		}

		var grade = ICommandDataProvider.GetGrade(score);
		await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, score, senderOriginalName, grade));
		return true;
	}
}
