namespace Sudoku.Workflow.Bot.Oicq.Handlers;

/// <summary>
/// 提供一个分数（经验值和金币）的处理器类型。
/// </summary>
public static class ScoreHandler
{
	/// <summary>
	/// 强化系统的成功率表。
	/// </summary>
	private static readonly decimal[,] Possibilities =
	{
		{ 1M, 1M, 1M }, // 0
		{ 1M, .88M, 1M }, // 1
		{ .968M, .792M, .608M }, // 2
		{ .686M, .55M, .429M }, // 3
		{ .495M, .403M, .242M }, // 4
		{ .396M, .33M, .201M }, // 5
		{ .319M, .264M, .132M }, // 6
		{ .264M, .212M, .106M }, // 7
		{ .22M, .132M, .06M }, // 8
		{ .135M, .045M, .022M }, // 9
		{ .125M, .046M, .018M }, // 10
		{ .116M, .046M, .017M }, // 11
		{ .107M, .0398M, .0157M }, // 12
		{ .104M, .035M, .013M }, // 13
		{ .099M, .031M, .0108M }, // 14
		{ .09M, .022M, .0045M }, // 15
		{ .08M, .02M, .002M }, // 16
	};

	/// <summary>
	/// 三叶草的成功率翻倍表。
	/// </summary>
	private static readonly decimal[] CloverLevels = { 1.1M, 1.2M, 1.4M, 1.7M, 2.0M, 2.4M, 3.0M, 3.3M, 3.6M, 4.0M };

	/// <summary>
	/// 强化系统里，强化卡的对应级别，以及全局的加成倍数表。
	/// </summary>
	private static readonly decimal[] GlobalRates =
	{
		1.0M, 1.1M, 1.2M, 1.3M, 1.4M, 1.5M, 1.7M, 1.9M, 2.1M,
		2.4M, 3.0M, 3.5M,
		4.0M, 5.0M, 5.5M, 6.0M, 7.0M, 8.0M
	};


	/// <summary>
	/// 根据经验值判断用户的级别。
	/// </summary>
	/// <param name="score">经验值。</param>
	/// <returns>用户的级别。</returns>
	/// <exception cref="InvalidOperationException">如果分数过大，就会产生此异常。</exception>
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

			throw new InvalidOperationException("等级过高。");
		}

		static IEnumerable<(int Level, int Threshold)> f()
		{
			// A000217(41449) 是最大的、比 int.MaxValue（2147483647）小的数字。
			for (var levelCurrent = 1; levelCurrent <= 41449; levelCurrent++)
			{
				yield return (levelCurrent, (int)(A000217(levelCurrent) * 2.5F));
			}
		}
	}

	/// <summary>
	/// 根据用户提供的卡片级别，求得用户的卡片加成倍数。
	/// </summary>
	/// <param name="cardLevel">卡片级别。</param>
	/// <returns>用户卡片加成倍数。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetGlobalRate(int cardLevel) => GlobalRates[cardLevel];

	/// <summary>
	/// 根据用户的连续签到天数，求得用户的连续签到的倍数。
	/// </summary>
	/// <param name="continuousDaysCount">连续签到的天数。</param>
	/// <returns>用户的签到倍数。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetCheckInRate(int continuousDaysCount) => 1.0M + continuousDaysCount / 7 * .2M;

	/// <summary>
	/// 根据用户提供的主卡级别、辅助卡级别和三叶草级别，求得卡片强化的成功率。
	/// </summary>
	/// <param name="main">主卡级别。</param>
	/// <param name="auxiliary">辅助卡级别。辅助卡最少一张，最多三张。</param>
	/// <param name="cloverLevel">三叶草的级别。如果不需要三叶草，则传 -1。</param>
	/// <returns>强化的成功率。</returns>
	/// <exception cref="ArgumentException">如果辅助卡为空数组，或超过 3 个元素，则产生此异常。</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal GetUpLevelingSuccessPossibility(int main, int[] auxiliary, int cloverLevel)
	{
		var p = auxiliary switch
		{
			[]
				=> throw new ArgumentException("辅助卡至少一张。", nameof(auxiliary)),
			[var c]
				=> Possibilities[main, main - c],
			[var c1, var c2]
				=> Possibilities[main, main - c1] + Possibilities[main, main - c2] / 3,
			[var c1, var c2, var c3]
				=> Possibilities[main, main - c1] + Possibilities[main, main - c2] / 3 + Possibilities[main, main - c3] / 3,
			_
				=> throw new ArgumentException("辅助卡最多三张。", nameof(auxiliary))
		};

		return Clamp(cloverLevel switch { -1 => p, _ => p * CloverLevels[cloverLevel] }, 0, 1);
	}

	/// <summary>
	/// 求得群的排名信息。
	/// </summary>
	/// <param name="group">所在群。</param>
	/// <param name="rankingListIsEmptyCallback">一个回调函数。如果本群没有人使用过机器人，该回调函数会触发。</param>
	/// <returns>一个 <see cref="Task{T}"/> 实例，表示该异步执行的逻辑数据，并带出计算结果。</returns>
	public static async Task<(string Name, User Data)[]?> GetUserRankingListAsync(Group @group, Func<Task> rankingListIsEmptyCallback)
	{
		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
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
			let qq = ud.Number
			let nickname = @group.GetMatchedMemberViaIdAsync(qq).Result?.Name
			where nickname is not null
			let numericQQ = int.TryParse(qq, out var result) ? result : 0
			orderby ud.ExperiencePoint descending, numericQQ
			select (Name: nickname, Data: ud)
		).ToArray();
	}

	/// <summary>
	/// 获取一个字符串，表示的是数值的字符串写法；如果是愚人节当天，则故意显示为 <see cref="int.MinValue"/> 的字符串写法，作为一个玩笑。
	/// </summary>
	/// <param name="base">基础数值。</param>
	/// <returns>字符串写法。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetEarnedScoringDisplayingString(int @base)
		=> (DateTime.Today switch { { Date: { Month: 4, Day: 1 } } => int.MinValue, _ => @base }).ToString();

	/// <inheritdoc cref="GetEarnedScoringDisplayingString(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetEarnedCoinDisplayingString(int @base)
		=> (DateTime.Today switch { { Date: { Month: 4, Day: 1 } } => int.MinValue, _ => @base }).ToString();

	/// <summary>
	/// 获取周末的翻倍情况。如果当天不是周末，则返回 1；否则返回 2。
	/// </summary>
	/// <returns>周末翻倍情况。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetWeekendFactor() => DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 2 : 1;
}
