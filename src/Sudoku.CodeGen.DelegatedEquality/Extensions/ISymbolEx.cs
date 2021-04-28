using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.DelegatedEquality.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ISymbol"/>.
	/// </summary>
	/// <seealso cref="ISymbol"/>
	public static class ISymbolEx
	{
		/// <summary>
		/// To determine whether the symbol has marked the specified attribute.
		/// </summary>
		/// <typeparam name="TAttribute">The type of that attribute.</typeparam>
		/// <param name="this">The symbol to check.</param>
		/// <returns>The result.</returns>
		public static bool Marks<TAttribute>(this ISymbol @this) where TAttribute : Attribute =>
			@this.GetAttributes().Any(static x => x.AttributeClass?.Name == typeof(TAttribute).Name);
	}
}
