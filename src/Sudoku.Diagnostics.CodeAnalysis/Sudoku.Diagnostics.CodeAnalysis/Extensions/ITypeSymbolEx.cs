using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.Deconstruction.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ITypeSymbol"/>.
	/// </summary>
	/// <seealso cref="ITypeSymbol"/>
	public static class ITypeSymbolEx
	{
		/// <summary>
		/// Get all deconstruction methods in this current type.
		/// </summary>
		/// <param name="this">The type.</param>
		/// <returns>All possible deconstruction methods.</returns>
		public static IEnumerable<IMethodSymbol> GetAllDeconstructionMethods(this ITypeSymbol @this) =>
			from method in @this.GetAllMembers().OfType<IMethodSymbol>()
			where method.IsDeconstructionMethod()
			orderby method.Parameters.Length
			select method;
	}
}
