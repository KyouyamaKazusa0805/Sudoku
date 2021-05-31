using System;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SemanticModel"/>.
	/// </summary>
	/// <seealso cref="SemanticModel"/>
	public static class SemanticModelEx
	{
		/// <summary>
		/// Checks whether the two <see cref="SyntaxNode"/>s hold the same type.
		/// </summary>
		/// <param name="this">The semantic model.</param>
		/// <param name="left">The left comparer.</param>
		/// <param name="right">The right comparer.</param>
		/// <param name="includeNullableAnnotation">
		/// Indicates whether the comparer uses advanced one to check nullable type.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <exception cref="ArgumentException">Throws when the type can't be inferred.</exception>
		public static bool TypeEquals(
			this SemanticModel @this, SyntaxNode left, SyntaxNode right, bool includeNullableAnnotation = false) =>
			@this.GetOperation(left) is not { Type: { } leftType }
			? throw new ArgumentException("The type can't be inferred.", nameof(left))
			: @this.GetOperation(right) is not { Type: { } rightType }
			? throw new ArgumentException("The type can't be inferred.", nameof(right))
			: (
				includeNullableAnnotation
				? SymbolEqualityComparer.IncludeNullability
				: SymbolEqualityComparer.Default
			).Equals(leftType, rightType);
	}
}
