namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="AutoDeconstructionGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="AutoDeconstructionGenerator"/>
internal sealed record class AutoDeconstructionReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(bool IsExtension, INamedTypeSymbol Symbol, AttributeData AttributeData, Location Location)>
{
	private const string BoundAttributeFullNameInstance = "System.Diagnostics.CodeGen.AutoDeconstructionAttribute";


	/// <inheritdoc/>
	public ICollection<(bool IsExtension, INamedTypeSymbol Symbol, AttributeData AttributeData, Location Location)> Collection { get; } =
		new List<(bool IsExtension, INamedTypeSymbol Symbol, AttributeData AttributeData, Location Location)>();

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
				} n,
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		var typeSymbol = semanticModel.GetDeclaredSymbol(n, CancellationToken);
		if (typeSymbol is not { ContainingType: var containingTypeSymbol })
		{
			return;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(BoundAttributeFullNameInstance);
		var attributesData = (
			from e in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(e.AttributeClass, attributeTypeSymbol)
			select e
		).ToArray();
		if (attributesData.Length == 0)
		{
			return;
		}

		var referencedLocation = identifier.GetLocation();
		if (containingTypeSymbol is not null)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0006, referencedLocation, messageArgs: null));
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0002, referencedLocation, messageArgs: null));
			return;
		}

		if (!Collection.Any(t => SymbolEqualityComparer.Default.Equals(t.Symbol, typeSymbol)))
		{
			foreach (var attributeData in attributesData)
			{
				Collection.Add((false, typeSymbol, attributeData, referencedLocation));
			}
		}
	}
}
