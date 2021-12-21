namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0603", "SCA0604")]
public sealed partial class PropertyStyleSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context.Node is not PropertyDeclarationSyntax
			{
				Identifier: var identifier,
				AccessorList.Accessors: { Count: 2 } accessors
			} node
		)
		{
			return;
		}

		if (
			(Getter: accessors[0], Setter: accessors[1]) is not (
				Getter: { Keyword.ValueText: var getterKeyword, Body: var getterBody } getter,
				Setter:
				{
					Modifiers: { Count: var modifiersCount } modifiers,
					Keyword: { ValueText: var setterKeyword, LeadingTrivia: var setterLeadingTrivia },
					AttributeLists: { Count: var attributesCount } attributeLists
				}
			)
		)
		{
			return;
		}

		if (getterKeyword == "get" && setterKeyword == "set")
		{
			bool setterHasLeadingCrLf = modifiersCount switch
			{
				0 when setterLeadingTrivia.Any(SyntaxKind.EndOfLineTrivia) => true,
				not 0 when modifiers[0].LeadingTrivia.Any(SyntaxKind.EndOfLineTrivia) => true,
				_ => attributesCount != 0 && attributeLists[0].OpenBracketToken.LeadingTrivia.Any(SyntaxKind.EndOfLineTrivia)
			};

			bool getterHasTrailingCrLf = getterBody switch
			{
				null when getter.SemicolonToken.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia) => true,
				{ CloseBraceToken.TrailingTrivia: var a } when a.Any(SyntaxKind.EndOfLineTrivia) => true,
				_ => false
			};

			if (getterHasTrailingCrLf && !setterHasLeadingCrLf)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0604, identifier.GetLocation(), messageArgs: null));
			}
		}
		else if (setterKeyword == "get" && getterKeyword == "set")
		{
			Diagnostics.Add(Diagnostic.Create(SCA0603, identifier.GetLocation(), messageArgs: null));
		}
	}
}
