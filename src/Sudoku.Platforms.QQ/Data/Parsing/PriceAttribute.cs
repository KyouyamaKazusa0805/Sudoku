namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Represents a price attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class PriceAttribute : DataParsingAttribute
{
	/// <summary>
	/// Initializes a <see cref="PriceAttribute"/> instance via the price.
	/// </summary>
	/// <param name="price">The price.</param>
	public PriceAttribute(int price) => Price = price;


	/// <summary>
	/// Indicates the price.
	/// </summary>
	public int Price { get; }
}
