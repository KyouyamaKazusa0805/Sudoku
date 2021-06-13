using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0301")]
	public sealed partial class SudokuGridAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the type name of the sudoku grid.
		/// </summary>
		private const string SudokuGridTypeName = "SudokuGrid";

		/// <summary>
		/// Indicates the field name "<c>RefreshingCandidates</c>".
		/// </summary>
		private const string RefreshingCandidatesFuncPtrName = "RefreshingCandidates";

		/// <summary>
		/// Indicates the field name "<c>ValueChanged</c>".
		/// </summary>
		private const string ValueChangedFuncPtrName = "ValueChanged";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(CheckSD0301, new[] { SyntaxKind.InvocationExpression });
		}


		private static void CheckSD0301(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: IdentifierNameSyntax { Identifier: { ValueText: SudokuGridTypeName } },
						Name: IdentifierNameSyntax
						{
							Identifier:
							{
								ValueText: var fieldName and (
									ValueChangedFuncPtrName or RefreshingCandidatesFuncPtrName
								)
							}
						}
					}
				} node
			)
			{
				return;
			}

			if (
				node.ContainingTypeIs(
					static nodeTraversing => nodeTraversing is StructDeclarationSyntax
					{
						Identifier: { ValueText: SudokuGridTypeName }
					}
				)
			)
			{
				return;
			}

			// You can't invoke them.
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0301,
					location: node.GetLocation(),
					messageArgs: new[] { fieldName }
				)
			);
		}
	}
}
