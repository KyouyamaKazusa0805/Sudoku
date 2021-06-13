using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0611", "SS0612")]
	public sealed partial class DiscardInVarPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.VarPattern });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (context.Node is not VarPatternSyntax { Designation: var designation } node)
			{
				return;
			}

			switch (designation)
			{
				case DiscardDesignationSyntax:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0611,
							location: node.GetLocation(),
							messageArgs: new[] { node.ToString() }
						)
					);

					break;
				}
				/*length-pattern*/
				case ParenthesizedVariableDesignationSyntax { Variables: { Count: >= 2 } variables }
				when variables.All(static variable => variable is DiscardDesignationSyntax):
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0612,
							location: node.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}
			}
		}
	}
}
