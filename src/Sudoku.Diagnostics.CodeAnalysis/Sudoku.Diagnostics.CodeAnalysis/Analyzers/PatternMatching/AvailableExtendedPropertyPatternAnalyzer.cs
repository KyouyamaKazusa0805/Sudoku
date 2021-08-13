namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0624")]
public sealed partial class AvailableExtendedPropertyPatternAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.RecursivePattern });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node is not RecursivePatternSyntax
			{
				PositionalPatternClause: null,
				PropertyPatternClause.Subpatterns: var subpatterns
			} node
		)
		{
			return;
		}

		// Level-order traversal + DFS will solve the problem.
		var subpatternQueue = new Queue<SubpatternSyntax>(subpatterns);
		while (subpatternQueue.Count != 0)
		{
			var currentNode = subpatternQueue.Dequeue();
			if (
				currentNode is not
				{
					NameColon.Name.Identifier.ValueText: var baseText,
					Pattern: RecursivePatternSyntax
					{
						Type: null,
						PropertyPatternClause.Subpatterns: { Count: var count } nestedSubpatterns,
						Designation: null
					}
				}
			)
			{
				continue;
			}

			if (count == 1)
			{
				// Possible extended pattern. Now check the descendants, whether the number of all
				// nested sub-patterns are only one.
				var sb = new StringBuilder(baseText).Append('.');
				var current = nestedSubpatterns[0];
				while (true)
				{
					string? propertyName = current.NameColon?.Name.Identifier.ValueText ?? "<Unknown>";
					if (
						current.Pattern is not RecursivePatternSyntax
						{
							Type: null,
							PropertyPatternClause.Subpatterns: { Count: var innerCount } innerNestedSubpatterns,
							Designation: null
						}
					)
					{
						sb.Append(propertyName);
						break;
					}

					if (innerCount != 1)
					{
						break;
					}

					sb.Append(propertyName).Append('.');
					current = innerNestedSubpatterns[0];
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0624,
						location: currentNode.GetLocation(),
						messageArgs: new[] { sb.Append(": ").Append(current.Pattern).ToString() }
					)
				);

				return;
			}

			foreach (var nestedSubpattern in nestedSubpatterns)
			{
				subpatternQueue.Enqueue(nestedSubpattern);
			}
		}
	}
}
