namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示价格的注解。用于一个枚举字段，表示该字段对应的物品的价格。
/// </summary>
/// <param name="price">物品的价格。</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class PriceAttribute(int price) : EnumFieldAttribute
{
	/// <summary>
	/// 表示物品的价格。
	/// </summary>
	public int Price { get; } = price;
}
