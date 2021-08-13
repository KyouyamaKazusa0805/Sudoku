namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0620")]
public sealed partial class UnnecessaryNotInRelationPatternAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.NotPattern });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node is not UnaryPatternSyntax
			{
				Pattern: RelationalPatternSyntax
				{
					OperatorToken.RawKind: var relationalToken,
					Expression: var relationalExpr
				}
			} node
		)
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0620,
				location: node.GetLocation(),
				messageArgs: null,
				properties: ImmutableDictionary.CreateRange(
					new KeyValuePair<string, string?>[]
					{
						new("RelationalToken", relationalToken switch
						{
							(int)SyntaxKind.GreaterThanToken => ">",
							(int)SyntaxKind.GreaterThanEqualsToken => ">=",
							(int)SyntaxKind.LessThanToken => "<",
							(int)SyntaxKind.LessThanEqualsToken => "<="
						})
					}
				),
				additionalLocations: new[] { relationalExpr.GetLocation() }
			)
		);
	}
}
