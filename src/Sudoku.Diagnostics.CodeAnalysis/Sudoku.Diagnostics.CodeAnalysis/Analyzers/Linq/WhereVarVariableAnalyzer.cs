using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0314")]
public sealed partial class WhereVarVariableAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.WhereClause });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (context.Node is not WhereClauseSyntax { Condition: var condition })
		{
			return;
		}

		foreach (var possibleNode in condition.DescendantNodes())
		{
			if (possibleNode is not VarPatternSyntax { Designation: not DiscardDesignationSyntax })
			{
				continue;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0314,
					location: possibleNode.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
