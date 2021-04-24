using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.Equality.Annotations;
using Sudoku.CodeGen.Equality.Extensions;

namespace Sudoku.CodeGen.Equality
{
	partial class AutoEqualsMethodGenerator
	{
		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="symbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static partial IReadOnlyList<string> GetMembers(INamedTypeSymbol symbol, bool handleRecursively)
		{
			var result = new List<string>(
				(
					from x in symbol.GetMembers().OfType<IFieldSymbol>()
					select x.Name
				).Concat(
					from x in symbol.GetMembers().OfType<IPropertySymbol>()
					select x.Name
				)
			);

			if (handleRecursively && symbol.BaseType is { } baseType && baseType.Marks<AutoEqualityAttribute>())
			{
				result.AddRange(GetMembers(baseType, handleRecursively: true));
			}

			return result;
		}
	}
}
