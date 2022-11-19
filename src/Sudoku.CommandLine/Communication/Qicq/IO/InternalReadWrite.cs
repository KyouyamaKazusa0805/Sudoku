namespace Sudoku.Communication.Qicq.IO;

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
	/// the expression <c>await PlayerReadWrite.ReadValueAsync(...)</c> will also be <see langword="null"/>.
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

		var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
		if (!Directory.Exists(botDataFolder))
		{
			return @default;
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			return @default;
		}

		var fileName = $"""{botUsersDataFolder}\{userId}.json""";
		return File.Exists(fileName) ? Deserialize<UserData>(File.ReadAllText(fileName)) : @default;
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

		var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			Directory.CreateDirectory(botUsersDataFolder);
		}

		var userId = userData.QQ;
		var fileName = $"""{botUsersDataFolder}\{userId}.json""";
		File.WriteAllText(fileName, Serialize(userData));
	}

	/// <summary>
	/// Generate a picture using the specified sudoku painter creator, storing it to the cached folder and return its file path.
	/// This method also saves the picture into the local path.
	/// </summary>
	/// <param name="painterCreator">
	/// The painter creator method. If <see langword="null"/>,
	/// the method will automatically get <see cref="ISudokuPainter"/> instance <see cref="Painter"/>.
	/// </param>
	/// <returns>The target path of the picture.</returns>
	[SupportedOSPlatform("windows")]
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static string? GenerateCachedPicturePath(Func<ISudokuPainter>? painterCreator = null)
	{
		painterCreator ??= static () => Painter!;

		var painter = painterCreator();
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			return null;
		}

		var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var cachedPictureFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
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
