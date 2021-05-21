using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for closed <see langword="enum"/> types.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class ForLoopBadConditionAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.ForStatement });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not ForStatementSyntax
				{
					Condition: BinaryExpressionSyntax
					{
						RawKind:
							(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
							or (int)SyntaxKind.GreaterThanExpression or (int)SyntaxKind.GreaterThanEqualsToken
							or (int)SyntaxKind.LessThanExpression or (int)SyntaxKind.LessThanEqualsToken,
						Left: var leftExpr,
						Right: var rightExpr
					}
				} node
			)
			{
				return;
			}

			// Many times we write complex expression and place them right, such as the following expression:
			//   i > arr.Length
			// where the expression 'arr.Length' is a complex expression that is at the right side.
			// Therefore, the analyzer will check the right expression at first.
			foreach (var expressionNode in new[] { rightExpr, leftExpr })
			{
				if (isSimpleExpression(expressionNode))
				{
					continue;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS9001,
						location: expressionNode.GetLocation(),
						additionalLocations: new[] { node.GetLocation() },
						messageArgs: null
					)
				);
			}


			static bool isSimpleExpression(ExpressionSyntax expression) =>
				expression is LiteralExpressionSyntax or DefaultExpressionSyntax or IdentifierNameSyntax;
		}
	}
}
