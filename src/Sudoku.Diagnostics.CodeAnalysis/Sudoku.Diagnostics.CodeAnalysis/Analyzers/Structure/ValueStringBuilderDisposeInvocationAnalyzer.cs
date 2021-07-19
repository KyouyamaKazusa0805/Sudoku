using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers.Structure
{
	[CodeAnalyzer("SD0316")]
	public sealed partial class ValueStringBuilderDisposeInvocationAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.InvocationExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

			var vsbType = compilation.GetTypeByMetadataName("System.Text.ValueStringBuilder");
			if (
				originalNode is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						Expression: var instance,
						Name: { Identifier: { ValueText: "Dispose" } }
					},
					ArgumentList: { Arguments: { Count: 0 } }
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(instance, cancellationToken) is not { Type: var type })
			{
				return;
			}

			if (!SymbolEqualityComparer.Default.Equals(type, vsbType))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0316,
					location: originalNode.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
