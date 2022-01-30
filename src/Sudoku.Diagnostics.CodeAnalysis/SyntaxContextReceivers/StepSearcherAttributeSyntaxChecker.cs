namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0126", "SCA0127", "SCA0128", "SCA0129", "SCA0130", "SCA0131", "SCA0132")]
public sealed partial class StepSearcherAttributeSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: ClassDeclarationSyntax { Identifier: var identifier } node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (
			symbol is not INamedTypeSymbol
			{
				AllInterfaces: var allInterfaces,
				InstanceConstructors: var constructors,
				IsAbstract: var isAbstract,
				IsStatic: var isStatic
			} typeSymbol
		)
		{
			return;
		}

		var stepSearcherInterface = compilation.GetTypeByMetadataName("Sudoku.Solving.Manual.Searchers.IStepSearcher");
		if (stepSearcherInterface is null)
		{
			return;
		}

		var stepSearcherAttribute = compilation.GetTypeByMetadataName("Sudoku.Solving.Manual.StepSearcherAttribute");
		if (stepSearcherAttribute is null)
		{
			return;
		}

		var attributesData = symbol.GetAttributes();
		bool hasImplThatInterface = allInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, stepSearcherInterface));
		bool hasMarkedThatAttribute = attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, stepSearcherAttribute));
		switch ((A: hasImplThatInterface, B: hasMarkedThatAttribute))
		{
			case (A: true, B: true):
			{
				break;
			}
			case (A: true, B: false):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0127, identifier.GetLocation(), messageArgs: null));
				break;
			}
			case (A: false, B: true):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0126, identifier.GetLocation(), messageArgs: null));
				return;
			}
			case (A: false, B: false):
			{
				return;
			}
		}

		if (isAbstract)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0130, identifier.GetLocation(), messageArgs: null));
			return;
		}

		if (isStatic)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0132, identifier.GetLocation(), messageArgs: null));
			return;
		}

		if (constructors.All(static c => c.Parameters.Length != 0))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0131, identifier.GetLocation(), messageArgs: null));
		}
	}
}
