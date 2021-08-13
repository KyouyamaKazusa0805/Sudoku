namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0604F")]
public sealed partial class BuiltInTypesConstantPatternAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.IsPatternExpression });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode, _, cancellationToken) = context;

		if (
			originalNode is not IsPatternExpressionSyntax
			{
				Expression: var expressionToCheck,
				Pattern: var pattern
			}
		)
		{
			return;
		}

		switch (pattern)
		{
			// o is constantValue
			case ConstantPatternSyntax
			{
				Expression: LiteralExpressionSyntax
				{
					RawKind: not (int)SyntaxKind.NullLiteralExpression
				} constantExpression
			}
			when semanticModel.TypeEquals(
				expressionToCheck, constantExpression, cancellationToken: cancellationToken
			):
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0604,
						location: originalNode.GetLocation(),
						messageArgs: new[]
						{
							expressionToCheck.ToString(),
							"==",
							constantExpression.ToString()
						},
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[] { new("EqualityToken", "==") }
						),
						additionalLocations: new[]
						{
							expressionToCheck.GetLocation(),
							constantExpression.GetLocation()
						}
					)
				);

				break;
			}

			// o is not constantValue
			case UnaryPatternSyntax
			{
				RawKind: (int)SyntaxKind.NotPattern,
				Pattern: ConstantPatternSyntax
				{
					Expression: LiteralExpressionSyntax
					{
						RawKind: not (int)SyntaxKind.NullLiteralExpression
					} constantExpression
				}
			}
			when semanticModel.TypeEquals(
				expressionToCheck, constantExpression, cancellationToken: cancellationToken
			):
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0604,
						location: originalNode.GetLocation(),
						messageArgs: new[]
						{
							expressionToCheck.ToString(),
							"!=",
							constantExpression.ToString()
						},
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[] { new("EqualityToken", "!=") }
						),
						additionalLocations: new[]
						{
							expressionToCheck.GetLocation(),
							constantExpression.GetLocation()
						}
					)
				);

				break;
			}
		}
	}
}
