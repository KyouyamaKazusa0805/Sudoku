#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.PrimaryConstructor.Annotations;
using Sudoku.CodeGen.PrimaryConstructor.Extensions;

namespace Sudoku.CodeGen.PrimaryConstructor
{
	partial class PrimaryConstructorGenerator
	{
		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="classSymbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static partial IReadOnlyList<SymbolInfo> GetMembers(
			INamedTypeSymbol classSymbol, bool handleRecursively)
		{
			var result = new List<SymbolInfo>(
				(
					from x in classSymbol.GetMembers().OfType<IFieldSymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !x.HasInitializer()
							|| x.Marks<PrimaryConstructorIncludedMemberAttribute>()
						)
						&& !x.Marks<PrimaryConstructorIgnoredMemberAttribute>()
					select new SymbolInfo(
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				).Concat(
					from x in classSymbol.GetMembers().OfType<IPropertySymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !x.HasInitializer()
							|| x.Marks<PrimaryConstructorIncludedMemberAttribute>()
						)
						&& !x.Marks<PrimaryConstructorIgnoredMemberAttribute>()
					select new SymbolInfo(
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				)
			);

			if (handleRecursively
				&& classSymbol.BaseType is { } baseType
				&& baseType.Marks<AutoGeneratePrimaryConstructorAttribute>())
			{
				result.AddRange(GetMembers(baseType, true));
			}

			return result;


			static string toCamelCase(string name)
			{
				name = name.TrimStart('_');
				return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
			}
		}
	}
}
