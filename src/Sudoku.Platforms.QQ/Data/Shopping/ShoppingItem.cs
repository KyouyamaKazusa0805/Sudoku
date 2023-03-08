namespace Sudoku.Platforms.QQ.Data.Shopping;

/// <summary>
/// Represents a shopping item.
/// </summary>
public enum ShoppingItem
{
	/// <summary>
	/// Indicates the card.
	/// </summary>
	[Name("强化卡")]
	[Description("可提供玩家 PK 或其他可获得经验值和金币时的加成倍数的基础卡片。卡片等级越高，卡片能带来的加成倍数越大。")]
	[Price(100)]
	[Group(ShoppingItemGroup.Card)]
	Card,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 1.1.
	/// </summary>
	[Name("三叶草 1 级（倍率 1.1）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(150)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel1,

	/// <summary>
	/// Indicates the clover level 2, which will make your upgrading factor change (from 1.0) to 1.2.
	/// </summary>
	[Name("三叶草 2 级（倍率 1.2）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(400)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel2,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 1.4.
	/// </summary>
	[Name("三叶草 3 级（倍率 1.4）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(900)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel3,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 1.7.
	/// </summary>
	[Name("三叶草 4 级（倍率 1.7）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(1300)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel4,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 2.0.
	/// </summary>
	[Name("三叶草 5 级（倍率 2.0）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(2500)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel5,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 2.4.
	/// </summary>
	[Name("三叶草 6 级（倍率 2.4）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(5000)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel6,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 3.0.
	/// </summary>
	[Name("三叶草 7 级（倍率 3.0）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(10000)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel7,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 3.3.
	/// </summary>
	[Name("三叶草 8 级（倍率 3.3）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(50000)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel8,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 3.6.
	/// </summary>
	[Name("三叶草 9 级（倍率 3.6）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(100000)]
	[Group(ShoppingItemGroup.Clover)]
	CloverLevel9,

	/// <summary>
	/// Indicates the clover level 1, which will make your upgrading factor change (from 1.0) to 4.0.
	/// </summary>
	[Name("三叶草终级（倍率 4.0）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Group(ShoppingItemGroup.Clover)]
	CloverFinal
}
