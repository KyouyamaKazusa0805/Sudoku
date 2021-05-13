using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>,
	/// to check whether the user wrote the code like <c>cells.Count == 0</c> or <c>candidateList.Count != 0</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class CellsOrCandidatesCountIsEqualToZeroAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the zero string.
		/// </summary>
		private const string ZeroString = "0";

		/// <summary>
		/// Indicates the property name of <c>Count</c>.
		/// </summary>
		private const string CountPropertyName = "Count";

		/// <summary>
		/// Indicates the full type name of <c>Cells</c>.
		/// </summary>
		private const string CellsFullTypeName = "Sudoku.Data.Cells";

		/// <summary>
		/// Indicates the full type name of <c>Candidates</c>.
		/// </summary>
		private const string CandidatesFullTypeName = "Sudoku.Data.Candidates";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context => CheckSudoku018(context),
				new[]
				{
					SyntaxKind.EqualsExpression,
					SyntaxKind.NotEqualsExpression,
					SyntaxKind.DefaultExpression,
					SyntaxKind.DefaultLiteralExpression
				}
			);
		}


		private static void CheckSudoku018(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not BinaryExpressionSyntax
				{
					RawKind: var kind,
					Left: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var exprNode,
						Name: { Identifier: { ValueText: CountPropertyName } }
					},
					Right: LiteralExpressionSyntax
					{
						RawKind: (int)SyntaxKind.NumericLiteralExpression,
						Token: { ValueText: ZeroString }
					}
				} node
			)
			{
				return;
			}

			if (
				context.SemanticModel.GetOperation(exprNode) is not
				{
					Kind: OperationKind.LocalReference,
					Type: var typeSymbol
				}
			)
			{
				return;
			}

			var compilation = context.Compilation;
			if (
				!SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					compilation.GetTypeByMetadataName(CellsFullTypeName)
				)
				&& !SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					compilation.GetTypeByMetadataName(CandidatesFullTypeName)
				)
			)
			{
				return;
			}

			string equalityToken = kind == (int)SyntaxKind.NotEqualsExpression ? "!=" : "==";
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku018,
						title: Titles.Sudoku018,
						messageFormat: Messages.Sudoku018,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku018
					),
					location: node.GetLocation(),
					messageArgs: new[]
					{
						exprNode.ToString(),
						equalityToken,
						equalityToken == "==" ? string.Empty : "!"
					}
				)
			);
		}
	}
}
