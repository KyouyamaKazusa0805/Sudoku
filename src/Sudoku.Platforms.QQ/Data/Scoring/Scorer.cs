namespace Sudoku.Platforms.QQ.Data.Scoring;

internal static class Scorer
{
	/// <summary>
	/// The total possibilities.
	/// </summary>
	private static readonly decimal[][] Possibilities =
	{
		new[] { 1M, 1M, 1M }, // 0
		new[] { 1M, .88M, 1M }, // 1
		new[] { .968M, .792M, .608M }, // 2
		new[] { .686M, .55M, .429M }, // 3
		new[] { .495M, .403M, .242M }, // 4
		new[] { .396M, .33M, .201M }, // 5
		new[] { .319M, .264M, .132M }, // 6
		new[] { .264M, .212M, .106M }, // 7
		new[] { .22M, .132M, .06M }, // 8
		new[] { .135M, .045M, .022M }, // 9
		new[] { .125M, .046M, .018M }, // 10
		new[] { .116M, .046M, .017M }, // 11
		new[] { .107M, .0398M, .0157M }, // 12
		new[] { .104M, .035M, .013M }, // 13
		new[] { .099M, .031M, .0108M }, // 14
		new[] { .09M, .022M, .0045M }, // 15
		new[] { .08M, .02M, .002M }, // 16
	};

	/// <summary>
	/// All clover levels.
	/// </summary>
	private static readonly decimal[] CloverLevels = { 1.1M, 1.2M, 1.4M, 1.7M, 2.0M, 2.4M, 3.0M, 3.3M, 3.6M, 4.0M };

	/// <summary>
	/// Global rate.
	/// </summary>
	private static readonly decimal[] GlobalRates = { 1.0M, 1.1M, 1.2M, 1.3M, 1.4M, 1.5M, 1.7M, 1.9M, 2.1M, 2.4M, 3.0M, 3.5M, 4.0M, 5.0M, 5.5M, 6.0M, 7.0M, 8.0M };


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
	public static decimal GetGlobalRate(int cardLevel) => GlobalRates[cardLevel];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetCheckInRate(int continuousDaysCount) => 1.0M + continuousDaysCount / 7 * .2M;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetUpLevelingSuccessPossibility(int main, int[] auxiliary, int cloverLevel)
	{
		var z = Possibilities[main];
		var p = auxiliary switch
		{
			[var c] => z[main - c],
			[var c1, var c2] => z[main - c1] + z[main - c2] / 3,
			[var c1, var c2, var c3] => z[main - c1] + z[main - c2] / 3 + z[main - c3] / 3
		};

		return Clamp(cloverLevel switch { -1 => p, _ => p * CloverLevels[cloverLevel] }, 0, 1);
	}

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
