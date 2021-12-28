namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0121")]
public sealed partial class LambdaDiscardParameterSyntaxChecker : ISyntaxContextReceiver
{
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
					descriptor: SCA0121,
					location: parameter.Locations[0],
					messageArgs: null
				)
			);
		}
	}
}
