using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0501")]
	public sealed partial class LocalFunctionNamingAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.LocalFunctionStatement });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not LocalFunctionStatementSyntax
				{
					Identifier: { ValueText: var name } identifier
				} node
			)
			{
				return;
			}

			if (name.IsCamelCase())
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0501,
					location: identifier.GetLocation(),
					messageArgs: null,
					additionalLocations: new[] { node.GetLocation() }
				)
			);
		}
	}
}
