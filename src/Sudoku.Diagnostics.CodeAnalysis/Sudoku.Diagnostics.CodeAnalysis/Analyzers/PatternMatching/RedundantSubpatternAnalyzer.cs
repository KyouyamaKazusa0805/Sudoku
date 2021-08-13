namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0609")]
public sealed partial class RedundantSubpatternAnalyzer : DiagnosticAnalyzer
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
		var (semanticModel, _, originalNode) = context;

		if (
			originalNode is not IsPatternExpressionSyntax
			{
				Pattern: var pattern
			} node
		)
		{
			return;
		}

		var subpatterns = new List<(SubpatternSyntax Node, string BoundParameterName)>();
		foreach (var descendantPattern in pattern.DescendantNodes().OfType<PositionalPatternClauseSyntax>())
		{
			if (
				descendantPattern is not
				{
					Parent: RecursivePatternSyntax parentNode,
					Subpatterns: { Count: >= 2 } currentSubpatterns
				}
			)
			{
				continue;
			}

			if (
				semanticModel.GetOperation(parentNode) is not IRecursivePatternOperation
				{
					DeconstructSymbol: IMethodSymbol { Parameters: var boundMethodParameters },
					MatchedType: { } type
				} operation
			)
			{
				return;
			}

			for (int i = 0, count = currentSubpatterns.Count; i < count; i++)
			{
				switch (currentSubpatterns[i])
				{
					case { NameColon: null, Pattern: not DiscardPatternSyntax } currentSubpattern:
					{
						string boundParameterName = boundMethodParameters[i].Name;
						subpatterns.Add((currentSubpattern, boundParameterName));

						break;
					}
					case
					{
						NameColon: { Name: { Identifier: { ValueText: var parameterName } } } nameColonNode,
						Pattern: not DiscardPatternSyntax
					} currentSubpattern:
					{
						subpatterns.Add((currentSubpattern, parameterName));

						break;
					}
				}
			}
		}

		for (int i = 0, count = subpatterns.Count, iterationCount = count - 1; i < iterationCount; i++)
		{
			var (_, firstName) = subpatterns[i];
			for (int j = i + 1; j < count; j++)
			{
				var (secondSubpattern, secondName) = subpatterns[j];
				if (firstName == secondName)
				{
					// Duplicate judgement.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0609,
							location: secondSubpattern.GetLocation(),
							messageArgs: new[] { secondSubpattern.ToString() }
						)
					);
				}
			}
		}
	}
}
