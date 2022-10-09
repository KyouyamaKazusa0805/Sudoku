namespace Sudoku.Diagnostics.CodeAnalysis;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SCA0101CodeFixProvider))]
[Shared]
public sealed class SCA0101CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(nameof(SCA0101));


	/// <inheritdoc/>
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		if (context is not
			{
				CancellationToken: var ct,
				Document: var originalDocument,
				Diagnostics: [{ Location.SourceSpan: var diagnosticSpan, Properties: var properties } diagnostic]
			})
		{
			return;
		}

		var root = (await originalDocument.GetSyntaxRootAsync(ct).ConfigureAwait(false))!;
		var targetNode = (ObjectCreationExpressionSyntax)root.FindNode(diagnosticSpan)!;
		var parentNode = root.FindNode(diagnosticSpan).Parent;
		if (parentNode is null)
		{
			return;
		}

		var suggestedMemberName = properties[SpecialNamedArgumentNames.SuggestedMemberName];
		if (suggestedMemberName is null)
		{
			return;
		}

		var typeName = properties[SpecialNamedArgumentNames.TypeName]!;
		context.RegisterCodeFix(
			CodeAction.Create(
				"Use suggested member instead",
				_ =>
				{
					var newRoot = root.ReplaceNode(
						targetNode,
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName(
								SyntaxFactory.Identifier(typeName)
							),
							SyntaxFactory.IdentifierName(
								SyntaxFactory.Identifier(suggestedMemberName)
							)
						)
					);

					return Task.FromResult(originalDocument.WithSyntaxRoot(newRoot));
				},
				nameof(SCA0101)
			),
			diagnostic);
	}
}
