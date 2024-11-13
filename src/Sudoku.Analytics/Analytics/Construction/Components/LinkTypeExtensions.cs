namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Provides with extension methods on <see cref="LinkType"/>.
/// </summary>
/// <seealso cref="LinkType"/>
public static class LinkTypeExtensions
{
	/// <summary>
	/// Creates a <see cref="ChainingRule"/> instance from the specified link type.
	/// </summary>
	/// <param name="this">The link type.</param>
	/// <returns>The target <see cref="ChainingRule"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ChainingRule? GetRuleInstance(this LinkType @this)
	{
		var types = typeof(LinkType).GetField(@this.ToString())?.GetGenericAttributeTypeArguments(typeof(ChainingRuleAttribute<>));
		return types is [var type] ? (ChainingRule?)Activator.CreateInstance(type) : null;
	}
}
