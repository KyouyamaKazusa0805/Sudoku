namespace Sudoku.Platforms.QQ.Data.Shopping;

/// <summary>
/// Provides with extension methods on <see cref="ShoppingItem"/>.
/// </summary>
/// <seealso cref="ShoppingItem"/>
public static class ShoppingItemExtensions
{
	/// <summary>
	/// Gets the name of the item.
	/// </summary>
	/// <param name="this">The shopping item.</param>
	/// <returns>The name of the item.</returns>
	public static string GetName(this ShoppingItem @this)
		=> typeof(ShoppingItem).GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()!.Name;
}
