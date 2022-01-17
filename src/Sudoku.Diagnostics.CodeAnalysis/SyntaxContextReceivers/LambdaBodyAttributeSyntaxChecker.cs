namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0117", "SCA0118")]
public sealed partial class LambdaBodyAttributeSyntaxChecker : ISyntaxContextReceiver
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

		const string attributeFullName = "Sudoku.Diagnostics.CodeGen.LambdaBodyAttribute";
		var attribute = compilation.GetTypeByMetadataName(attributeFullName);
		if (attribute is null)
		{
			return;
		}

		switch (node)
		{
			case AccessorDeclarationSyntax { Keyword: { RawKind: (int)SyntaxKind.SetKeyword } keyword }
			when semanticModel.GetDeclaredSymbol(node, _cancellationToken) is IMethodSymbol symbol
			&& symbol.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0117, keyword.GetLocation(), messageArgs: null));

				break;
			}

			case AccessorDeclarationSyntax
			{
				Keyword: { RawKind: (int)SyntaxKind.GetKeyword } keyword,
				ExpressionBody: null,
				Body: not null
			}
			when semanticModel.GetDeclaredSymbol(node, _cancellationToken) is IMethodSymbol symbol
			&& symbol.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0118, keyword.GetLocation(), messageArgs: null));

				break;
			}

			case MethodDeclarationSyntax { Identifier: var identifier, ExpressionBody: null, Body: not null }
			when semanticModel.GetDeclaredSymbol(node, _cancellationToken) is { } symbol
			&& symbol.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0118, identifier.GetLocation(), messageArgs: null));

				break;
			}
		}
	}
}
