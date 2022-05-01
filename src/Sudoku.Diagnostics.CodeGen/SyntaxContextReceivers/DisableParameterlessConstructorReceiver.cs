namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="DisableParameterlessConstructorGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="DisableParameterlessConstructorGenerator"/>
internal sealed record class DisableParameterlessConstructorReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(INamedTypeSymbol Symbol, AttributeData)>
{
	/// <inheritdoc/>
	public ICollection<(INamedTypeSymbol Symbol, AttributeData)> Collection { get; } =
		new List<(INamedTypeSymbol, AttributeData)>();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		const string attributeFullName = "System.Diagnostics.CodeGen.DisableParameterlessConstructorAttribute";

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

		var typeSymbol = semanticModel.GetDeclaredSymbol(n, CancellationToken);
		if (typeSymbol is not { ContainingType: null, InstanceConstructors: var instanceConstructors })
		{
			return;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(attributeFullName);
		bool predicate(AttributeData e) => SymbolEqualityComparer.Default.Equals(e.AttributeClass, attributeTypeSymbol);
		if (typeSymbol.GetAttributes().FirstOrDefault(predicate) is not { } attributeData)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			return;
		}

		// Check whether the type contains a user-defined parameterless constructor.
		if (instanceConstructors.Any(static e => e is { Parameters: [], IsImplicitlyDeclared: false }))
		{
			return;
		}

		if (!Collection.Any(t => SymbolEqualityComparer.Default.Equals(t.Symbol, typeSymbol)))
		{
			Collection.Add((typeSymbol, attributeData));
		}
	}
}
