namespace Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides extension methods on <see cref="ExpressionSyntax"/>.
/// </summary>
/// <seealso cref="ExpressionSyntax"/>
internal static class ExpressionSyntaxExtensions
{
	/// <summary>
	/// To check whether the specified expression is a <see langword="nameof"/> expression.
	/// </summary>
	/// <param name="this">The expression node to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNameOfExpression(this ExpressionSyntax? @this) => @this is InvocationExpressionSyntax
	{
		Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" }
	};
}
