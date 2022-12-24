namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines show library collection data command.
/// </summary>
[Command]
file sealed class ShowLibraryCollectionCommand : Command
{
	/// <summary>
	/// Indicates the object that is used for synchronization.
	/// </summary>
	private static readonly object SyncRoot = new();


	/// <inheritdoc/>
	public override string CommandName => R.Command("DisplayLib")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var groupId = e.GroupId;
		if (InternalReadWrite.ReadLibraryConfiguration(groupId) is not { Count: var libCount and not 0 } libs)
		{
			await e.SendMessageAsync(R.MessageFormat("PuzzleLibraryIsNullOrEmpty")!);
			return true;
		}

		switch (args)
		{
			case []:
			{
				await e.SendMessageAsync(
					string.Format(R.MessageFormat("PuzzleLibGlobalInfo")!, libCount, string.Join('\u3001', from lib in libs select lib.Name))
				);

				break;
			}
			default:
			{
				var lib = libs.PuzzleLibraries.FirstOrDefault(lib => lib.Name == args);
				if (lib is null)
				{
					await e.SendMessageAsync(string.Format(R.MessageFormat("NoSpecifiedLibExists")!, args));
					return true;
				}

				var validPuzzlesCount = InternalReadWrite.CheckValidPuzzlesCountInPuzzleLibrary(lib);
				await e.SendMessageAsync(
					string.Format(
						R.MessageFormat("PuzzleLibSpecifiedInfo")!,
						lib.Name,
						lib.Description switch { var d => string.IsNullOrWhiteSpace(d) ? R["None"]! : d },
						lib.Tags switch { null or [] => R["None"]!, var tags => string.Join('\u3001', tags) },
						validPuzzlesCount,
						lib.FinishedPuzzlesCount,
						(double)lib.FinishedPuzzlesCount / validPuzzlesCount
					)
				);

				break;
			}
		}

		return true;
	}
}
