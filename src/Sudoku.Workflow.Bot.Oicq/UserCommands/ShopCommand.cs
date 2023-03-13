namespace Sudoku.Workflow.Bot.Oicq.UserCommands;

[GroupCommandModule("商店")]
internal sealed class ShopCommand : GroupCommandModule
{
	/// <summary>
	/// 表示你要查看的商店的物品类型。可以填入的值有“强化卡”和“三叶草”。
	/// </summary>
	[DoubleArgument("物品")]
	[Hint("表示你要查看的商店的物品类型。可以填入的值有“强化卡”和“三叶草”。")]
	public string? ItemKind { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Id: var senderId })
		{
			return;
		}

		if (StorageHandler.Read(senderId) is not { ExperiencePoint: var exp } user)
		{
			await messageReceiver.QuoteMessageAsync("用户尚没有使用过机器人。商店功能至少需要用户达到 20 级才可使用。");
			return;
		}

		if (ScoreHandler.GetGrade(exp) < 20)
		{
			await messageReceiver.QuoteMessageAsync("很抱歉，商店功能至少需要用户达到 20 级才可使用。");
			return;
		}

		switch (ItemKind)
		{
			case null:
			{
				await messageReceiver.QuoteMessageAsync("商品物件太多，暂时不支持商品全查询。请使用参数“物品”查询具体物件信息。");
				break;
			}
			case ItemKinds.Card:
			{
				var itemsString = new List<string>();
				foreach (var element in Enum.GetValues<Item>())
				{
					if (element.GetGroup() == ItemGroup.Card && element.IsBuyable(out var price))
					{
						itemsString.Add($"{element.GetName()} - {price} 金币 / 1 个");
					}
				}

				await messageReceiver.SendMessageAsync(
					$"""
					商品 - 卡片：
					可提供玩家 PK 或其他可获得经验值和金币时的加成倍数的基础卡片。卡片等级越高，卡片能带来的加成倍数越大。
					---
					{string.Join(Environment.NewLine, itemsString)}
					"""
				);

				break;
			}
			case ItemKinds.Clover:
			{
				var itemsString = new List<string>();
				foreach (var element in Enum.GetValues<Item>())
				{
					if (element.GetGroup() == ItemGroup.Clover && element.IsBuyable(out var price))
					{
						itemsString.Add($"{element.GetName()} - {price} 金币 / 1 个");
					}
				}

				await messageReceiver.SendMessageAsync(
					$"""
					商品 - 三叶草：
					提供给玩家强化底卡时，提高成功率。
					---
					{string.Join(Environment.NewLine, itemsString)}
					"""
				);

				break;
			}
		}
	}
}

/// <summary>
/// 表示 <see cref="ShopCommand.ItemKind"/> 属性的所有取值。
/// </summary>
/// <seealso cref="ShopCommand.ItemKind"/>
file static class ItemKinds
{
	/// <summary>
	/// 表示物品是强化卡。
	/// </summary>
	public const string Card = "强化卡";

	/// <summary>
	/// 表示物品是三叶草。
	/// </summary>
	public const string Clover = "三叶草";
}
