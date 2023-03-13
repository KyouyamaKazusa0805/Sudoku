namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 为 <see cref="Item"/> 的实例提供扩展方法。
/// </summary>
/// <seealso cref="Item"/>
public static class ItemExtensions
{
	/// <summary>
	/// 这是一个 <see langword="typeof"/>(<see cref="Item"/>) 的缓存字段，避免反复产生新的结果。
	/// </summary>
	private static readonly Type ItemTypeInfo = typeof(Item);


	/// <summary>
	/// 获得物品的名字。
	/// </summary>
	/// <exception cref="InvalidOperationException">如果该实例没有标记 <see cref="NameAttribute"/> 特性，就会产生此异常。</exception>
	public static string GetName(this Item @this)
		=> ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()?.Name
		?? throw new InvalidOperationException($"The current enumeration field must be marked as '{nameof(NameAttribute)}'.");

	/// <summary>
	/// 获得物品的名字。如果字段没有标记 <see cref="NameAttribute"/>，那么给出的第二个参数 <paramref name="defaultValue"/> 则会作为结果返回，不产生异常。
	/// </summary>
	public static string GetName(this Item @this, string defaultValue)
		=> ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()?.Name ?? defaultValue;

	/// <summary>
	/// 获得物品的描述信息。如果字段没有标记 <see cref="DescriptionAttribute"/>，那么就会返回 <see langword="null"/>。
	/// </summary>
	public static string? GetDescription(this Item @this)
		=> ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<DescriptionAttribute>()?.Description;

	/// <summary>
	/// 获得物品的描述信息。
	/// 如果字段没有标记 <see cref="DescriptionAttribute"/>，那么给出的第二个参数 <paramref name="defaultValue"/> 则会作为结果返回，不产生异常。
	/// </summary>
	public static string GetDescription(this Item @this, string defaultValue)
		=> ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<DescriptionAttribute>()?.Description ?? defaultValue;

	/// <summary>
	/// 返回是否物品可以购买到。如果返回 <see langword="true"/>，那么参数 <paramref name="price"/> 还会返回处该物品的价格。
	/// </summary>
	public static bool IsBuyable(this Item @this, out int price)
	{
		if (ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<PriceAttribute>() is { Price: var p })
		{
			price = p;
			return true;
		}

		price = -1;
		return false;
	}

	/// <summary>
	/// 获得物品的购买时的价格。
	/// </summary>
	/// <exception cref="InvalidOperationException">如果物品无法被购买到，就会产生此异常。</exception>
	public static int GetPrice(this Item @this)
		=> ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<PriceAttribute>()?.Price
		?? throw new InvalidOperationException("This item is not buyable.");

	/// <summary>
	/// 获得物品的对应组别。
	/// </summary>
	/// <exception cref="InvalidOperationException">如果该实例没有标记 <see cref="ItemGroupAttribute"/> 特性，就会产生此异常。</exception>
	public static ItemGroup GetGroup(this Item @this)
		=> ItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<ItemGroupAttribute>()?.Group
		?? throw new InvalidOperationException("Every shopping item should be counted in a group.");
}
