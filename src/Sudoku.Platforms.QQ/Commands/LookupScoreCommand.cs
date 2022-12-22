namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// The lookup score command.
/// </summary>
[Command]
file sealed class LookupScoreCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("LookupScore")!;

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
			await e.SendMessageAsync(string.Format(R.MessageFormat("UserScoreNotFound")!, senderName, senderOriginalName));
			return true;
		}

		var grade = Scorer.GetGrade(score);
		await e.SendMessageAsync(string.Format(R.MessageFormat("UserScoreIs")!, senderName, score, senderOriginalName, grade));
		return true;
	}
}
