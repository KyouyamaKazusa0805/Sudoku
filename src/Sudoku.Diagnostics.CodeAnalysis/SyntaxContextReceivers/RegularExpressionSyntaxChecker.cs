namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0103", "SCA0104", "SCA0105", "SCA0106", "SCA0107")]
public sealed partial class RegularExpressionSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: FieldDeclarationSyntax { Declaration.Variables: { Count: not 0 } variables },
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName(typeof(IsRegexAttribute).FullName);
		foreach (var variable in variables)
		{
			var symbol = semanticModel.GetDeclaredSymbol(variable, _cancellationToken);
			if (
				symbol is not IFieldSymbol
				{
					Type: { SpecialType: var specialTypeOfTypeSymbol },
					DeclaringSyntaxReferences: { Length: not 0 } syntaxReferences,
					IsStatic: var isStatic,
					IsReadOnly: var isReadOnly,
					IsConst: var isConstant
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

			var syntaxReference = syntaxReferences[0];
			var syntaxNode = syntaxReference.GetSyntax(_cancellationToken);

			if (!(isStatic && isReadOnly || isConstant))
			{
				Diagnostics.Add(Diagnostic.Create(SCA0107, syntaxNode.GetLocation(), messageArgs: null));

				continue;
			}

			if (specialTypeOfTypeSymbol != SpecialType.System_String)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0103, syntaxNode.GetLocation(), messageArgs: null));

				continue;
			}

			if (
				syntaxNode is not VariableDeclaratorSyntax
				{
					Initializer:
					{
						Value: var stringLiteralExpression and (
							LiteralExpressionSyntax { RawKind: (int)SyntaxKind.StringLiteralExpression }
							or InterpolatedStringExpressionSyntax
						)
					}
				}
			)
			{
				continue;
			}

			switch (stringLiteralExpression)
			{
				// $@"content"
				// @$"content"
				// $"content"
				case InterpolatedStringExpressionSyntax
				{
					StringStartToken.ValueText: var stringStartToken,
					Contents: { Count: not 0 } contents
				}:
				{
					//if (stringStartToken == @"$""")
					//{
					//	Diagnostics.Add(Diagnostic.Create(SCA0105, syntaxNode.GetLocation(), messageArgs: null));
					//
					//	continue;
					//}

					foreach (var content in contents)
					{
						if (content is not InterpolationSyntax { Expression: var interpolationExpr })
						{
							continue;
						}

						var interpolationExprOperation = semanticModel.GetOperation(interpolationExpr, _cancellationToken);
						if (
							interpolationExprOperation is not IFieldReferenceOperation
							{
								Field:
								{
									IsConst: true,
									HasConstantValue: true,
									ConstantValue: string realInterpolationValue
								}
							}
						)
						{
							continue;
						}

						if (RegexExtensions.TryMatch(string.Empty, realInterpolationValue, out _))
						{
							continue;
						}

						Diagnostics.Add(Diagnostic.Create(SCA0106, content.GetLocation(), messageArgs: null));
					}

					break;
				}

				// @"content"
				// "content"
				case LiteralExpressionSyntax { Token: { ValueText: var literalTokenValue } literalToken }:
				{
					if (literalToken.ToString()[0] != '@')
					{
						Diagnostics.Add(Diagnostic.Create(SCA0105, literalToken.GetLocation(), messageArgs: null));

						continue;
					}

					if (RegexExtensions.TryMatch(string.Empty, literalTokenValue, out _))
					{
						continue;
					}

					Diagnostics.Add(Diagnostic.Create(SCA0106, literalToken.GetLocation(), messageArgs: null));

					break;
				}
			}
		}
	}
}
