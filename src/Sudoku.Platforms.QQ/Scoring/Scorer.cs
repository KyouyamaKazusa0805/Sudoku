namespace Sudoku.Platforms.QQ.Scoring;

/// <summary>
/// Represents a type that handles and creates the data that are used for grading and scoring.
/// </summary>
public static class Scorer
{
	/// <summary>
	/// Gets the grade via the specified score value.
	/// </summary>
	/// <param name="score">The score.</param>
	/// <returns>The grade.</returns>
	/// <exception cref="InvalidOperationException">Throws when the grade value is too large.</exception>
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

	/// <summary>
	/// Gets the factor that describes whether today is weekend.
	/// </summary>
	/// <returns>The factor value. If today is weekend, 2; otherwise 1.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetWeekendFactor() => DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 2 : 1;

	/// <summary>
	/// Generates a value that describes the experience point that the current user can be earned.
	/// </summary>
	/// <param name="continuousDaysCount">The number of continuous days that the user has already been checking-in.</param>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GenerateValueEarned(int continuousDaysCount)
	{
		var earned = GenerateOriginalValueEarned();
		var level = continuousDaysCount / 7;
		return (int)Round(earned * (level * .2 + 1)) * GetWeekendFactor();
	}

	/// <summary>
	/// Generates a value that describes the experience point that the current user can be earned.
	/// </summary>
	/// <param name="distribution">The distribution.</param>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GenerateOriginalValueEarned(Distribution distribution = Distribution.Normal)
	{
		return distribution switch
		{
			Distribution.Constant => 4,
			Distribution.Exponent => e(),
			Distribution.Normal => n(),
			_ => throw new ArgumentOutOfRangeException(nameof(distribution))
		};


		static int e()
		{
			var table = new[] { 2, 3, 4, 6, 12 };
			return getNext(table);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static int getNext(int[] table)
				=> table[
					Rng.Next(0, 10000) switch { < 5000 => 0, >= 5000 and < 7500 => 1, >= 7500 and < 8750 => 2, >= 8750 and < 9375 => 3, _ => 4 }
				];
		}

		static int n()
		{
			var sigma = 2.5;
			var mu = 0;
			var table = new[] { -1, 1, 2, 3, 4, 5, 6, 8, 10, 12, 16 };
			return getNextRandomGaussian(sigma, mu, table);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static int getNextRandomGaussian(double sigma, double mu, int[] table)
			{
				var u1 = 1.0 - Rng.NextDouble();
				var u2 = 1.0 - Rng.NextDouble();
				var target = (int)(sigma * Sqrt(-2.0 * Log(u1)) * Sin(2.0 * PI * u2) + mu + (table.Length - 1) / 2.0);
				return table[Clamp(target, 0, table.Length - 1)];
			}
		}
	}

	/// <summary>
	/// Gets the experience point that can be earned by a player in a single gaming.
	/// </summary>
	/// <param name="difficultyLevel">The difficulty level of the puzzle.</param>
	/// <returns>The experience point.</returns>
	/// <exception cref="NotSupportedException">Throws when the specified difficulty level or target cells count is not supported.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the specified difficulty level is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetScoreEarnedInEachGaming(DifficultyLevel difficultyLevel)
	{
		var @base = difficultyLevel switch
		{
			DifficultyLevel.Easy => 16,
			DifficultyLevel.Moderate => 20,
			DifficultyLevel.Hard => 28,
			_ when Enum.IsDefined(difficultyLevel) => throw new NotSupportedException("Other kinds of difficulty levels are not supported."),
			_ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel))
		};

		return @base * GetWeekendFactor();
	}

	/// <summary>
	/// Get deduct via the specified times per gaming.
	/// </summary>
	/// <param name="times">The times the user has answered with wrong result.</param>
	/// <returns>The deduct.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="times"/> is below 0.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetDeduct(int times)
		=> times switch
		{
			0 => 0,
			1 => 6,
			2 => 12,
			>= 3 => 18,
			_ => throw new ArgumentOutOfRangeException(nameof(times))
		} * GetWeekendFactor();

	/// <summary>
	/// Gets the ranking from the specified group.
	/// </summary>
	/// <param name="group">The group.</param>
	/// <param name="rankingListIsEmptyCallback">Indicates the callback method that will be raised when the ranking list is empty.</param>
	/// <returns>The list of ranking result.</returns>
	public static async Task<(string Name, UserData Data)[]?> GetUserRankingListAsync(Group @group, Func<Task> rankingListIsEmptyCallback)
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

	/// <summary>
	/// Get display string for scores.
	/// </summary>
	/// <param name="base">The base score value to be displayed.</param>
	/// <returns>The score to be displayed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string GetEarnedScoringDisplayingString(int @base)
		=> (DateTime.Today switch { { Date: { Month: 4, Day: 1 } } => int.MinValue, _ => @base }).ToString();
}
