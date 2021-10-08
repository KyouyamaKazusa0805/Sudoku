namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <seealso cref="INamedTypeSymbol"/>
public static class INamedTypeSymbolExtensions
{
	/// <summary>
	/// Determines whether the current type is derived from the specified type.
	/// </summary>
	/// <param name="this">The type symbol.</param>
	/// <param name="typeSymbol">The type symbol.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool DerivedFrom(this INamedTypeSymbol @this, INamedTypeSymbol typeSymbol)
	{
		for (var symbol = @this; symbol is not null; symbol = symbol.BaseType)
		{
			if (SymbolEqualityComparer.Default.Equals(typeSymbol, symbol))
			{
				return true;
			}
		}

		return false;
	}
}
