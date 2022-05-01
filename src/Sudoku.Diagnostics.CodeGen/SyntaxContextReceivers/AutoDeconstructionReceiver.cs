namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="AutoDeconstructionGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="AutoDeconstructionGenerator"/>
internal sealed record class AutoDeconstructionReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(bool IsExtension, INamedTypeSymbol Symbol, AttributeData AttributeData)>
{
	/// <inheritdoc/>
	public ICollection<(bool IsExtension, INamedTypeSymbol Symbol, AttributeData AttributeData)> Collection { get; } =
		new List<(bool IsExtension, INamedTypeSymbol Symbol, AttributeData AttributeData)>();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		const string attributeFullName = "System.Diagnostics.CodeGen.AutoDeconstructionAttribute";
		if (
			context is not
			{
				Node: TypeDeclarationSyntax { Modifiers: var modifiers and not [] } n,
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
		var attributesData = (
			from e in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(e.AttributeClass, attributeTypeSymbol)
			select e
		).ToArray();
		if (attributesData.Length == 0)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			return;
		}

		if (!Collection.Any(t => SymbolEqualityComparer.Default.Equals(t.Symbol, typeSymbol)))
		{
			foreach (var attributeData in attributesData)
			{
				Collection.Add((false, typeSymbol, attributeData));
			}
		}
	}
}
