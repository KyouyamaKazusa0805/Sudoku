using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.Deconstruction.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ITypeSymbol"/>.
	/// </summary>
	/// <seealso cref="ITypeSymbol"/>
	public static class ITypeSymbolEx
	{
		/// <summary>
		/// Get all members that belongs to the type and its base types (but interfaces don't check).
		/// </summary>
		/// <param name="this">The symbol.</param>
		/// <returns>All members.</returns>
		public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol @this)
		{
			for (var typeSymbol = @this; typeSymbol is not null; typeSymbol = typeSymbol.BaseType)
			{
				foreach (var member in typeSymbol.GetMembers())
				{
					yield return member;
				}
			}
		}
	}
}
