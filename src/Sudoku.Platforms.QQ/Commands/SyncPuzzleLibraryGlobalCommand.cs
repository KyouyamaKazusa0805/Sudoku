namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines sync puzzle library global command.
/// </summary>
[Command(Permissions.Administrator, Permissions.Owner)]
file sealed class SyncPuzzleLibraryGlobalCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("SyncPuzzleLib")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var groupId = e.GroupId;
		if (InternalReadWrite.ReadLibraries(groupId) is not { } updated)
		{
			await e.SendMessageAsync(R.MessageFormat("SyncPuzzleLibFailed_NoPuzzleLibFound"));
			return true;
		}

		switch (InternalReadWrite.ReadLibraryConfiguration(groupId))
		{
			case null:
			{
				InternalReadWrite.WriteLibraryConfiguration(updated);

				await e.SendMessageAsync(R.MessageFormat("SyncPuzzleLibSuccessful"));

				break;
			}
			case { } original:
			{
				var result = new PuzzleLibraryCollection { GroupId = groupId };
				updated.PuzzleLibraries.ForEach(collectionUpdater);

				InternalReadWrite.WriteLibraryConfiguration(result);

				await e.SendMessageAsync(R.MessageFormat("SyncPuzzleLibSuccessful"));

				break;


				void collectionUpdater(PuzzleLibraryData l)
					=> result.PuzzleLibraries.Add(original.IndexOf(l) switch { var i and not -1 => original[i], _ => l });
			}
		}

		return true;
	}
}
