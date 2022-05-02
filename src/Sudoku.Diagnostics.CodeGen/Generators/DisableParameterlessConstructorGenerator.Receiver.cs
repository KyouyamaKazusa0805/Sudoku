namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class DisableParameterlessConstructorGenerator
{
	/// <summary>
	/// The inner syntax context receiver instance.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed record class Receiver(CancellationToken CancellationToken) : ISyntaxContextReceiver
	{
		/// <summary>
		/// Indicates the result collection.
		/// </summary>
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
}
