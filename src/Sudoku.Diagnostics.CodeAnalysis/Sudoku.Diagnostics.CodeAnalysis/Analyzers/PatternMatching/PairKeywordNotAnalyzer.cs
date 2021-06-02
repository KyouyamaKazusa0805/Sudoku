using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the pair of the keyword <see langword="not"/>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class PairKeywordNotAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.NotPattern });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not UnaryPatternSyntax
				{
					Parent: not UnaryPatternSyntax { RawKind: (int)SyntaxKind.NotPattern },
					RawKind: (int)SyntaxKind.NotPattern,
					Pattern: var pattern
				} node
			)
			{
				return;
			}

			int count = 1;
			for (
				var patternNode = pattern;
				patternNode is UnaryPatternSyntax
				{
					RawKind: (int)SyntaxKind.NotPattern,
					Pattern: var nestedPattern
				};
				patternNode = nestedPattern,
				count++
			) ;

			if (count < 2)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0626,
					location: node.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
