namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="AutoOverloadsComparisonOperatorsGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="AutoOverloadsComparisonOperatorsGenerator"/>
internal sealed record class AutoOverloadsComparisonOperatorsReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(INamedTypeSymbol Symbol, AttributeData)>
{
	/// <inheritdoc/>
	public ICollection<(INamedTypeSymbol Symbol, AttributeData)> Collection { get; } =
		new List<(INamedTypeSymbol, AttributeData)>();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		const string attributeFullName = "System.Diagnostics.CodeGen.AutoOverloadsComparisonOperatorsAttribute";

		if (
			context is not
			{
				Node: TypeDeclarationSyntax { Modifiers: var modifiers and not [] } n and (
					ClassDeclarationSyntax or StructDeclarationSyntax
				),
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { ContainingType: null } typeSymbol)
		{
			return;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(attributeFullName);
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a
		).FirstOrDefault();
		if (attributeData is null)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			return;
		}

		var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>();
		if (methods.Any(m => m.Name is "op_GreaterThan" or "op_GreaterThanOrEqual" or "op_LessThan" or "op_LessThanOrEqual"))
		{
			return;
		}

		if (!Collection.Any(e => SymbolEqualityComparer.Default.Equals(e.Symbol, typeSymbol)))
		{
			Collection.Add((typeSymbol, attributeData));
		}
	}
}
