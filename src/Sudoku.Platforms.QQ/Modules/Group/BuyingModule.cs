namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
file sealed class BuyingModule : GroupModule
{
#pragma warning disable CS0414
	private static readonly int LevelDefaultValue = -1;
	private static readonly int BatchedCountDefaultValue = 1;
#pragma warning restore CS0414


	/// <inheritdoc/>
	public override string RaisingCommand => "购买";

	/// <summary>
	/// Indicates the item you want to buy.
	/// </summary>
	[DoubleArgumentCommand("物品")]
	public string? ItemName { get; set; }

	/// <summary>
	/// Indicates the level of the clover to buy.
	/// </summary>
	[DoubleArgumentCommand("等级")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue(nameof(LevelDefaultValue))]
	public int Level { get; set; }

	/// <summary>
	/// Indicates the batched count.
	/// </summary>
	[DoubleArgumentCommand("批量")]
	[ValueConverter<NumericConverter<int>>]
	[DefaultValue(nameof(BatchedCountDefaultValue))]
	public int BatchedCount { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		if (InternalReadWrite.Read(senderId) is not { ExperiencePoint: var exp, Coin: var coin } user)
		{
			await messageReceiver.SendMessageAsync("用户没有使用过机器人。无法购买商品。");
			return;
		}

		if (Scorer.GetGrade(exp) < 20)
		{
			await messageReceiver.SendMessageAsync("购买功能至少需要用户达到 20 级才可使用。");
			return;
		}

		switch (this)
		{
			case { ItemName: ItemNames.Card, BatchedCount: var count }:
			{
				var price = ShoppingItem.Card.GetPrice();
				if (coin < price * count)
				{
					goto Failed_CoinNotEnough;
				}

				user.Coin -= price * count;

				if (!user.Items.TryAdd(ShoppingItem.Card, 1))
				{
					user.Items[ShoppingItem.Card] += count;
				}

				InternalReadWrite.Write(user);

				goto Successful;
			}
			case { ItemName: null or ItemNames.Clover, Level: var level, BatchedCount: var count }:
			{
				if (level is not >= 1 and <= 9)
				{
					await messageReceiver.SendMessageAsync("可购买的三叶草卡片等级为 1 到 9。请检查你的输入，如“！购买 物品 三叶草 等级 3”。");
					break;
				}

				var targetItem = ShoppingItem.CloverLevel1 + (level - 1);
				var price = targetItem.GetPrice();
				if (coin < price * count)
				{
					goto Failed_CoinNotEnough;
				}

				user.Coin -= price * count;

				if (!user.Items.TryAdd(targetItem, 1))
				{
					user.Items[targetItem] += count;
				}

				InternalReadWrite.Write(user);

				goto Successful;
			}
			default:
			{
				await messageReceiver.SendMessageAsync(
					"你要购买的物品名称不清楚。可提供购买的商品可以为“三叶草”和“强化卡”两种。请输入合适的指令，如“！购买 物品 强化卡”。"
				);

				break;
			}

		Failed_CoinNotEnough:
			{
				await messageReceiver.SendMessageAsync("购买失败。你的金币不足。");
				break;
			}
		Successful:
			{
				await messageReceiver.SendMessageAsync("恭喜，购买成功！");
				break;
			}
		}
	}
}

file static class ItemNames
{
	public const string Clover = "三叶草";
	public const string Card = "强化卡";
}
