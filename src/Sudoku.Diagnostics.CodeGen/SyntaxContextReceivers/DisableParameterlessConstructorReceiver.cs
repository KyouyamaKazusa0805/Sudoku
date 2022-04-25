namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="DisableParameterlessConstructorGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="DisableParameterlessConstructorGenerator"/>
internal sealed record class DisableParameterlessConstructorReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(INamedTypeSymbol Symbol, AttributeData, Location)>
{
	private const string BoundAttributeFullName = "System.Diagnostics.CodeGen.DisableParameterlessConstructorAttribute";


	/// <inheritdoc/>
	public ICollection<(INamedTypeSymbol Symbol, AttributeData, Location)> Collection { get; } =
		new List<(INamedTypeSymbol, AttributeData, Location)>();

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
				Node: StructDeclarationSyntax
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

		if (
			semanticModel.GetDeclaredSymbol(n, CancellationToken) is not
			{
				ContainingType: var containingTypeSymbol
			} typeSymbol
		)
		{
			return;
		}

		var referencedLocation = identifier.GetLocation();
		var attributeTypeSymbol = compilation.GetTypeByMetadataName(BoundAttributeFullName);
		var attributeData = typeSymbol.GetAttributes().FirstOrDefault(predicate);
		bool predicate(AttributeData e) => SymbolEqualityComparer.Default.Equals(e.AttributeClass, attributeTypeSymbol);
		if (attributeData is null)
		{
			return;
		}

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

		// Check whether the type contains a user-defined parameterless constructor.
		if (typeSymbol.InstanceConstructors.Any(static e => e is { Parameters: [], IsImplicitlyDeclared: false }))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0003, referencedLocation, messageArgs: null));
			return;
		}

		if (!Collection.Any(t => SymbolEqualityComparer.Default.Equals(t.Symbol, typeSymbol)))
		{
			Collection.Add((typeSymbol, attributeData, referencedLocation));
		}
	}
}
