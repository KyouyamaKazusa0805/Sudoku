using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0302")]
	public sealed partial class UnmeaningfulSelectClauseAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.QueryExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not QueryExpressionSyntax
				{
					FromClause:
					{
						Type: null,
						Identifier: { ValueText: var identifier }
					},
					Body:
					{
						SelectOrGroup: SelectClauseSyntax
						{
							Expression: IdentifierNameSyntax
							{
								Identifier: { ValueText: var selectClauseIdentifier }
							}
						},
						Continuation: null
					}
				} node
			)
			{
				return;
			}

			if (identifier != selectClauseIdentifier)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0302,
					location: node.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
