namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines update library property command.
/// </summary>
[Command(Permissions.Owner, Permissions.Administrator)]
file sealed class UpdatePuzzleLibraryPropertyCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("UpdatePuzzleLibProp")!;

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
			when possibleRevert == R.CommandSegment("Revert")!
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
				if (propName == R.CommandSegment(nameof(PuzzleLibraryData.Description))!)
				{
					lib.Description = propValue;

					await updateLibConfigFile(libs);
				}
				else if (propName == R.CommandSegment(nameof(PuzzleLibraryData.Tags))!)
				{
					var tags = split(propValue, ',', '\uff0c', ';', '\uff1b', '\u3001') switch { [] => null, var t => t };

					lib.Tags = tags;

					await updateLibConfigFile(libs);
				}
				else if (propName == R.CommandSegment(nameof(PuzzleLibraryData.FinishedPuzzlesCount))!)
				{
					if (int.TryParse(propValue, out var finishedPuzzlesCount))
					{
						if (finishedPuzzlesCount >= 0 && finishedPuzzlesCount < InternalReadWrite.CheckValidPuzzlesCountInPuzzleLibrary(lib))
						{
							lib.FinishedPuzzlesCount = finishedPuzzlesCount;

							await updateLibConfigFile(libs);
						}
						else
						{
							await e.SendMessageAsync(
								string.Format(
									R.MessageFormat("UpdateLibPropFailed_ValueMustLowerThanLibPuzzlesCount")!,
									libraryName,
									propName
								)
							);
						}
					}
					else
					{
						await e.SendMessageAsync(
							string.Format(
								R.MessageFormat("UpdateLibPropFailed_ValueMustBeInteger")!,
								libraryName,
								propName
							)
						);
					}
				}

				break;


				async Task updateLibConfigFile(PuzzleLibraryCollection libs)
				{
					InternalReadWrite.WriteLibraryConfiguration(libs);

					await e.SendMessageAsync(string.Format(R.MessageFormat("UpdateLibPropertySuccessfully")!, libraryName, propName));
				}
			}


			static PuzzleLibraryData? getFirstMatch(PuzzleLibraryCollection libraries, string libraryName)
			{
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
