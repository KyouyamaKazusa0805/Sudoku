using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for <see langword="record"/> <c>ToString</c> invocation,
	/// and the type is recursive.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class RecordToStringAnalyzer : DiagnosticAnalyzer
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
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var expr,
						Name: { Identifier: { ValueText: "ToString" } }
					},
					ArgumentList: { Arguments: { Count: 0 } }
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: { IsRecord: true } type })
			{
				return;
			}

			/*slice-pattern*/
			if (
				!type.GetMembers().OfType<IMethodSymbol>().Any(
					static method => method is { IsImplicitlyDeclared: true, Name: "ToString" }
				)
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS9004,
					location: originalNode.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
