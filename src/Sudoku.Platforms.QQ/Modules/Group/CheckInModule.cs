namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
file sealed class CheckInModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "签到";


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		var userData = InternalReadWrite.Read(senderId, new() { QQ = senderId });
		switch (userData)
		{
			case { LastCheckIn: { Date: var date, TimeOfDay: var time } } when date == DateTime.Today:
			{
				// Disallow user checking in multiple times in a same day.
				await messageReceiver.SendMessageAsync($"签到失败~ 你今天 {time:hh' 点 'mm' 分'}的时候已经签过到了，明天再来试试吧~");

				return;
			}
			case { LastCheckIn: var dateTime } when (DateTime.Today - dateTime.Date).Days == 1:
			{
				// Continuous.
				userData.ComboCheckedIn++;

				var expEarned = LocalScorer.GetExperiencePoint(userData.ComboCheckedIn, userData.CardLevel);
				var coinEarned = LocalScorer.GetCoin(Scorer.GetGlobalRate(userData.CardLevel));
				userData.ExperiencePoint += expEarned;
				userData.Coin += coinEarned;
				userData.LastCheckIn = DateTime.Now;

				var finalScore = Scorer.GetEarnedScoringDisplayingString(expEarned);
				var finalCoin = Scorer.GetEarnedCoinDisplayingString(coinEarned);
				if (LocalScorer.GetEarnedItem() is { } earnedItem)
				{
					if (!userData.Items.TryAdd(earnedItem, 1))
					{
						userData.Items[earnedItem]++;
					}

					await messageReceiver.QuoteMessageAsync(
						$"""
						签到成功！已连续签到 {userData.ComboCheckedIn} 天~ 恭喜获得：
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
						签到成功！已连续签到 {userData.ComboCheckedIn} 天~ 恭喜获得：
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
				// Normal case.
				userData.ComboCheckedIn = 1;

				var expEarned = LocalScorer.GetExperienceOriginal();
				var coinEarned = LocalScorer.GetCoinOriginal();
				userData.ExperiencePoint += expEarned;
				userData.Coin += coinEarned;
				userData.LastCheckIn = DateTime.Now;

				var finalScore = Scorer.GetEarnedScoringDisplayingString(expEarned);
				var finalCoin = Scorer.GetEarnedCoinDisplayingString(coinEarned);
				if (LocalScorer.GetEarnedItem() is { } earnedItem)
				{
					if (!userData.Items.TryAdd(earnedItem, 1))
					{
						userData.Items[earnedItem]++;
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

		InternalReadWrite.Write(userData);
	}
}

file static class LocalScorer
{
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoinOriginal() => 24 + Rng.Next(-6, 7);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExperiencePoint(int continuousDaysCount, int cardLevel)
		=> (int)Round(GetExperienceOriginal() * (Scorer.GetCheckInRate(continuousDaysCount) + Scorer.GetGlobalRate(cardLevel))) * Scorer.GetWeekendFactor();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCoin(decimal rate) => (int)Round(GetCoinOriginal() * Scorer.GetWeekendFactor() * rate);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ShoppingItem? GetEarnedItem()
		=> Rng.Next(0, 10000) switch
		{
			< 400 => ShoppingItem.CloverLevel4,
			< 1000 => ShoppingItem.CloverLevel3,
			< 2500 => ShoppingItem.CloverLevel2,
			< 4000 => ShoppingItem.CloverLevel1,
			< 8000 => ShoppingItem.Card,
			_ => null
		};
}
