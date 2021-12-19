namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0407")]
public sealed partial class CallerArgumentExpressionAttributeSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: InvocationExpressionSyntax
				{
					ArgumentList.Arguments: { Count: >= 1 } argumentNodes
				} node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IInvocationOperation
			{
				TargetMethod: var methodSymbol,
				Arguments: { Length: >= 1 } arguments
			}
		)
		{
			return;
		}

		var stringSymbol = compilation.GetSpecialType(SpecialType.System_String);
		var attribute = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.CallerArgumentExpressionAttribute")!;
		foreach (var argument in argumentNodes)
		{
			var argumentOperation = semanticModel.GetOperation(argument, _cancellationToken);
			if (argumentOperation is not IArgumentOperation { Parameter: { Type: var parameterType } parameter })
			{
				continue;
			}

			if (parameter.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			if (!SymbolEqualityComparer.Default.Equals(stringSymbol, parameterType))
			{
				continue;
			}

			Diagnostics.Add(Diagnostic.Create(SCA0407, argument.GetLocation(), messageArgs: null));
		}
	}
}
