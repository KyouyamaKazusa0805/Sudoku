namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class RefStructOverridensGenerator
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
		public ICollection<INamedTypeSymbol> Collection { get; } = new List<INamedTypeSymbol>();


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

			Collection.Add(typeSymbol);
		}
	}
}
