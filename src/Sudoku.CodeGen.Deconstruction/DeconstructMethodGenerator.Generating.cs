#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.Deconstruction.Annotations;
using Sudoku.CodeGen.Deconstruction.Extensions;

namespace Sudoku.CodeGen.Deconstruction
{
	partial class DeconstructMethodGenerator
	{
		/// <summary>
		/// Try to get all possible fields or properties in the specified type.
		/// </summary>
		/// <param name="symbol">The specified symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.
		/// </param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static partial IReadOnlyList<SymbolInfo> GetMembers(
			INamedTypeSymbol symbol, bool handleRecursively)
		{
			var result = new List<SymbolInfo>(
				(
					from x in symbol.GetMembers().OfType<IFieldSymbol>()
					select new SymbolInfo(
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				).Concat(
					from x in symbol.GetMembers().OfType<IPropertySymbol>()
					select new SymbolInfo(
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				)
			);

			if (handleRecursively
				&& symbol.BaseType is { } baseType && baseType.Marks<AutoDeconstructAttribute>())
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
