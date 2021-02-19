using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Provides extension methods on <see cref="INamedTypeSymbol"/>.
	/// </summary>
	/// <seealso cref="INamedTypeSymbol"/>
	public static class INamedTypeSymbolEx
	{
		/// <summary>
		/// Check whether the current symbol inherits from the specified type symbol.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The symbol to check.</param>
		/// <param name="type">The type.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool InheritsFrom(this INamedTypeSymbol @this, ITypeSymbol type)
		{
			var baseType = @this.BaseType;
			while (!(baseType is null))
			{
				if (SymbolEqualityComparer.Default.Equals(baseType, type))
				{
					return true;
				}

				baseType = baseType.BaseType;
			}

			return false;
		}
	}
}
