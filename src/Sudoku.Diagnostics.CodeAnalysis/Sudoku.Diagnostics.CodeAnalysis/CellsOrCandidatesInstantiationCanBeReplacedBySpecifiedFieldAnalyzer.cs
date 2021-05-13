using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>,
	/// to check whether the user wrote the code like <c>new Cells()</c> or <c>default(Candidates)</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class CellsOrCandidatesInstantiationCanBeReplacedBySpecifiedFieldAnalyzer : DiagnosticAnalyzer
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
				static context => CheckSudoku021(context),
				new[]
				{
					SyntaxKind.ObjectCreationExpression,
					SyntaxKind.ImplicitObjectCreationExpression,
					SyntaxKind.DefaultExpression,
					SyntaxKind.DefaultLiteralExpression
				}
			);
		}


		private static void CheckSudoku021(SyntaxNodeAnalysisContext context)
		{
			bool isOfTypeCells;
			string typeName;
			var (semanticModel, compilation, originalNode) = context;

			switch (originalNode)
			{
				case BaseObjectCreationExpressionSyntax
				{
					ArgumentList: { Arguments: { Count: 0 } },
					Initializer: null
				} node:
				{
					if (
						semanticModel.GetOperation(node) is not IObjectCreationOperation
						{
							Kind: OperationKind.ObjectCreation,
							Type: var typeSymbol
						}
					)
					{
						return;
					}

					isOfTypeCells = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CellsFullTypeName)
					);
					if (
						!isOfTypeCells
						&& !SymbolEqualityComparer.Default.Equals(
							typeSymbol,
							compilation.GetTypeByMetadataName(CandidatesFullTypeName)
						)
					)
					{
						return;
					}

					typeName = isOfTypeCells ? CellsTypeName : CandidatesTypeName;

					break;
				}
				case DefaultExpressionSyntax node:
				{
					if (
						semanticModel.GetOperation(node) is not IDefaultValueOperation
						{
							Kind: OperationKind.DefaultValue,
							Type: var typeSymbol
						}
					)
					{
						return;
					}

					isOfTypeCells = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CellsFullTypeName)
					);
					if (
						!isOfTypeCells
						&& !SymbolEqualityComparer.Default.Equals(
							typeSymbol,
							compilation.GetTypeByMetadataName(CandidatesFullTypeName)
						)
					)
					{
						return;
					}

					typeName = isOfTypeCells ? CellsTypeName : CandidatesTypeName;

					break;
				}
				case LiteralExpressionSyntax { RawKind: (int)SyntaxKind.ImplicitObjectCreationExpression } node:
				{
					if (
						semanticModel.GetOperation(node) is not IDefaultValueOperation
						{
							Kind: OperationKind.DefaultValue,
							Type: var typeSymbol
						}
					)
					{
						return;
					}

					isOfTypeCells = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CellsFullTypeName)
					);
					if (
						!isOfTypeCells
						&& !SymbolEqualityComparer.Default.Equals(
							typeSymbol,
							compilation.GetTypeByMetadataName(CandidatesFullTypeName)
						)
					)
					{
						return;
					}

					typeName = isOfTypeCells ? CellsTypeName : CandidatesTypeName;

					break;
				}
				default:
				{
					return;
				}
			}

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
					location: originalNode.GetLocation(),
					messageArgs: new[] { typeName, EmptyPropertyName }
				)
			);
		}
	}
}
