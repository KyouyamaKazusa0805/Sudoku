namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="ISymbol"/>.
/// </summary>
/// <seealso cref="ISymbol"/>
internal static class ISymbolExtensions
{
	/// <summary>
	/// To determine whether the specified symbol (should be property or field members)
	/// has an initializer.
	/// </summary>
	/// <param name="this">The symbol to check.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool HasInitializer(this ISymbol @this) =>
		@this is { DeclaringSyntaxReferences: { Length: not 0 } list }
		&& list[0] is (_, syntaxNode: VariableDeclaratorSyntax { Initializer: not null });

	/// <summary>
	/// Gets the member type string representation.
	/// </summary>
	/// <param name="this">The symbol.</param>
	/// <returns>The result string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string? GetMemberType(this ISymbol @this) => @this switch
	{
		IFieldSymbol f => f.Type.ToDisplayString(TypeFormats.FullName),
		IPropertySymbol p => p.Type.ToDisplayString(TypeFormats.FullName),
		_ => null
	};
}
