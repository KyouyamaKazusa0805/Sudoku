using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the method invocation <c>ToString</c>
	/// of type <c>SudokuGrid</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class SudokuGridFormatStringAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of the sudoku grid.
		/// </summary>
		private const string SudokuGridFullTypeName = "Sudoku.Data.SudokuGrid";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.InvocationExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			/*length-pattern*/
			if (
				originalNode is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var expression,
						Name: { Identifier: { ValueText: "ToString" } } nameNode
					},
					ArgumentList: { Arguments: { Count: 0 } }
				} node
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expression) is not { Type: var possibleSudokuGridType })
			{
				return;
			}

			var sudokuGridType = compilation.GetTypeByMetadataName(SudokuGridFullTypeName);
			if (!SymbolEqualityComparer.Default.Equals(possibleSudokuGridType, sudokuGridType))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0310,
					location: nameNode.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
