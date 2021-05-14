using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that checks the usage of the type <c>SudokuGrid</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
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

			context.RegisterSyntaxNodeAction(
				static context => CheckSudoku014(context),
				new[] { SyntaxKind.InvocationExpression }
			);
		}


		private static void CheckSudoku014(SyntaxNodeAnalysisContext context)
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
					descriptor: new(
						id: DiagnosticIds.Sudoku014,
						title: Titles.Sudoku014,
						messageFormat: Messages.Sudoku014,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku014
					),
					location: node.GetLocation(),
					messageArgs: new[] { fieldName }
				)
			);
		}
	}
}
