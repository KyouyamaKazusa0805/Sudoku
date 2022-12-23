namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines manual update library command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator)]
file sealed class ManualUpdateLibraryCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("ManualUpdatePuzzleLib")!;

	/// <inheritdoc/>
	public override string[] Prefixes => CommonCommandPrefixes.HashTag;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (InternalReadWrite.ReadLibraries(e.GroupId) is { } libs and not [])
		{
			InternalReadWrite.WriteLibraryConfiguration(libs);

			await e.SendMessageAsync(R.MessageFormat("UpdateLibConfigSuccessfully")!);
		}

		return true;
	}
}
