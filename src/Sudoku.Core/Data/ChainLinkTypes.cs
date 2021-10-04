namespace Sudoku.Data;

/// <summary>
/// Defines the <see langword="static"/> data for <see cref="ChainLinkType"/> instances.
/// </summary>
public static class ChainLinkTypes
{
	/// <summary>
	/// Indicates the default link.
	/// </summary>
	public static readonly ChainLinkType Default = new(0);

	/// <summary>
	/// Indicates the weak link.
	/// </summary>
	public static readonly ChainLinkType Weak = new(1);

	/// <summary>
	/// Indicates the strong link.
	/// </summary>
	public static readonly ChainLinkType Strong = new(2);

	/// <summary>
	/// Indicates the line link.
	/// </summary>
	public static readonly ChainLinkType Line = new(3);
}
