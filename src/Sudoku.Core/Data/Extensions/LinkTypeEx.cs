namespace Sudoku.Data.Extensions;

/// <summary>
/// Provides extension methods for <see cref="LinkType"/>.
/// </summary>
/// <seealso cref="LinkType"/>
public static class LinkTypeEx
{
	/// <summary>
	/// Get the string notation of this current link type.
	/// </summary>
	/// <param name="this">The link type.</param>
	/// <returns>The notation.</returns>
	public static string GetNotation(this LinkType @this) => @this switch
	{
		LinkType.Default => " -> ",
		LinkType.Weak => " -- ",
		LinkType.Strong => " == ",
		LinkType.Line => " -- "
	};
}
