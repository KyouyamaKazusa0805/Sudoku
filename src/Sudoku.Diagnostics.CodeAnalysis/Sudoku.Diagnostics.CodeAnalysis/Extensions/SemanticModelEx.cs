using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.SymbolEqualityComparer;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SemanticModel"/>.
	/// </summary>
	/// <seealso cref="SemanticModel"/>
	public static class SemanticModelEx
	{
		/// <summary>
		/// <para>Checks whether the two <see cref="SyntaxNode"/>s hold the same type.</para>
		/// <para>
		/// If <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>-literal expression
		/// and another node is of reference type, the method also return <see langword="true"/>.
		/// </para>
		/// </summary>
		/// <param name="this">The semantic model.</param>
		/// <param name="left">The left comparer.</param>
		/// <param name="right">The right comparer.</param>
		/// <param name="withNullableChecking">
		/// Indicates whether the comparer uses advanced one to check nullable type.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <exception cref="ArgumentException">Throws when the type can't be inferred.</exception>
		public static bool TypeEquals(
			this SemanticModel @this, SyntaxNode left, SyntaxNode right, bool withNullableChecking = false) =>
			(left.RawKind, right.RawKind, @this.GetOperation(left)?.Type, @this.GetOperation(right)?.Type) switch
			{
				((int)SyntaxKind.NullLiteralExpression, _, _, { IsReferenceType: true }) => true,
				(_, (int)SyntaxKind.NullLiteralExpression, { IsReferenceType: true }, _) => true,
				((int)SyntaxKind.NullLiteralExpression, _, _, { IsValueType: true }) => false,
				(_, (int)SyntaxKind.NullLiteralExpression, { IsValueType: true }, _) => false,
				(_, _, null, _) => throw new ArgumentException("The type can't be inferred.", nameof(left)),
				(_, _, _, null) => throw new ArgumentException("The type can't be inferred.", nameof(right)),
				(_, _, var l, var r) => (withNullableChecking ? IncludeNullability : Default).Equals(l, r)
			};
	}
}
