namespace Sudoku.Platforms.QQ;

/// <summary>
/// Extracts a type that creates data used by commands.
/// </summary>
internal interface ICommandDataProvider
{
	/// <summary>
	/// Gets the time limit for a single gaming.
	/// </summary>
	/// <param name="difficultyLevel">The difficulty level of the puzzle.</param>
	/// <returns>The time limit.</returns>
	/// <exception cref="NotSupportedException">Throws when the specified argument value is not supported.</exception>
	internal static TimeSpan GetGamingTimeLimit(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => 5.Minutes(),
			DifficultyLevel.Moderate => new TimeSpan(0, 5, 30),
			DifficultyLevel.Hard => 6.Minutes(),
			_ => throw new NotSupportedException("The specified difficulty is not supported.")
		};

	/// <summary>
	/// Gets the ranking from the specified group.
	/// </summary>
	/// <param name="group">The group.</param>
	/// <param name="rankingListIsEmptyCallback">Indicates the callback method that will be raised when the ranking list is empty.</param>
	/// <returns>The list of ranking result.</returns>
	internal static async Task<(string Name, UserData Data)[]?> GetUserRankingListAsync(Group @group, Func<Task> rankingListIsEmptyCallback)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			// This folder is special; if the computer does not contain the folder, we should return directly.
			return null;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			await rankingListIsEmptyCallback();
			return null;
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			await rankingListIsEmptyCallback();
			return null;
		}

		return (
			from file in Directory.GetFiles(botUsersDataFolder, "*.json")
			let ud = Deserialize<UserData>(File.ReadAllText(file))
			where ud is not null
			let qq = ud.QQ
			let nickname = @group.GetMatchedMemberViaIdAsync(qq).Result?.Name
			where nickname is not null
			let numericQQ = int.TryParse(qq, out var result) ? result : 0
			orderby ud.Score descending, numericQQ
			select (Name: nickname, Data: ud)
		).ToArray();
	}
}
