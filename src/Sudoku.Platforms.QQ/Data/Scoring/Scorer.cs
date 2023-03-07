namespace Sudoku.Platforms.QQ.Data.Scoring;

internal static class Scorer
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetGrade(int score)
	{
		return score switch { 0 or 1 => 1, >= 2 and < 7 => 2, >= 7 and < 15 => 3, _ => g(score) };


		static int g(int score)
		{
			foreach (var (level, threshold) in f())
			{
				if (score < threshold)
				{
					return level - 1;
				}
			}

			throw new InvalidOperationException("The grade value is too large.");
		}

		static IEnumerable<(int Level, int Threshold)> f()
		{
			// A000217(41449) is the last and biggest value that is lower than int.MaxValue.
			for (var levelCurrent = 1; levelCurrent <= 41449; levelCurrent++)
			{
				yield return (levelCurrent, (int)(A000217(levelCurrent) * 2.5F));
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetGlobalRate(int cardLevel)
		=> cardLevel switch
		{
			>= 0 and <= 5 => 1.0M + cardLevel * .1M,
			6 => 1.7M,
			7 => 1.9M,
			8 => 2.1M,
			9 => 2.4M,
			10 => 3.0M,
			11 => 3.5M,
			12 => 4.0M,
			13 => 5.0M,
			14 => 5.5M,
			15 => 6.0M,
			16 => 7.0M,
			17 => 8.0M,
			_ => throw new ArgumentOutOfRangeException(nameof(cardLevel))
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetCheckInRate(int continuousDaysCount) => 1.0M + continuousDaysCount / 7 * .2M;

	public static async Task<(string Name, User Data)[]?> GetUserRankingListAsync(Group @group, Func<Task> rankingListIsEmptyCallback)
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
			let ud = Deserialize<User>(File.ReadAllText(file))
			where ud is not null
			let qq = ud.QQ
			let nickname = @group.GetMatchedMemberViaIdAsync(qq).Result?.Name
			where nickname is not null
			let numericQQ = int.TryParse(qq, out var result) ? result : 0
			orderby ud.ExperiencePoint descending, numericQQ
			select (Name: nickname, Data: ud)
		).ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetEarnedScoringDisplayingString(int @base)
		=> (DateTime.Today switch { { Date: { Month: 4, Day: 1 } } => int.MinValue, _ => @base }).ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetEarnedCoinDisplayingString(int @base)
		=> (DateTime.Today switch { { Date: { Month: 4, Day: 1 } } => int.MinValue, _ => @base }).ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetWeekendFactor() => DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 2 : 1;
}
