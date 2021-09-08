namespace Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides extension methods on <see cref="ArgumentSyntax"/>.
/// </summary>
/// <seealso cref="ArgumentSyntax"/>
internal static class ArgumentSyntaxExtensions
{
	/// <summary>
	/// Gets the full name of the parameter.
	/// </summary>
	/// <param name="this">The argument syntax node.</param>
	/// <param name="semanticModel">The semantic model.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The full name.</returns>
	public static string? GetParamFullName(
		this ArgumentSyntax @this,
		SemanticModel semanticModel,
		CancellationToken cancellationToken
	)
	{
		if (
			semanticModel.GetOperation(@this.Expression, cancellationToken) is not
			{
				Type: { } typeSymbol
			} operation
		)
		{
			return null;
		}

		string typeStr = typeSymbol.ToDisplayString(NullableFlowState.None);
		return @this.RefKindKeyword.RawKind switch
		{
			(int)SyntaxKind.RefKeyword => $"ref {typeStr}",
			(int)SyntaxKind.OutKeyword => $"out {typeStr}",
			_ => typeStr
		};
	}
}
