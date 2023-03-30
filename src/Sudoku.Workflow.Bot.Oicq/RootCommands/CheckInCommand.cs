namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

[Command("签到")]
internal sealed class CheckInCommand : Command
{
	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		var user = UserOperations.Read(senderId, new() { Number = senderId });
		switch (user)
		{
			case { LastCheckIn: { Date: var date, TimeOfDay: var time } } when date == DateTime.Today:
			{
				// 禁止用户同一天签到多次。
				await messageReceiver.SendMessageAsync($"签到失败~ 你今天 {time:hh' 点 'mm' 分'}的时候已经签过到了，明天再来试试吧~");

				return;
			}
			case { LastCheckIn: var dateTime } when (DateTime.Today - dateTime.Date).Days == 1:
			{
				// 连续签到。
				user.ComboCheckedIn++;

				var expEarned = LocalScorer.GetExperiencePoint(user.ComboCheckedIn, user.CardLevel);
				var coinEarned = LocalScorer.GetCoin(ScoringOperation.GetGlobalRate(user.CardLevel));
				user.ExperiencePoint += expEarned;
				user.Coin += coinEarned;
				user.LastCheckIn = DateTime.Now;

				var finalScore = ScoringOperation.GetEarnedScoringDisplayingString(expEarned);
				var finalCoin = ScoringOperation.GetEarnedCoinDisplayingString(coinEarned);
				if (LocalScorer.GetItem() is { } earnedItem)
				{
					if (!user.Items.TryAdd(earnedItem, 1))
					{
						user.Items[earnedItem]++;
					}

					await messageReceiver.QuoteMessageAsync(
						$"""
						签到成功！已连续签到 {user.ComboCheckedIn} 天~ 恭喜获得：
						* {finalScore} 经验值
						* {finalCoin} 金币
						* {earnedItem.GetName()} * 1
						---
						一天只能签到一次哦~
						"""
					);
				}
				else
				{
					await messageReceiver.QuoteMessageAsync(
						$"""
						签到成功！已连续签到 {user.ComboCheckedIn} 天~ 恭喜获得：
						* {finalScore} 经验值
						* {finalCoin} 金币
						---
						一天只能签到一次哦~
						"""
					);
				}

				break;
			}
			default:
			{
				// 断签，或者这个人头一回用机器人签到。
				user.ComboCheckedIn = 1;

				var expEarned = LocalScorer.GetExperienceOriginal();
				var coinEarned = LocalScorer.GetCoinOriginal();
				user.ExperiencePoint += expEarned;
				user.Coin += coinEarned;
				user.LastCheckIn = DateTime.Now;

				var finalScore = ScoringOperation.GetEarnedScoringDisplayingString(expEarned);
				var finalCoin = ScoringOperation.GetEarnedCoinDisplayingString(coinEarned);
				if (LocalScorer.GetItem() is { } earnedItem)
				{
					if (!user.Items.TryAdd(earnedItem, 1))
					{
						user.Items[earnedItem]++;
					}

					await messageReceiver.QuoteMessageAsync(
						$"""
						签到成功！恭喜获得：
						* {finalScore} 经验值
						* {finalCoin} 金币
						* {earnedItem.GetName()} * 1
						---
						一天只能签到一次哦~
						"""
					);
				}
				else
				{
					await messageReceiver.QuoteMessageAsync(
						$"""
						签到成功！恭喜获得：
						* {finalScore} 经验值
						* {finalCoin} 金币
						---
						一天只能签到一次哦~
						"""
					);
				}

				break;
			}
		}

		UserOperations.Write(user);
	}
}

/// <summary>
/// 本地的计分规则的方法。
/// </summary>
file static class LocalScorer
{
	/// <summary>
	/// 获取经验值获得。
	/// </summary>
	/// <param name="distribution">使用的随机数分布规则。</param>
	/// <returns>经验值获得。</returns>
	/// <exception cref="ArgumentOutOfRangeException">如果 <paramref name="distribution"/> 没有定义在类型里，就会产生此异常。</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperienceOriginal(Distribution distribution = Distribution.Normal)
	{
		return distribution switch
		{
			Distribution.Constant => 4,
			Distribution.Exponent => e(),
			Distribution.Normal => n(),
			_ => throw new ArgumentOutOfRangeException(nameof(distribution))
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int e()
			=> new[] { 2, 3, 4, 6, 12 }[
				Rng.Next(0, 10000) switch
				{
					< 5000 => 0,
					>= 5000 and < 7500 => 1,
					>= 7500 and < 8750 => 2,
					>= 8750 and < 9375 => 3,
					_ => 4
				}
			];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	/// 获得金币数。
	/// </summary>
	/// <returns>金币数。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoinOriginal() => 240 + Rng.Next(-6, 7);

	/// <summary>
	/// 获得经验值。
	/// </summary>
	/// <param name="continuousDaysCount">连续签到天数。</param>
	/// <param name="cardLevel">用户的强化卡级别。</param>
	/// <returns>经验值。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperiencePoint(int continuousDaysCount, int cardLevel)
		=> (int)Round(GetExperienceOriginal() * (ScoringOperation.GetCheckInRate(continuousDaysCount) + ScoringOperation.GetGlobalRate(cardLevel)))
			* ScoringOperation.GetWeekendFactor();

	/// <summary>
	/// 获得金币。
	/// </summary>
	/// <param name="rate">加成倍率。</param>
	/// <returns>金币。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoin(decimal rate) => (int)Round(GetCoinOriginal() * ScoringOperation.GetWeekendFactor() * rate);

	/// <summary>
	/// 获得的物品。
	/// </summary>
	/// <returns>获得的物品。可能为 <see langword="null"/>。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Item? GetItem()
		=> Rng.Next(0, 10000) switch
		{
			< 400 => Item.CloverLevel4,
			< 1000 => Item.CloverLevel3,
			< 2500 => Item.CloverLevel2,
			< 4000 => Item.CloverLevel1,
			< 8000 => Item.Card,
			_ => null
		};
}
