using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes the interpolated strings.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class InterpolatedStringAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSudoku016(context);
					CheckSudoku020(context);
				},
				new SyntaxKind[] { SyntaxKind.Interpolation, SyntaxKind.InterpolatedStringExpression }
			);
		}


		private static void CheckSudoku016(SyntaxNodeAnalysisContext context)
		{
			if (
				context.SemanticModel.GetOperation(context.Node) is not IInterpolationOperation
				{
					Kind: OperationKind.Interpolation,
					Expression: { Type: { IsValueType: true } }
				}
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku016,
						title: Titles.Sudoku016,
						messageFormat: Messages.Sudoku016,
						category: Categories.Performance,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku016
					),
					location: context.Node.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSudoku020(SyntaxNodeAnalysisContext context)
		{
			var node = context.Node;
			if (node is not InterpolatedStringExpressionSyntax)
			{
				return;
			}

			if (node.DescendantNodes().OfType<InterpolationSyntax>().Any())
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku020,
						title: Titles.Sudoku020,
						messageFormat: Messages.Sudoku020,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku020
					),
					location: node.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
