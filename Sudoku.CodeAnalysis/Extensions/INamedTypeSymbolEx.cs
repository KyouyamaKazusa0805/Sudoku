using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="INamedTypeSymbol"/>.
	/// </summary>
	/// <seealso cref="INamedTypeSymbol"/>
	public static class INamedTypeSymbolEx
	{
		/// <summary>
		/// Check whether the current symbol derives from the specified type symbol.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The symbol to check.</param>
		/// <param name="type">The type.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool DerivedFrom(this INamedTypeSymbol @this, ITypeSymbol? type)
		{
			if (type is null)
			{
				return false;
			}

			for (var baseType = @this.BaseType; baseType is not null; baseType = baseType.BaseType)
			{
				if (SymbolEqualityComparer.Default.Equals(baseType, type))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether the current symbol derives from the specified type.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The symbol to check.</param>
		/// <param name="compilation">The compilation.</param>
		/// <param name="typeName">The type name.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool DerivedFrom(this INamedTypeSymbol @this, Compilation compilation, string typeName) =>
			@this.DerivedFrom(compilation.GetTypeByMetadataName(typeName));
	}
}
