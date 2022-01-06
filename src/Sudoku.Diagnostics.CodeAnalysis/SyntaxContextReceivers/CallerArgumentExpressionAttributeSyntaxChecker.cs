namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0407", "SCA0408", "SCA0409", "SCA0410")]
public sealed partial class CallerArgumentExpressionAttributeSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: InvocationExpressionSyntax { ArgumentList.Arguments: [_, ..] argumentNodes } node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (operation is not IInvocationOperation { TargetMethod: var methodSymbol, Arguments: [_, ..] arguments })
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.CallerArgumentExpressionAttribute")!;
		foreach (var argument in argumentNodes)
		{
			var argumentOperation = semanticModel.GetOperation(argument, _cancellationToken);
			if (argumentOperation is not IArgumentOperation { Parameter: { Type.SpecialType: var specialType } parameter })
			{
				continue;
			}

			var attributesData = parameter.GetAttributes();
			var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
			if (attributeData is not { ConstructorArguments: [{ Value: string attributeArgumentValue }, ..] })
			{
				continue;
			}

			if (argumentNodes.All(a => a.Expression.ToString() != attributeArgumentValue))
			{
				Diagnostics.Add(Diagnostic.Create(SCA0410, argument.GetLocation(), messageArgs: null));
				continue;
			}

			if (argument is { Expression: IdentifierNameSyntax { Identifier.RawKind: (int)SyntaxKind.ArgListKeyword } })
			{
				Diagnostics.Add(Diagnostic.Create(SCA0409, argument.GetLocation(), messageArgs: null));
				continue;
			}

			if (specialType != SpecialType.System_String)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0408, argument.GetLocation(), messageArgs: null));
				continue;
			}

			Diagnostics.Add(Diagnostic.Create(SCA0407, argument.GetLocation(), messageArgs: null));
		}
	}
}
