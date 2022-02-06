namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0133", "SCA0134")]
public sealed partial class TechniqueNameAttributeSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: EnumMemberDeclarationSyntax { Identifier: var identifier } node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName("Sudoku.Techniques.TechniqueNameAttribute");
		if (attribute is null)
		{
			return;
		}

		var targetType = compilation.GetTypeByMetadataName("Sudoku.Techniques.Technique");
		if (targetType is null)
		{
			return;
		}

		var possibleFieldSymbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (possibleFieldSymbol is not IFieldSymbol { ContainingType: var containingType } fieldSymbol)
		{
			return;
		}

		var attributesData = fieldSymbol.GetAttributes();
		var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
		if (attributeData is not { ConstructorArguments: [{ Value: string techniqueName }] })
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(containingType, targetType))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0133, identifier.GetLocation(), messageArgs: null));
			return;
		}

		if (techniqueName is [' ', ..] or [.., ' '])
		{
			Diagnostics.Add(Diagnostic.Create(SCA0134, identifier.GetLocation(), messageArgs: null));
			return;
		}
	}
}
