namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="INamespaceSymbol"/>.
/// </summary>
/// <seealso cref="INamespaceSymbol"/>
internal static class INamespaceSymbolExtensions
{
	/// <summary>
	/// Try to fetch all possible types that is from the specified namespace and its children namespaces.
	/// </summary>
	/// <param name="this">The namespace.</param>
	/// <returns>All found types.</returns>
	public static IEnumerable<INamedTypeSymbol> GetAllNestedTypes(this INamespaceSymbol @this)
	{
		var result = new List<INamedTypeSymbol>(@this.GetTypeMembers());
		getAllNestedTypes(@this, result);
		return result;

		static void getAllNestedTypes(INamespaceSymbol @this, List<INamedTypeSymbol> result)
		{
			foreach (var @namespace in @this.GetNamespaceMembers())
			{
				getAllNestedTypes(@namespace, result);
				result.AddRange(@namespace.GetTypeMembers());
			}
		}
	}
}
