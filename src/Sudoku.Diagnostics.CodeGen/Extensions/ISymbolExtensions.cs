namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="ISymbol"/>.
/// </summary>
/// <seealso cref="ISymbol"/>
internal static class ISymbolExtensions
{
	/// <summary>
	/// Gets the member type string representation.
	/// </summary>
	/// <param name="this">The symbol.</param>
	/// <returns>
	/// The result string that is the string representation of the type gotten.
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see cref="IFieldSymbol"/></term>
	/// <description>The property <see cref="IFieldSymbol.Type"/> will be returned.</description>
	/// </item>
	/// <item>
	/// <term><see cref="IPropertySymbol"/></term>
	/// <description>The property <see cref="IPropertySymbol.Type"/> will be returned.</description>
	/// </item>
	/// <item>
	/// <term><see cref="IMethodSymbol"/></term>
	/// <description>The property <see cref="IMethodSymbol.ReturnType"/> will be returned.</description>
	/// </item>
	/// <item>
	/// <term><see cref="IEventSymbol"/></term>
	/// <description>The property <see cref="IEventSymbol.Type"/> will be returned.</description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>Constant value <see langword="null"/> will be returned.</description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string? GetMemberType(this ISymbol @this) => @this switch
	{
		IFieldSymbol f => f.Type.ToDisplayString(TypeFormats.FullName),
		IPropertySymbol p => p.Type.ToDisplayString(TypeFormats.FullName),
		IMethodSymbol m => m.ReturnType.ToDisplayString(TypeFormats.FullName),
		IEventSymbol e => e.Type.ToDisplayString(TypeFormats.FullName),
		_ => null
	};
}
