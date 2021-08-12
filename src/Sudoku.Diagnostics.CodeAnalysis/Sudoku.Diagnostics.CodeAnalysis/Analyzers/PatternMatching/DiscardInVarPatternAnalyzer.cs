using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0611F", "SS0612F")]
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
			case DiscardDesignationSyntax
			{
				Parent:
					CasePatternSwitchLabelSyntax { WhenClause: null }
					or SwitchExpressionArmSyntax { WhenClause: null }
			}:
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
			case ParenthesizedVariableDesignationSyntax
			{
				Parent:
					CasePatternSwitchLabelSyntax { WhenClause: null }
					or SwitchExpressionArmSyntax { WhenClause: null },
				Variables: { Count: >= 2 } variables
			}
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
