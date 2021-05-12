using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class CellsOrCandidatesAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the cells type name.
		/// </summary>
		private const string CellsTypeName = "Cells";

		/// <summary>
		/// Indicates the candidates type name.
		/// </summary>
		private const string CandidatesTypeName = "Candidates";

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

		/// <summary>
		/// Indicates the property name to check in the diagnostic result <c>SUDOKU021</c>.
		/// </summary>
		private const string EmptyPropertyName = "Empty";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSudoku018(context);
					CheckSudoku021(context);
				},
				new SyntaxKind[] { SyntaxKind.InvocationExpression }
			);
		}


		private static void CheckSudoku018(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not BinaryExpressionSyntax
				{
					RawKind: var kind and (
						(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					),
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

		private static void CheckSudoku021(SyntaxNodeAnalysisContext context)
		{
			var syntaxTree = context.Node.SyntaxTree;
			var walker = new Sudoku021SyntaxWalker(context.SemanticModel, context.Compilation);
			walker.Visit(syntaxTree.GetRoot());

			// If the syntax tree doesn't contain any dynamically called clause,
			// just skip it.
			if (walker.Collection is null)
			{
				return;
			}

			// Iterate on each dynamically called location.
			foreach (var (typeName, node) in walker.Collection)
			{
				// You can't invoke them.
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.Sudoku021,
							title: Titles.Sudoku021,
							messageFormat: Messages.Sudoku021,
							category: Categories.Performance,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.Sudoku021
						),
						location: node.GetLocation(),
						messageArgs: new[] { typeName, EmptyPropertyName }
					)
				);
			}
		}
	}
}
