namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="RefStructOverridensGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="RefStructOverridensGenerator"/>
internal sealed record RefStructOverridensReceiver(CancellationToken CancellationToken)
: IResultCollectionReceiver<INamedTypeSymbol>
{
	/// <summary>
	/// Indicates the descriptor <c>SCA0013</c>
	/// (<see langword="ref struct"/>s requires the keyword <see langword="partial"/>).
	/// </summary>
	[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
	private static readonly DiagnosticDescriptor SCA0013 = new(
		id: nameof(SCA0013),
		title: "Ref structs requires the keyword 'partial'",
		messageFormat: "Ref structs requires the keyword 'partial'",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null
	);


	/// <inheritdoc/>
	public ICollection<INamedTypeSymbol> Collection { get; } = new List<INamedTypeSymbol>();

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
					Modifiers: [_, ..] modifiers
				} n,
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.RefKeyword))
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(n, CancellationToken);
		if (symbol is not { TypeKind: TypeKind.Struct } typeSymbol)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0013, identifier.GetLocation(), messageArgs: null));

			return;
		}

		Collection.Add(typeSymbol);
	}
}
