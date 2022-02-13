namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0603", "SCA0604")]
public sealed partial class PropertyStyleSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
#pragma warning disable IDE0055
			context is not
			{
				Node: PropertyDeclarationSyntax
				{
					Identifier: var identifier,
					AccessorList.Accessors: [
						{
							Keyword.ValueText: var getterKeyword,
							Body: var getterBody,
							SemicolonToken.TrailingTrivia: var getterSemicolonTrailingTrivia
						} getter,
						{
							Modifiers: var modifiers,
							Keyword: { ValueText: var setterKeyword, LeadingTrivia: var setterLeadingTrivia },
							AttributeLists: var attributeLists
						}
					]
				} node,
				SemanticModel.SyntaxTree.FilePath: var filePath
			}
#pragma warning restore IDE0055
		)
		{
			return;
		}

		if (filePath.EndsWith(".Designer.cs") || Regex.IsMatch(filePath, @".+\.g(\.\w+)?\.cs"))
		{
			// Don't check on generated files.
			return;
		}

		if (getterKeyword == "get" && setterKeyword is "set" or "init")
		{
			if (
				getterBody switch
				{
					null when getterSemicolonTrailingTrivia.Any(SyntaxKind.EndOfLineTrivia) => true,
					{ CloseBraceToken.TrailingTrivia: var a } when a.Any(SyntaxKind.EndOfLineTrivia) => true,
					_ => false
				} && !(
					(Modifiers: modifiers, Attributes: attributeLists) switch
					{
						(Modifiers: [], _) when setterLeadingTrivia.Any(SyntaxKind.EndOfLineTrivia) => true,
						(Modifiers: [{ LeadingTrivia: var leadingTrivia }, ..], _) when leadingTrivia.Any(SyntaxKind.EndOfLineTrivia) => true,
						(_, Attributes: [{ OpenBracketToken.LeadingTrivia: var leadingTrivia }, ..]) => leadingTrivia.Any(SyntaxKind.EndOfLineTrivia),
						_ => false
					}
				)
			)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0604, identifier.GetLocation(), messageArgs: null));
			}
		}
		else if (setterKeyword == "get" && getterKeyword is "set" or "init")
		{
			Diagnostics.Add(Diagnostic.Create(SCA0603, identifier.GetLocation(), messageArgs: null));
		}
	}
}
