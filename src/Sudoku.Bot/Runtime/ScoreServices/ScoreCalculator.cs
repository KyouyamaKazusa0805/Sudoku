namespace Sudoku.Runtime.ScoreServices;

/// <summary>
/// 分数计算器。
/// </summary>
public static class ScoreCalculator
{
	/// <summary>
	/// 获取当天是否是周末。
	/// </summary>
	public static bool TodayIsWeekend() => DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

	/// <summary>
	/// 计算签到的经验值，按峰值为 5 的正态分布求随机结果。
	/// </summary>
	public static int GetCheckInExperienceValue(this Random @this)
	{
		var sigma = 2.5;
		var mu = 0;
		var table = new[] { -1, 1, 2, 3, 4, 5, 6, 8, 10, 12, 16 };
		return getNextRandomGaussian(sigma, mu, table);


		int getNextRandomGaussian(double sigma, double mu, int[] table)
		{
			var u1 = 1.0 - @this.NextDouble();
			var u2 = 1.0 - @this.NextDouble();
			var target = (int)(sigma * Sqrt(-2.0 * Log(u1)) * Sin(2.0 * PI * u2) + mu + (table.Length - 1) / 2.0);
			return table[Clamp(target, 0, table.Length - 1)];
		}
	}

	/// <summary>
	/// 计算签到的金币。
	/// </summary>
	public static int GetCheckInCoinValue(this Random @this) => 24 + @this.Next(-15, 16);

	/// <summary>
	/// 根据连续答错的次数确定扣的经验值。
	/// </summary>
	/// <param name="times">同一个题目答错的次数。</param>
	/// <returns>扣除的经验值。</returns>
	/// <exception cref="ArgumentOutOfRangeException">如果 <paramref name="times"/> 为负数，就会产生此异常。</exception>
	public static int GetExperiencePointDeduct(int times)
		=> times switch
		{
			0 => 0,
			1 => 6,
			2 => 12,
			>= 3 => 18,
			_ => throw new ArgumentOutOfRangeException(nameof(times))
		} * (TodayIsWeekend() ? 2 : 1);

	/// <summary>
	/// 获取连续天数所对应的额外倍率。
	/// </summary>
	public static decimal GetComboDaysRate(int comboDays) => 1 + comboDays / 7M * .2M;
}
