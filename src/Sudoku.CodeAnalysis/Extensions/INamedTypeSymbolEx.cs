using System.Linq;
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
		/// <param name="this">The symbol to check.</param>
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
		/// Creates an <see cref="INamedTypeSymbol"/> with specified type arguments.
		/// </summary>
		/// <param name="this">The base symbol.</param>
		/// <param name="compilation">The compilation.</param>
		/// <param name="specialTypes">All special types to create.</param>
		/// <returns>Result symbol.</returns>
		public static INamedTypeSymbol WithTypeArguments(
			this INamedTypeSymbol @this, Compilation compilation, params SpecialType[] specialTypes) =>
			@this.Construct(
				(
					from specialType in specialTypes
					select compilation.GetSpecialType(specialType)
				).ToArray()
			);

		/// <summary>
		/// Check whether the current symbol derives from the specified type.
		/// </summary>
		/// <param name="this">The symbol to check.</param>
		/// <param name="compilation">The compilation.</param>
		/// <param name="typeName">The type name.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool DerivedFrom(this INamedTypeSymbol @this, Compilation compilation, string typeName) =>
			@this.DerivedFrom(compilation.GetTypeByMetadataName(typeName));
	}
}
