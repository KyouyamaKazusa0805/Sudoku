using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0635")]
	public sealed partial class RemoveParameterNameAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.RecursivePattern });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not RecursivePatternSyntax
				{
					Parent: not RecursivePatternSyntax
					{
						PositionalPatternClause: { Subpatterns: { Count: not 0 } }
					},
					PositionalPatternClause: { Subpatterns: { Count: not 0 } subpatterns }
				} node
			)
			{
				return;
			}

			recursion(subpatterns);


			void recursion(in SeparatedSyntaxList<SubpatternSyntax> subpatterns)
			{
				foreach (var subpattern in subpatterns)
				{
					switch (subpattern.Pattern)
					{
						case RecursivePatternSyntax
						{
							PositionalPatternClause: { Subpatterns: { Count: not 0 } nestedSubpatterns }
						}:
						{
							recursion(nestedSubpatterns);

							break;
						}
						case DiscardPatternSyntax when subpattern.NameColon is { } nameColonNode:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0635,
									location: subpattern.GetLocation(),
									messageArgs: null,
									additionalLocations: new[] { nameColonNode.GetLocation() }
								)
							);

							break;
						}
					}
				}
			}
		}
	}
}
