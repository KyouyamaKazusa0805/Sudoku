namespace Sudoku.Data;

/// <summary>
/// Defines the <see langword="static"/> data for <see cref="LinkType"/> instances.
/// </summary>
/// <completionlist cref="LinkType"/>
public static class LinkTypes
{
	/// <summary>
	/// Indicates the default link.
	/// </summary>
	public static readonly LinkType Default = new(0);

	/// <summary>
	/// Indicates the weak link.
	/// </summary>
	public static readonly LinkType Weak = new(1);

	/// <summary>
	/// Indicates the strong link.
	/// </summary>
	public static readonly LinkType Strong = new(2);

	/// <summary>
	/// Indicates the line link.
	/// </summary>
	public static readonly LinkType Line = new(3);
}
