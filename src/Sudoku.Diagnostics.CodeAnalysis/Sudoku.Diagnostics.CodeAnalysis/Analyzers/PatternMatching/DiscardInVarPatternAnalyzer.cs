using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for:
	/// <list type="bullet">
	/// <item>
	/// Discards <see langword="_"/> in the <see langword="var"/> pattern, e.g.
	/// <c><see langword="is var _"/></c>.
	/// </item>
	/// <item>
	/// Deconstruction discards, e.g.<c><see langword="is var"/> (<see langword="_"/>, <see langword="_"/>)</c>.
	/// </item>
	/// </list>
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
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
				/*slice-pattern*/
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
