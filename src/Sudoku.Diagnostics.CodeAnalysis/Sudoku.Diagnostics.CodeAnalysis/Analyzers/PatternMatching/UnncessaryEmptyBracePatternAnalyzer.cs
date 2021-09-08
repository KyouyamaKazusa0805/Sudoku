namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0614F")]
public sealed partial class UnncessaryEmptyBracePatternAnalyzer : DiagnosticAnalyzer
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
				Expression: var expr,
				Pattern: RecursivePatternSyntax
				{
					PropertyPatternClause: PropertyPatternClauseSyntax
					{
						Subpatterns: { Count: var count } subpatterns
					} propertyPattern,
					Designation: var designation
				} recursivePattern
			}
		)
		{
			return;
		}

		if (count == 0)
		{
			// o is { }
			// o is { } _
			// where 'o' is of non-nullable value type.
			if (semanticModel.GetOperation(expr) is { Type: (isValueType: true, _, isNullable: false) }
				&& designation is null or DiscardDesignationSyntax)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0614,
						location: propertyPattern.GetLocation(),
						messageArgs: null
					)
				);
			}
		}
		else
		{
			checkSS0614Recursively(context, subpatterns);
		}


		void checkSS0614Recursively(
			in SyntaxNodeAnalysisContext context,
			in SeparatedSyntaxList<SubpatternSyntax> subpatterns
		)
		{
			foreach (var subpattern in subpatterns)
			{
				if (
					subpattern is not
					{
						NameColon.Name: var nestedName,
						Pattern: RecursivePatternSyntax
						{
							PropertyPatternClause:
							{
								Subpatterns: { Count: var count } nestedSubpatterns
							} propertyPattern,
							Designation: var nestedDesignation
						} pattern
					}
				)
				{
					continue;
				}

				switch (count)
				{
					case 0 when semanticModel.GetOperation(nestedName) is
					{
						Type: (isValueType: true, _, isNullable: false)
					} && nestedDesignation is null or DiscardDesignationSyntax:
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0614,
								location: propertyPattern.GetLocation(),
								messageArgs: null
							)
						);

						break;
					}
					case not 0:
					{
						checkSS0614Recursively(context, nestedSubpatterns);

						break;
					}
				}
			}
		}
	}
}
