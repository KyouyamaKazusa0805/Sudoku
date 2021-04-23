using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGen.Deconstruction.Extensions
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

		/// <summary>
		/// To determine whether the specified symbol (should be property or field members)
		/// has an initializer.
		/// </summary>
		/// <param name="this">The symbol to check.</param>
		/// <returns>The result.</returns>
		internal static bool HasInitializer(this ISymbol @this) =>
			/*length-pattern*/
			@this is { DeclaringSyntaxReferences: { Length: not 0 } list }
			&& list[0] is (_, syntaxNode: VariableDeclaratorSyntax { Initializer: not null });
	}
}
