namespace Sudoku.Platforms.QQ.Data.Shopping;

/// <summary>
/// Provides with extension methods on <see cref="ShoppingItem"/>.
/// </summary>
/// <seealso cref="ShoppingItem"/>
public static class ShoppingItemExtensions
{
	/// <summary>
	/// The cached field.
	/// </summary>
	private static readonly Type ShoppingItemTypeInfo = typeof(ShoppingItem);


	/// <summary>
	/// Gets the name of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <returns>The name of the item.</returns>
	/// <exception cref="InvalidOperationException">Throws when the field is not marked <see cref="NameAttribute"/>.</exception>
	public static string GetName(this ShoppingItem @this)
		=> ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()?.Name
		?? throw new InvalidOperationException($"The current enumeration field must be marked as '{nameof(NameAttribute)}'.");

	/// <summary>
	/// Gets the name of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <param name="defaultValue">The default name to be displayed if the field is not marked <see cref="NameAttribute"/>.</param>
	/// <returns>The name of the item.</returns>
	public static string GetName(this ShoppingItem @this, string defaultValue)
		=> ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()?.Name ?? defaultValue;

	/// <summary>
	/// Gets the description of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <returns>The description of the item.</returns>
	public static string? GetDescription(this ShoppingItem @this)
		=> ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<DescriptionAttribute>()?.Description;

	/// <summary>
	/// Gets the description of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <param name="defaultValue">The default name to be displayed if the field is not marked <see cref="DescriptionAttribute"/>.</param>
	/// <returns>The description of the item.</returns>
	public static string GetDescription(this ShoppingItem @this, string defaultValue)
		=> ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<DescriptionAttribute>()?.Description ?? defaultValue;

	/// <summary>
	/// Indicates whether the item is buyable. If <see langword="true"/>, the price will be returned via argument <paramref name="price"/>.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <param name="price">The price if available.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsBuyable(this ShoppingItem @this, out int price)
	{
		if (ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<PriceAttribute>() is { Price: var p })
		{
			price = p;
			return true;
		}

		price = -1;
		return false;
	}

	/// <summary>
	/// Gets the price of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <returns>The price of the item.</returns>
	/// <exception cref="InvalidOperationException">Throws when the item is not buyable.</exception>
	public static int GetPrice(this ShoppingItem @this)
		=> ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<PriceAttribute>()?.Price
		?? throw new InvalidOperationException("This item is not buyable.");

	/// <summary>
	/// Gets the group of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <returns>The group of the item.</returns>
	/// <exception cref="InvalidOperationException">Throws when the field is not marked <see cref="GroupAttribute"/>.</exception>
	public static ShoppingItemGroup GetGroup(this ShoppingItem @this)
		=> ShoppingItemTypeInfo.GetField(@this.ToString())!.GetCustomAttribute<GroupAttribute>()?.Group
		?? throw new InvalidOperationException("Every shopping item should be counted in a group.");
}
