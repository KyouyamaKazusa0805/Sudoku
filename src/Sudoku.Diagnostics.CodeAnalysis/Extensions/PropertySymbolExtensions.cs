namespace Microsoft.CodeAnalysis;

/// <summary>
/// Proivdes extension methods on <see cref="IPropertySymbol"/>.
/// </summary>
/// <seealso cref="IPropertySymbol"/>
public static class PropertySymbolExtensions
{
	/// <summary>
	/// Determine whether the current property is auto-implemented property.
	/// </summary>
	/// <param name="this">The property symbol.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAutoProperty(this IPropertySymbol @this) =>
		@this
			.ContainingType
			.GetMembers()
			.OfType<IFieldSymbol>()
			.Any(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, @this));
}
