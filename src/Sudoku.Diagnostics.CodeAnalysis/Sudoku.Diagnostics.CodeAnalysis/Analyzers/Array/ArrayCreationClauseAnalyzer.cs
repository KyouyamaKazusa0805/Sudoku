using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS9002")]
	public sealed partial class ArrayCreationClauseAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.VariableDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			/*length-pattern*/
			if (
				context.Node is not VariableDeclarationSyntax
				{
					Type: ArrayTypeSyntax { RankSpecifiers: { Count: 1 } },
					Variables: { Count: not 0 } variables
				} node
			)
			{
				return;
			}

			foreach (var variable in variables)
			{
				if (
					variable is not
					{
						Initializer:
						{
							Value: ArrayCreationExpressionSyntax
							{
								Type: var type,
								Initializer: { } initializer
							} value
						}
					}
				)
				{
					continue;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS9002,
						location: type.GetLocation(),
						additionalLocations: new[] { initializer.GetLocation(), value.GetLocation() },
						messageArgs: null
					)
				);
			}
		}
	}
}
