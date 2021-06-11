using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0614")]
	public sealed partial class UnncessaryEmptyBracePatternAnalyzer : DiagnosticAnalyzer
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
			var (semanticModel, _, originalNode) = context;
			if (
				originalNode is not RecursivePatternSyntax
				{
					PropertyPatternClause: PropertyPatternClauseSyntax
					{
						Subpatterns: { Count: var count } subpatterns
					} propertyPattern,
					Designation: var designation
				} recursivePattern
			)
			{
				return;
			}

			if (count == 0)
			{
				if (
					semanticModel.GetOperation(recursivePattern) is IRecursivePatternOperation
					{
						MatchedType: (isValueType: true, _, _)
					} && designation is null
				)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0614,
							location: propertyPattern.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
			else
			{
				checkSS0614Recursively(context, subpatterns);
			}


			void checkSS0614Recursively(
				in SyntaxNodeAnalysisContext context, in SeparatedSyntaxList<SubpatternSyntax> subpatterns)
			{
				foreach (var subpattern in subpatterns)
				{
					if (
						subpattern is not
						{
							NameColon: not null,
							Pattern: RecursivePatternSyntax
							{
								PropertyPatternClause:
								{
									Subpatterns: { Count: var count } nestedSubpatterns
								} propertyPattern
							} pattern
						}
					)
					{
						continue;
					}

					switch (count)
					{
						case 0
						when semanticModel.GetOperation(pattern) is IRecursivePatternOperation
						{
							MatchedType: (isValueType: true, _, _)
						} && designation is null:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0614,
									location: propertyPattern.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
						case not 0:
						{
							checkSS0614Recursively(context, nestedSubpatterns);

							break;
						}
					}
				}
			}
		}
	}
}
