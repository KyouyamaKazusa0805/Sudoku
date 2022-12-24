namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a update puzzle library specified command.
/// </summary>
[Command(Permissions.Administrator, Permissions.Owner)]
file sealed class UpdatePuzzleLibrarySpecifiedCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("UpdateLibSpecified")!;

	/// <inheritdoc/>
	public override string[] Prefixes => CommonCommandPrefixes.HashTag;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var groupId = e.GroupId;
		var newLib = InternalReadWrite.ReadLibrary(groupId, args);
		if (newLib is null)
		{
			await e.SendMessageAsync(R.MessageFormat("UpdateLibConfigFailed_SpecifiedLibNameNotFound"));
			return true;
		}

		var collection = InternalReadWrite.ReadLibraryConfiguration(groupId);
		if (collection is null)
		{
			collection = new() { GroupId = groupId, PuzzleLibraries = new() { newLib } };
		}
		else
		{
			collection.PuzzleLibraries.Add(newLib);
		}

		InternalReadWrite.WriteLibraryConfiguration(collection);

		await e.SendMessageAsync(R.MessageFormat("UpdateLibConfigSuccessfully")!);

		return true;
	}
}
