namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
file sealed class CardUplevelingModule : GroupModule
{
#pragma warning disable CS0414
	private static readonly int CloverLevelDefaultValue = -1;
	private static readonly int MainCardLevelDefaultValue = -1;
#pragma warning restore CS0414


	/// <inheritdoc/>
	public override string RaisingCommand => "强化";

	/// <summary>
	/// Indicates the clover level.
	/// </summary>
	[DoubleArgumentCommand("三叶草")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue(nameof(CloverLevelDefaultValue))]
	public int CloverLevel { get; set; }

	/// <summary>
	/// Indicates the main card level.
	/// </summary>
	[DoubleArgumentCommand("主卡")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue(nameof(MainCardLevelDefaultValue))]
	public int MainCardLevel { get; set; }

	/// <summary>
	/// Indicates the auxiliary cards.
	/// </summary>
	[DoubleArgumentCommand("辅助")]
	[ValueConverter<NumericArrayConverter<int>>]
	public int[]? AuxiliaryCards { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		if (InternalReadWrite.Read(senderId) is not { ExperiencePoint: var exp, CardLevel: var userCardLevel, Coin: var coin } user)
		{
			await messageReceiver.SendMessageAsync("很抱歉，你尚未使用过机器人。强化系统至少要求用户达到 25 级。");
			return;
		}

		if (Scorer.GetGrade(exp) < 25)
		{
			await messageReceiver.SendMessageAsync("很抱歉，强化系统至少要求用户达到 25 级。");
			return;
		}

		if (coin < 30)
		{
			await messageReceiver.SendMessageAsync("强化一次需消耗 30 金币。金币不足，无法强化。");
			return;
		}

		switch (this)
		{
			case { AuxiliaryCards: null }:
			{
				await messageReceiver.SendMessageAsync("参数有误。辅助卡至少需要一张。请给出强化等级数值。所有的辅助卡片，可使用“！查询”指令查询。");
				break;
			}
			case { AuxiliaryCards: { Length: > 3 } cards }:
			{
				await messageReceiver.SendMessageAsync("参数有误。强化系统最多只能上三张辅助卡。请检查参数输入，如“！强化 辅助 2，2，2”。");
				break;
			}
			case { MainCardLevel: < -1 or >= 17 }:
			{
				await messageReceiver.SendMessageAsync("参数有误。主卡级别必须介于 0 到 17 之间，且不包含 17。");
				break;
			}
			case { AuxiliaryCards: { } cards, CloverLevel: var level, MainCardLevel: var main }:
			{
				if (level is < -1 or > 10)
				{
					await messageReceiver.SendMessageAsync("参数有误。三叶草等级必须介于 0 到 10 之间，且请优先确保你拥有当前等级的三叶草。");
					break;
				}

				await uplevelAsync(messageReceiver, main == -1 ? userCardLevel : main);

				break;
			}


			static bool lackingOfCardsChecker(KeyValuePair<int, int> kvp) => kvp.Value < 0;

			async Task uplevelAsync(GroupMessageReceiver messageReceiver, int main)
			{
				var copied = await precheckWhetherCardsAreEnoughAsync(user, cards, main);
				if (copied is null)
				{
					return;
				}

				var possibility = Scorer.GetUpLevelingSuccessPossibility(main, cards, level);

				user.Coin -= 30;

				var final = Rng.Next(0, 10000);
				var boundary = possibility * 10000;

				if (final < boundary)
				{
					// Success.
					main++;

					user.Items.Remove(ShoppingItem.Card);

					if (!copied.TryAdd(main, 1))
					{
						copied[main]++;
					}
					user.UplevelingCards = copied;

					InternalReadWrite.Write(user);

					await messageReceiver.SendMessageAsync($"恭喜你，强化成功！卡片等级变动：{main - 1} -> {main}！");
				}
				else
				{
					// Failed.
					var originalLevel = main;
					if (main > 5)
					{
						main--;
					}

					user.Items.Remove(ShoppingItem.Card);

					if (!copied.TryAdd(main, 1))
					{
						copied[main]++;
					}
					user.UplevelingCards = copied;

					InternalReadWrite.Write(user);

					await messageReceiver.SendMessageAsync(
						originalLevel switch
						{
							> 5 => $"不够好运，强化失败。卡片等级降级：{originalLevel} -> {originalLevel - 1}。",
							_ => "不够好运，强化失败。卡片小于 5 级不掉级。"
						}
					);
				}
			}

			async Task<Dictionary<int, int>?> precheckWhetherCardsAreEnoughAsync(User user, int[] cards, int main)
			{
				var copied = new Dictionary<int, int>(user.UplevelingCards);
				if (user.Items.TryGetValue(ShoppingItem.Card, out var basicCardsCount) && !copied.TryAdd(0, basicCardsCount))
				{
					copied[0] += basicCardsCount;
				}

				if (!copied.ContainsKey(main))
				{
					await messageReceiver.SendMessageAsync($"你尚未拥有需要强化的级别 {main} 的卡片。请检查输入。");
					return null;
				}

				copied[main]--;

				for (var trial = 0; trial < Min(3, cards.Length); trial++)
				{
					var currentCard = cards[trial];

					if (!copied.ContainsKey(currentCard))
					{
						await messageReceiver.SendMessageAsync($"你的强化辅助卡之中不包含 {currentCard} 级别的卡片。请检查输入。");
						return null;
					}

					copied[currentCard]--;
				}

				if (copied.Any(lackingOfCardsChecker))
				{
					var (key, _) = copied.First(lackingOfCardsChecker);
					await messageReceiver.SendMessageAsync($"强化辅助卡级别为 {key} 不够使用。请重新调整卡片等级。");
					return null;
				}

				return copied;
			}
		}
	}
}
