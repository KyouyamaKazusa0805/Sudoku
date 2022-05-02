namespace Sudoku.Diagnostics.CodeGen.Generators;

public sealed partial class AutoOverridesGetHashCodeGenerator
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
		public ICollection<(INamedTypeSymbol Symbol, AttributeData AttributeData)> Collection { get; } =
			new List<(INamedTypeSymbol, AttributeData)>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			const string attributeFullName = "System.Diagnostics.CodeGen.AutoOverridesGetHashCodeAttribute";

			if (
				context is not
				{
					Node: TypeDeclarationSyntax
					{
						Modifiers: var modifiers and not []
					} n and (ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax),
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
			if (attributeData is not { ConstructorArguments: [{ Values: not [] }] })
			{
				return;
			}

			if (!modifiers.Any(SyntaxKind.PartialKeyword))
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
