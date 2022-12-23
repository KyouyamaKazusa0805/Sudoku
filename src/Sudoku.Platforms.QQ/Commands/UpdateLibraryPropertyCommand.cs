namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines update library property command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator)]
file sealed class UpdateLibraryPropertyCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => "updatelib prop";

	/// <inheritdoc/>
	public override string[] Prefixes => CommonCommandPrefixes.HashTag;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var groupId = e.GroupId;
		switch (split(args, ' '))
		{
			case [var libraryName, var possibleRevert]
			when propertyNameMatch(possibleRevert, "revert")
				&& InternalReadWrite.ReadLibraryConfiguration(groupId) is { } libs && getFirstMatch(libs, libraryName) is { } lib:
			{
				if (lib.FinishedPuzzlesCount > 0)
				{
					lib.FinishedPuzzlesCount--;

					InternalReadWrite.WriteLibraryConfiguration(libs);

					await e.SendMessageAsync(string.Format(R.MessageFormat("CurrentLibProgressIsReverted")!, libraryName));
				}
				else
				{
					await e.SendMessageAsync(string.Format(R.MessageFormat("CurrentLibCannotBeReverted")!, libraryName));
				}

				break;
			}

			case [var libraryName, var propName, var propValue]
			when InternalReadWrite.ReadLibraryConfiguration(groupId) is { } libs && getFirstMatch(libs, libraryName) is { } lib:
			{
				if (propertyNameMatch(propName, nameof(PuzzleLibraryData.Description)))
				{
					lib.Description = propValue;

					await updateLibConfigFile(libs);
				}
				else if (propertyNameMatch(propName, nameof(PuzzleLibraryData.Tags)))
				{
					var tags = split(propValue, ',', '\uff0c', ';', '\uff1b') switch { [] => null, var t => t };

					lib.Tags = tags;

					await updateLibConfigFile(libs);
				}

				break;


				async Task updateLibConfigFile(PuzzleLibraryCollection libs)
				{
					InternalReadWrite.WriteLibraryConfiguration(libs);

					await e.SendMessageAsync(string.Format(R.MessageFormat("UpdateLibPropertySuccessfully")!, libraryName, propName));
				}
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool propertyNameMatch(string propName, string nameToBeMatched)
				=> propName.Equals(nameToBeMatched, StringComparison.OrdinalIgnoreCase);

			static PuzzleLibraryData? getFirstMatch(PuzzleLibraryCollection libraries, string libraryName)
			{
				if (libraries is null)
				{
					return null;
				}

				foreach (var library in libraries)
				{
					if (library.Name == libraryName)
					{
						return library;
					}
				}

				return null;
			}
		}

		return true;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string[] split(string str, params char[] chars)
			=> str.Split(chars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	}
}
