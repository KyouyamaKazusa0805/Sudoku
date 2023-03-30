namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 强化指令。
/// </summary>
[Command("强化")]
[RequiredUserLevel(25)]
internal sealed class UplevelCommand : Command
{
	/// <summary>
	/// 表示强化期间，需要的三叶草的级别，支持 1 到 10 级。该参数可以没有，默认情况下表示不带三叶草进行强化。
	/// </summary>
	[DoubleArgument("三叶草")]
	[Hint("表示强化期间，需要的三叶草的级别，支持 1 到 10 级。该参数可以没有，默认情况下表示不带三叶草进行强化。")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue<int>(-1)]
	[DisplayingIndex(2)]
	public int CloverLevel { get; set; }

	/// <summary>
	/// 表示你需要强化的主卡级别，支持 1 到 16 级。该参数可以没有，默认情况下表示你拥有的强化卡里最高级别的那一张作为主卡。
	/// </summary>
	[DoubleArgument("主卡")]
	[Hint("表示你需要强化的主卡级别，支持 1 到 16 级。该参数可以没有，默认情况下表示你拥有的强化卡里最高级别的那一张作为主卡。")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue<int>(-1)]
	[DisplayingIndex(0)]
	public int MainCardLevel { get; set; }

	/// <summary>
	/// 表示辅助卡片的级别。每一个卡片都使用逗号分隔，中间没有空格。辅助卡片至少一张，最多三张。每张辅助卡的级别都不能超过主卡级别，也不能低于主卡级别 3 级及以上。
	/// </summary>
	[DoubleArgument("辅助")]
	[Hint("表示辅助卡片的级别。每一个卡片都使用逗号分隔，中间没有空格。辅助卡片至少一张，最多三张。每张辅助卡的级别都不能超过主卡级别，也不能低于主卡级别 3 级及以上。")]
	[ValueConverter<NumericArrayConverter<int>>]
	[DisplayingIndex(1)]
	public int[]? AuxiliaryCards { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		if (UserOperations.Read(senderId) is not { CardLevel: var userCardLevel, Coin: var coin } user)
		{
			await messageReceiver.SendMessageAsync("很抱歉，你尚未使用过机器人。强化系统至少要求用户达到 25 级。");
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


				static bool lackingOfCardsChecker(KeyValuePair<int, int> kvp) => kvp.Value < 0;

				async Task uplevelAsync(GroupMessageReceiver messageReceiver, int main)
				{
					var copied = await precheckWhetherCardsAreEnoughAsync(user, cards, main);
					if (copied is null)
					{
						return;
					}

					if (Array.Exists(cards, card => main - card >= 3))
					{
						await messageReceiver.SendMessageAsync("抱歉，你的主卡级别比辅助卡级别差距超过 3 级，不允许这种强化。");
						return;
					}

					if (Array.Exists(cards, card => main - card < 0))
					{
						await messageReceiver.SendMessageAsync("抱歉，你的辅助卡级别比主卡级别还要高，不允许这种强化。");
						return;
					}

					var possibility = ScoringOperation.GetUpLevelingSuccessPossibility(main, cards, level);

					user.Coin -= 30;

					var final = Rng.Next(0, 10000);
					var boundary = possibility * 10000;

					if (final < boundary)
					{
						// Success.
						main++;

						user.Items.Remove(Item.Card);

						if (!copied.TryAdd(main, 1))
						{
							copied[main]++;
						}
						user.UplevelingCards = copied;

						UserOperations.Write(user);

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

						user.Items.Remove(Item.Card);

						if (!copied.TryAdd(main, 1))
						{
							copied[main]++;
						}
						user.UplevelingCards = copied;

						UserOperations.Write(user);

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
					if (user.Items.TryGetValue(Item.Card, out var basicCardsCount) && !copied.TryAdd(0, basicCardsCount))
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
}
