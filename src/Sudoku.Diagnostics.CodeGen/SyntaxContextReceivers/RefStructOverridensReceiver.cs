namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="RefStructOverridensGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="RefStructOverridensGenerator"/>
internal sealed record class RefStructOverridensReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(INamedTypeSymbol Symbol, bool, bool)>
{
	private const string
		GetHashCodeAttributeFullName = "System.Diagnostics.CodeGen.AutoOverridesGetHashCodeAttribute",
		ToStringAttributeFullName = "System.Diagnostics.CodeGen.AutoOverridesToStringAttribute";


	/// <inheritdoc/>
	public ICollection<(INamedTypeSymbol Symbol, bool, bool)> Collection { get; } =
		new List<(INamedTypeSymbol, bool, bool)>();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: StructDeclarationSyntax { Modifiers: var modifiers and not [] } n,
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.RefKeyword) || !modifiers.Any(SyntaxKind.PartialKeyword))
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { } typeSymbol)
		{
			return;
		}

		var attributesData = typeSymbol.GetAttributes();
		var getHashCodeAttribute = compilation.GetTypeByMetadataName(GetHashCodeAttributeFullName);
		var toStringAttribute = compilation.GetTypeByMetadataName(ToStringAttributeFullName);
		bool mayGenerateGetHashCode = !attributesData.Any(e => SymbolEqualityComparer.Default.Equals(e.AttributeClass, getHashCodeAttribute));
		bool mayGenerateToString = !attributesData.Any(e => SymbolEqualityComparer.Default.Equals(e.AttributeClass, toStringAttribute));

		Collection.Add((typeSymbol, mayGenerateGetHashCode, mayGenerateToString));
	}
}
