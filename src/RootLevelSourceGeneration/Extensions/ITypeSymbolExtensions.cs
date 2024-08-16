namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="ITypeSymbol"/>.
/// </summary>
/// <seealso cref="ITypeSymbol"/>
public static class ITypeSymbolExtensions
{
	/// <summary>
	/// Try to treat the current type as a collection type, to fetch its element type.
	/// </summary>
	/// <param name="this">The current type symbol.</param>
	/// <returns>The type of element.</returns>
	public static ITypeSymbol? GetCollectionElementType(this ITypeSymbol @this)
	{
		return @this switch { INamedTypeSymbol i => i.GetAllMembers(true), _ => @this.GetMembers() } is var allMembers
			&& allMembers.FirstOrDefault(getEnumeratorChecker) is { } methodSymbol
			&& methodSymbol is IMethodSymbol { ReturnType: INamedTypeSymbol enumeratorType }
			&& enumeratorType.GetAllMembers(true).ToArray() is { Length: not 0 } members
			&& Array.FindIndex(members, currentPropertyChecker) is var propertyIndex and not -1
			&& Array.FindIndex(members, moveNextChecker) != -1
			? ((IPropertySymbol)members[propertyIndex]).Type
			: null;


		static bool getEnumeratorChecker(ISymbol m)
			=> m is IMethodSymbol
			{
				Name: "GetEnumerator",
				ReturnsVoid: false,
				ReturnsByRef: false,
				Parameters: []
			};

		static bool currentPropertyChecker(ISymbol m) => m is IPropertySymbol { Name: "Current", IsIndexer: false };

		static bool moveNextChecker(ISymbol m)
			=> m is IMethodSymbol
			{
				Name: "MoveNext",
				Parameters: [],
				ReturnsByRef: false,
				ReturnType.SpecialType: SpecialType.System_Boolean
			};
	}
}
