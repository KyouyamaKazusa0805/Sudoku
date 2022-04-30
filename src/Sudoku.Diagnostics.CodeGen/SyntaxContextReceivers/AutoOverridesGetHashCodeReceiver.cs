namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="AutoOverridesGetHashCodeGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="AutoOverridesGetHashCodeGenerator"/>
internal sealed record class AutoOverridesGetHashCodeReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(INamedTypeSymbol Symbol, AttributeData AttributeData, SyntaxToken Identifier)>
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoOverridesGetHashCodeAttribute";


	/// <inheritdoc/>
	public ICollection<(INamedTypeSymbol Symbol, AttributeData AttributeData, SyntaxToken Identifier)> Collection { get; } =
		new List<(INamedTypeSymbol, AttributeData, SyntaxToken)>();

	/// <summary>
	/// Indicates the diagnostic results found.
	/// </summary>
	internal List<Diagnostic> Diagnostics { get; } = new();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: TypeDeclarationSyntax
				{
					Identifier: var identifier,
					Modifiers: { Count: not 0 } modifiers
				} n and (ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax),
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { ContainingType: var containingType } typeSymbol)
		{
			return;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a
		).FirstOrDefault();
		if (attributeData is not { ConstructorArguments: [{ Values: var values }] })
		{
			return;
		}

		if (values is [])
		{
			Diagnostics.Add(Diagnostic.Create(SCA0007, identifier.GetLocation(), messageArgs: null));
			return;
		}

		if (containingType is not null)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0006, identifier.GetLocation(), messageArgs: null));
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0002, identifier.GetLocation(), messageArgs: null));
			return;
		}

		if (!Collection.Any(t => SymbolEqualityComparer.Default.Equals(t.Symbol, typeSymbol)))
		{
			Collection.Add((typeSymbol, attributeData, identifier));
		}
	}
}
