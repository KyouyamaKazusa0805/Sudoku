namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一种物品。该物品可能可以被购买到，也可能在商店里不提供购买，但它可以作为用户保留到数据内，作为数据的一部分。
/// </summary>
public enum Item
{
	/// <summary>
	/// 表示一张强化卡。
	/// </summary>
	[Name("强化卡")]
	[Description("可提供玩家 PK 或其他可获得经验值和金币时的加成倍数的基础卡片。卡片等级越高，卡片能带来的加成倍数越大。")]
	[Price(100)]
	[ItemGroup(ItemGroup.Card)]
	Card,

	/// <summary>
	/// 表示Ⅰ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 1.1 倍。
	/// </summary>
	[Name("三叶草 1 级（倍率 1.1）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(150)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel1,

	/// <summary>
	/// 表示Ⅱ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 1.2 倍。
	/// </summary>
	[Name("三叶草 2 级（倍率 1.2）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(400)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel2,

	/// <summary>
	/// 表示Ⅲ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 1.4 倍。
	/// </summary>
	[Name("三叶草 3 级（倍率 1.4）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(900)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel3,

	/// <summary>
	/// 表示Ⅳ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 1.7 倍。
	/// </summary>
	[Name("三叶草 4 级（倍率 1.7）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(1300)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel4,

	/// <summary>
	/// 表示Ⅴ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 2.0 倍。
	/// </summary>
	[Name("三叶草 5 级（倍率 2.0）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(2500)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel5,

	/// <summary>
	/// 表示Ⅵ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 2.4 倍。
	/// </summary>
	[Name("三叶草 6 级（倍率 2.4）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(5000)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel6,

	/// <summary>
	/// 表示Ⅶ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 3.0 倍。
	/// </summary>
	[Name("三叶草 7 级（倍率 3.0）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(10000)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel7,

	/// <summary>
	/// 表示Ⅷ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 3.3 倍。
	/// </summary>
	[Name("三叶草 8 级（倍率 3.3）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(50000)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel8,

	/// <summary>
	/// 表示Ⅸ级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 3.6 倍。
	/// </summary>
	[Name("三叶草 9 级（倍率 3.6）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[Price(100000)]
	[ItemGroup(ItemGroup.Clover)]
	CloverLevel9,

	/// <summary>
	/// 表示终级三叶草。它可以帮助你在强化的时候，将成功率改为原来的 4.0 倍。
	/// </summary>
	[Name("三叶草终级（倍率 4.0）")]
	[Description("提供给玩家强化底卡时，提高成功率。")]
	[ItemGroup(ItemGroup.Clover)]
	CloverFinal
}
