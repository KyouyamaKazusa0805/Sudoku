namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0103", "SCA0104", "SCA0105", "SCA0106")]
public sealed partial class RegularExpressionSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: FieldDeclarationSyntax { Declaration.Variables: [_, ..] variables },
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var attribute = compilation.GetTypeSymbol<IsRegexAttribute>();
		foreach (var variable in variables)
		{
			if (
				variable is not
				{
					Initializer.Value: LiteralExpressionSyntax
					{
						RawKind: (int)SyntaxKind.StringLiteralExpression,
						Token: var literalToken
					} stringLiteralExpression
				}
			)
			{
				continue;
			}

			var symbol = semanticModel.GetDeclaredSymbol(variable, _cancellationToken);
			if (
				symbol is not IFieldSymbol
				{
					Type.SpecialType: var specialTypeOfTypeSymbol,
					DeclaringSyntaxReferences: [_, ..],
					IsConst: var isConstant,
					HasConstantValue: var hasConstant,
					ConstantValue: var constantValue
				}
			)
			{
				continue;
			}

			var attributesData = symbol.GetAttributes();
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			if (!isConstant || !hasConstant || constantValue is not string realConstantValue)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0106, variable.GetLocation(), messageArgs: null));

				continue;
			}

			if (specialTypeOfTypeSymbol != SpecialType.System_String)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0103, variable.GetLocation(), messageArgs: null));

				continue;
			}

			if (literalToken.ToString()[0] != '@')
			{
				Diagnostics.Add(Diagnostic.Create(SCA0105, literalToken.GetLocation(), messageArgs: null));

				continue;
			}

			if (RegexExtensions.TryMatch(string.Empty, realConstantValue, out _))
			{
				continue;
			}

			Diagnostics.Add(Diagnostic.Create(SCA0104, literalToken.GetLocation(), messageArgs: null));
		}
	}
}
