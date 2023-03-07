namespace Sudoku.Platforms.QQ.IO;

/// <summary>
/// Represents a type that interacts with local files. All methods in this type are synchronized methods.
/// </summary>
internal static class InternalReadWrite
{
	/// <summary>
	/// Reads the specified user's local file, and returns <see cref="UserData"/> instance.
	/// </summary>
	/// <param name="userId">The user QQ number.</param>
	/// <param name="default">The default value returned.</param>
	/// <returns>
	/// The converted data. The result value of <see langword="await"/> expression has a same nullability
	/// with argument <paramref name="default"/>, which means if <paramref name="default"/> is <see langword="null"/>,
	/// the expression <c>await InternalReadWrite.ReadValueAsync(...)</c> will also be <see langword="null"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	[return: NotNullIfNotNull(nameof(@default))]
	public static UserData? Read(string userId, UserData? @default = null)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return @default;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return @default;
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			return @default;
		}

		var fileName = $"""{botUsersDataFolder}\{userId}.json""";
		return File.Exists(fileName) ? Deserialize<UserData>(File.ReadAllText(fileName)) : @default;
	}

	/// <summary>
	/// Gets the puzzle library configuration file of the target group specified as its group ID.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <returns>The group library collection.</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibraryCollection? ReadLibraryConfiguration(string groupId)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			throw new InvalidOperationException("The key path is not found.");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var filePath = $"""{groupLibraryFolder}\config.json""";
		if (!File.Exists(filePath))
		{
			return null;
		}

		var json = File.ReadAllText(filePath);
		return Deserialize<PuzzleLibraryCollection>(json);
	}

	/// <summary>
	/// Gets the puzzle library for the target group specified as its group ID.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <returns>The puzzle library data.</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibraryCollection? ReadLibraries(string groupId)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			throw new InvalidOperationException("The key path is not found.");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var final = new List<PuzzleLibraryData>();
		var di = new DirectoryInfo(groupLibraryFolder);
		foreach (var textFile in di.EnumerateFiles("*.txt"))
		{
			if (textFile is { Length: not 0, FullName: var path })
			{
				final.Add(new() { Name = Path.GetFileNameWithoutExtension(path), GroupId = groupId, PuzzleFilePath = path });
			}
		}

		return new() { GroupId = groupId, PuzzleLibraries = final };
	}

	/// <summary>
	/// Gets the puzzle library for the target group specified as its group ID, and the specified library name.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <param name="libraryName">The library name.</param>
	/// <returns>The puzzle library data.</returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static PuzzleLibraryData? ReadLibrary(string groupId, string libraryName)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			throw new InvalidOperationException("The key path is not found.");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			return null;
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			return null;
		}

		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			return null;
		}

		var di = new DirectoryInfo(groupLibraryFolder);
		return di.EnumerateFiles($"{libraryName}.txt").FirstOrDefault() switch
		{
			{ FullName: var fullName } => new() { Name = Path.GetFileNameWithoutExtension(fullName), GroupId = groupId, PuzzleFilePath = fullName },
			_ => null
		};
	}

	/// <summary>
	/// Writes the specified user's data to the local file.
	/// </summary>
	/// <param name="userData">The user data.</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void Write(UserData userData)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			throw new InvalidOperationException("The key path is not found.");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			Directory.CreateDirectory(botUsersDataFolder);
		}

		var userId = userData.QQ;
		var fileName = $"""{botUsersDataFolder}\{userId}.json""";
		File.WriteAllText(fileName, Serialize(userData));
	}

	/// <summary>
	/// Writes library data to the bot configuration file path.
	/// </summary>
	/// <param name="libraryCollection">The library collection.</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static void WriteLibraryConfiguration(PuzzleLibraryCollection libraryCollection)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			throw new InvalidOperationException("The key path is not found.");
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var libraryFolder = $"""{botDataFolder}\Puzzles""";
		if (!Directory.Exists(libraryFolder))
		{
			Directory.CreateDirectory(libraryFolder);
		}

		var groupId = libraryCollection.GroupId;
		var groupLibraryFolder = $"""{libraryFolder}\{groupId}""";
		if (!Directory.Exists(groupLibraryFolder))
		{
			Directory.CreateDirectory(groupLibraryFolder);
		}

		var fileName = $"""{groupLibraryFolder}\config.json""";
		File.WriteAllText(fileName, Serialize(libraryCollection));
	}

	/// <summary>
	/// Checks for the specified library, to get the total number of valid puzzles stored in the file.
	/// </summary>
	/// <param name="groupId">The group ID.</param>
	/// <param name="libraryName">The library name.</param>
	/// <returns>
	/// The number of valid puzzles. -1 for the case that the specified group does not store any library whose name is the specified name.
	/// </returns>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int CheckValidPuzzlesCountInPuzzleLibrary(string groupId, string libraryName)
		=> ReadLibrary(groupId, libraryName) switch { { } lib => CheckValidPuzzlesCountInPuzzleLibrary(lib), _ => -1 };

	/// <inheritdoc cref="CheckValidPuzzlesCountInPuzzleLibrary(string, string)"/>
	/// <param name="puzzleLibrary">The <see cref="PuzzleLibraryData"/> instance.</param>
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static int CheckValidPuzzlesCountInPuzzleLibrary(PuzzleLibraryData puzzleLibrary)
	{
		return File.ReadLines(puzzleLibrary.PuzzleFilePath).Count(lineValidator);


		static bool lineValidator(string line) => !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _);
	}

	/// <summary>
	/// Generate a picture using the specified sudoku painter creator, storing it to the cached folder and return its file path.
	/// This method also saves the picture into the local path.
	/// </summary>
	/// <param name="painterCreator">The painter creator method.</param>
	/// <returns>The target path of the picture.</returns>
	[SupportedOSPlatform("windows")]
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static string? GenerateCachedPicturePath(Func<ISudokuPainter> painterCreator)
	{
		var painter = painterCreator();
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return null;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var cachedPictureFolder = $"""{botDataFolder}\TempPictures""";
		if (!Directory.Exists(cachedPictureFolder))
		{
			Directory.CreateDirectory(cachedPictureFolder);
		}

		var targetPath = (string?)null;
		for (var index = 0; index < int.MaxValue; index++)
		{
			var picturePath = $"""{cachedPictureFolder}\temp{(index == 0 ? string.Empty : index.ToString())}.png""";
			if (!File.Exists(picturePath))
			{
				targetPath = picturePath;
				break;
			}
		}
		if (targetPath is null)
		{
			// Cannot find a suitable path. This case is very special.
			return null;
		}

		painter.SaveTo(targetPath);

		return targetPath;
	}
}
