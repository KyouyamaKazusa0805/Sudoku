namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="SemanticModel"/>.
/// </summary>
/// <seealso cref="SemanticModel"/>
public static class SemanticModelExtensions
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
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <exception cref="ArgumentException">Throws when the type can't be inferred.</exception>
	public static bool TypeEquals(
		this SemanticModel @this, SyntaxNode left, SyntaxNode right, bool withNullableChecking = false,
		CancellationToken cancellationToken = default) => (
			LKind: left.RawKind,
			RKind: right.RawKind,
			LType: @this.GetOperation(left, cancellationToken)?.Type,
			RType: @this.GetOperation(right, cancellationToken)?.Type
		) switch
		{
			(LKind: (int)SyntaxKind.NullLiteralExpression, _, _, RType: { IsReferenceType: true }) => true,
			(_, RKind: (int)SyntaxKind.NullLiteralExpression, LType: { IsReferenceType: true }, _) => true,
			(LKind: (int)SyntaxKind.NullLiteralExpression, _, _, RType: { IsValueType: true }) => false,
			(_, RKind: (int)SyntaxKind.NullLiteralExpression, LType: { IsValueType: true }, _) => false,
			(_, _, LType: null, _) => throw new ArgumentException("The type can't be inferred.", nameof(left)),
			(_, _, _, RType: null) => throw new ArgumentException("The type can't be inferred.", nameof(right)),
			(_, _, var l, var r) => (withNullableChecking ? SymbolEqualityComparer.IncludeNullability : SymbolEqualityComparer.Default).Equals(l, r)
		};
}
