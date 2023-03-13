namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示价格的注解。用于一个枚举字段，表示该字段对应的物品的价格。
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class PriceAttribute : EnumFieldAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="PriceAttribute"/> 类型的实例，并给出价格。
	/// </summary>
	/// <param name="price">物品的价格。</param>
	public PriceAttribute(int price) => Price = price;


	/// <summary>
	/// 表示物品的价格。
	/// </summary>
	public int Price { get; }
}
