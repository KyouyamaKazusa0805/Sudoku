using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
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
					CheckSS0101(context);
					CheckSS0301(context);
				},
				new[] { SyntaxKind.Interpolation, SyntaxKind.InterpolatedStringExpression }
			);
		}


		private static void CheckSS0101(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _) = context;
			if (
				semanticModel.GetOperation(context.Node) is not IInterpolationOperation
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
						id: DiagnosticIds.SS0101,
						title: Titles.SS0101,
						messageFormat: Messages.SS0101,
						category: Categories.Performance,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.SS0101
					),
					location: context.Node.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSS0301(SyntaxNodeAnalysisContext context)
		{
			if (context.Node is not InterpolatedStringExpressionSyntax node)
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
						id: DiagnosticIds.SS0301,
						title: Titles.SS0301,
						messageFormat: Messages.SS0301,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.SS0301
					),
					location: node.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
