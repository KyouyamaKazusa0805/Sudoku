namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0107", "SCA0108", "SCA0109")]
public sealed partial class RestrictArgumentsSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: var node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName(typeof(RestrictAttribute).FullName)!;
		CheckOnMethodDeclaration(node, semanticModel, attribute);
		CheckOnMethodInvocation(node, semanticModel, attribute);
	}

	private void CheckOnMethodDeclaration(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol attribute)
	{
		if (node is not MethodDeclarationSyntax { Identifier: var identifier })
		{
			return;
		}

		var methodSymbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (
			methodSymbol is not IMethodSymbol
			{
				Parameters: [_, ..] parameters,
				IsAbstract: false,
				IsExtern: false
			}
		)
		{
			return;
		}

		int count = 0;
		foreach (var parameter in parameters)
		{
			if (
				parameter is not
				{
					Type: var parameterType,
					DeclaringSyntaxReferences: [{ SyntaxTree: var syntaxTree, Span: var span }, ..]
				}
			)
			{
				continue;
			}

			var attributesData = parameter.GetAttributes();
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			if (parameterType is IPointerTypeSymbol or IFunctionPointerTypeSymbol)
			{
				count++;

				continue;
			}

			var location = Location.Create(syntaxTree, span);
			Diagnostics.Add(Diagnostic.Create(SCA0107, location, messageArgs: null));
		}

		if (count == 1)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0108, identifier.GetLocation(), messageArgs: null));
		}
	}

	private void CheckOnMethodInvocation(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol attribute)
	{
		var nodeOperation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			nodeOperation is not IInvocationOperation
			{
				TargetMethod: { IsAbstract: false, IsExtern: false },
				Arguments: [_, ..] arguments
			}
		)
		{
			return;
		}

		var restrictArguments = new List<IArgumentOperation>();
		foreach (var argument in arguments)
		{
			if (
				argument is not
				{
					ArgumentKind: ArgumentKind.Explicit,
					Parameter: { Type: IPointerTypeSymbol or IFunctionPointerTypeSymbol } parameter
				}
			)
			{
				continue;
			}

			var attributesData = parameter.GetAttributes();
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			restrictArguments.Add(argument);
		}

		for (int i = 0, length = restrictArguments.Count, outerLength = length - 1; i < outerLength; i++)
		{
			for (int j = i + 1; j < length; j++)
			{
				if (
					(Left: restrictArguments[i], Right: restrictArguments[j]) is not (
						Left: { Syntax: var leftSyntax },
						Right: { Syntax: var rightSyntax }
					)
				)
				{
					continue;
				}

				if (leftSyntax.ToString() != rightSyntax.ToString())
				{
					continue;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0109, rightSyntax.GetLocation(), messageArgs: null));
			}
		}
	}
}
