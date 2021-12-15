namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

/// <summary>
/// Defines the syntax checker that checks for the diagnostics below:
/// <list type="table">
/// <item>
/// <term><c>SDC0203</c></term>
/// <description>Can't apply <see cref="IsDiscardAttribute"/> onto a parameter that has already discarded.</description>
/// </item>
/// </list>
/// </summary>
public sealed class LambdaDiscardParameterSyntaxChecker : ISyntaxContextReceiver
{
	/// <summary>
	/// Indicates the context used.
	/// </summary>
	private readonly CancellationToken _cancellationToken;


	/// <summary>
	/// Initializes a <see cref="ThisConstraintSyntaxChecker"/> instance using the cancellation token.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	public LambdaDiscardParameterSyntaxChecker(CancellationToken cancellationToken) =>
		_cancellationToken = cancellationToken;


	/// <summary>
	/// Indicates all possible diagnostics types used.
	/// </summary>
	public List<Diagnostic> Diagnostics { get; } = new();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		// Deconstruct instance.
		if (
			context is not
			{
				Node: ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters.Count: not 0 } node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName(typeof(IsDiscardAttribute).FullName);

		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (operation is not IAnonymousFunctionOperation { Symbol.Parameters: var parameters })
		{
			return;
		}

		foreach (var parameter in parameters)
		{
			var attributesData = parameter.GetAttributes();
			if (!attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			if (!parameter.IsDiscard)
			{
				continue;
			}

			Diagnostics.Add(
				Diagnostic.Create(
					descriptor: Sdc0203,
					location: parameter.Locations[0],
					messageArgs: null
				)
			);
		}
	}
}
