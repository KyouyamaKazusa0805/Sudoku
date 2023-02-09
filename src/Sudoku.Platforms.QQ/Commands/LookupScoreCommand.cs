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
			[] => await group.GetMemberFromQQAsync(senderId) switch
			{
				{ Name: var nickname } => InternalReadWrite.Read(senderId) switch
				{
					{ Score: var score } userData => await validMessage(senderName, score, nickname, Scorer.GetGrade(score)),
					_ => notFoundMessage(senderName, nickname)
				},
				_ => null
			},
			_ => await getMatchedMembers(group, args) switch
			{
				[] => R.MessageFormat("LookupNameOrIdInvalid")!,
				[{ Name: var foundMemberName, Id: var foundMemberId }] => await group.GetMemberFromQQAsync(foundMemberId) switch
				{
					{ Name: var nickname } => InternalReadWrite.Read(foundMemberId) switch
					{
						{ Score: var score } userData => await validMessage(foundMemberName, score, nickname, Scorer.GetGrade(score)),
						_ => notFoundMessage(foundMemberName, nickname)
					},
					_ => null
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
		async Task<string> validMessage(string name, int score, string nickname, int grade)
		{
			var ranking = (await ICommandDataProvider.GetUserRankingListAsync(group, rankingEmptyCallback))!;
			var rankingIndex = int.MaxValue;
			for (var i = 0; i < ranking.Length; i++)
			{
				if (ranking[i].Data.QQ == senderId)
				{
					rankingIndex = i + 1;
					break;
				}
			}

			return string.Format(R.MessageFormat("UserScoreIs")!, name, score, nickname, grade, rankingIndex);


			async Task rankingEmptyCallback() => await e.SendMessageAsync(R.MessageFormat("RankingListIsEmpty")!);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string notFoundMessage(string name, string nickname) => string.Format(R.MessageFormat("UserScoreNotFound")!, name, nickname);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static async Task<Member[]> getMatchedMembers(Group group, string args)
			=> (from m in await @group.GetGroupMembersAsync() where new[] { m.Id, m.Name }.Any(e => e == args) select m).ToArray();
	}
}
