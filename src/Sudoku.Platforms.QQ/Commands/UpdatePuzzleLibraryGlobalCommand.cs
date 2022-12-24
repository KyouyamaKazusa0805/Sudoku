namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines update puzzle library global command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator, IsDeprecated = true)]
[Obsolete("The type should be used.", false)]
file sealed class UpdatePuzzleLibraryGlobalCommand : Command
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

			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R.MessageFormat("UpdateLibGlobalCommandIsDangerous")!);
		}

		return true;
	}
}
