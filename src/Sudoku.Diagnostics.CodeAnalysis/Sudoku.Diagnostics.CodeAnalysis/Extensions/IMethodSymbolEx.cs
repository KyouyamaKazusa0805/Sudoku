using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IMethodSymbol"/>.
	/// </summary>
	/// <seealso cref="IMethodSymbol"/>
	public static class IMethodSymbolEx
	{
		/// <summary>
		/// To determine whether the specified method symbol is referenced to a deconstruction method.
		/// </summary>
		/// <param name="this">The symbol to check.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool IsDeconstructionMethod(this IMethodSymbol @this)
		{
			if (
				@this is not
				{
					Name: "Deconstruct",
					Parameters: { Length: >= 2 } parameters,
					TypeParameters: { IsEmpty: true },
					ExplicitInterfaceImplementations: { IsEmpty: true },
					DeclaredAccessibility: Accessibility.Public,
					IsStatic: false
				}
			)
			{
				return false;
			}

			if (parameters.Any(static parameter => parameter.RefKind != RefKind.Out))
			{
				return false;
			}

			foreach (var parameter in parameters)
			{
				var (type, name) = parameter;
				string possibleFieldName = name.ToCamelCase(), possiblePropertyName = name.ToPascalCase();

				if (
					(
						from fieldOrPropertySymbol in type.GetMembers()
						where fieldOrPropertySymbol.CanBeReferencedByName
						select fieldOrPropertySymbol
					).All(
						symbol => symbol switch
						{
							IFieldSymbol when symbol.Name != possibleFieldName => true,
							IPropertySymbol when symbol.Name != possiblePropertyName => true,
							_ => false
						}
					)
				)
				{
					return false;
				}
			}

			return true;
		}
	}
}
