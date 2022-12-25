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
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { Sender: { Group: var group, Id: var senderId, Name: var senderName, MmeberProfile.NickName: var senderOriginalName } })
		{
			return false;
		}

		var targetMessage = args switch
		{
			[] => InternalReadWrite.Read(senderId) switch
			{
				{ Score: var score } userData when Scorer.GetGrade(score) is var grade
					=> string.Format(R.MessageFormat("UserScoreIs")!, senderName, score, senderOriginalName, grade),
				_ => string.Format(R.MessageFormat("UserScoreNotFound")!, senderName, senderOriginalName)
			},
			_ => await getMatchedMembers(group, args) switch
			{
				[] => R.MessageFormat("LookupNameOrIdInvalid")!,
				[{ Id: var foundMemberId }] => InternalReadWrite.Read(foundMemberId) switch
				{
					{ Score: var score } userData when Scorer.GetGrade(score) is var grade
						=> string.Format(R.MessageFormat("UserScoreIs")!, senderName, score, senderOriginalName, grade),
					_ => string.Format(R.MessageFormat("UserScoreNotFound")!, senderName, senderOriginalName)
				},
				{ Length: > 1 } => R.MessageFormat("LookupNameOrIdAmbiguous")!,
				_ => null
			}
		};

		if (targetMessage is not null)
		{
			await e.SendMessageAsync(targetMessage);
		}

		return true;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static async Task<Member[]> getMatchedMembers(Group group, string args)
			=> (from m in await @group.GetGroupMembersAsync() where new[] { m.Id, m.Name }.Any(e => e == args) select m).ToArray();
	}
}
