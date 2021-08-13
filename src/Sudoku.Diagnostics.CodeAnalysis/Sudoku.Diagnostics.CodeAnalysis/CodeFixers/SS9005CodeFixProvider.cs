namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS9005")]
public sealed partial class SS9005CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS9005));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, getterOrAccessorListSpan) = diagnostic.AdditionalLocations[0];
		var getterOrProperty = root.FindNode(getterOrAccessorListSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS9005,
				createChangedDocument: async c =>
				{
					SyntaxNode nodeToReplace = int.Parse(diagnostic.Properties["AccessorsCount"]!) switch
					{
						1 when getterOrProperty is PropertyDeclarationSyntax { Modifiers: var m } p =>
							p.WithModifiers(RemoveReadOnlyKeyword(m)), // Remove 'readonly' on property.
						2 when getterOrProperty is AccessorDeclarationSyntax { Modifiers: var m } g =>
							g.WithModifiers(RemoveReadOnlyKeyword(m)), // Remove 'readonly' on getter.
					};

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						getterOrProperty,
						nodeToReplace
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS9005)
			),
			diagnostic
		);
	}


	private static SyntaxTokenList RemoveReadOnlyKeyword(SyntaxTokenList oldList)
	{
		int i = 0, count = oldList.Count;
		for (; i < count; i++)
		{
			if (oldList[i].RawKind == (int)SyntaxKind.ReadOnlyKeyword)
			{
				break;
			}
		}
		if (i >= count)
		{
			throw new InvalidOperationException("The invalid case.");
		}

		return oldList.RemoveAt(i);
	}
}
