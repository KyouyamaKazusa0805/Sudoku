namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="IPropertySymbol"/>.
/// </summary>
/// <see cref="IPropertySymbol"/>
public static class IPropertySymbolExtensions
{
	/// <summary>
	/// <para>
	/// Checks whether the property member that specified as an <see cref="IPropertySymbol"/> instance
	/// is an auto-implemented property.
	/// </para>
	/// <para>
	/// An <b>auto-implemented property</b> is a property that only contains the keyword <see langword="get"/>,
	/// <see langword="set"/> or <see langword="init"/> as the body,
	/// e.g. <c>public int Property { get; set; }</c>.
	/// </para>
	/// </summary>
	/// <param name="this">The property symbol to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAutoImplemented(this IPropertySymbol @this) =>
		@this.ContainingType
			.GetMembers()
			.OfType<IFieldSymbol>()
			.Any(field => SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, @this));
}
