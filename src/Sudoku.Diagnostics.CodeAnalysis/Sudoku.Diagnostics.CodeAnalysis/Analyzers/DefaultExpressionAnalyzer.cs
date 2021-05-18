using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c>, <c>Candidates</c> and <c>SudokuGrid</c>,
	/// to check whether the user wrote the code like <c>new Cells()</c> or <c>default(Candidates)</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class DefaultExpressionAnalyzer : DiagnosticAnalyzer
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
		/// Indicates the sudoku grid type name.
		/// </summary>
		private const string SudokuGridTypeName = "SudokuGrid";

		/// <summary>
		/// Indicates the full type name of <c>Cells</c>.
		/// </summary>
		private const string CellsFullTypeName = "Sudoku.Data.Cells";

		/// <summary>
		/// Indicates the full type name of <c>Candidates</c>.
		/// </summary>
		private const string CandidatesFullTypeName = "Sudoku.Data.Candidates";

		/// <summary>
		/// Indicates the full type name of the sudoku grid.
		/// </summary>
		private const string SudokuGridFullTypeName = "Sudoku.Data.SudokuGrid";

		/// <summary>
		/// Indicates the field name to check in the diagnostic result <c>SUDOKU021</c>.
		/// </summary>
		private const string EmptyPropertyName = "Empty";

		/// <summary>
		/// Indicates the field name to check in the diagnostic result <c>SUDOKU021</c>.
		/// </summary>
		private const string SudokuGridEmptyPropertyName = "Undefined";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				CheckSD0303,
				new[]
				{
					SyntaxKind.ObjectCreationExpression,
					SyntaxKind.ImplicitObjectCreationExpression,
					SyntaxKind.DefaultExpression,
					SyntaxKind.DefaultLiteralExpression
				}
			);
		}


		private static void CheckSD0303(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;
			string typeName;
			bool isOfTypeSudokuGrid;
			switch (originalNode)
			{
				case BaseObjectCreationExpressionSyntax
				{
					Parent: not EqualsValueClauseSyntax { Parent: ParameterSyntax },
					ArgumentList: { Arguments: { Count: 0 } },
					Initializer: null
				} node
				when semanticModel.GetOperation(node) is IObjectCreationOperation
				{
					Kind: OperationKind.ObjectCreation,
					Type: var typeSymbol
				}:
				{
					bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CellsFullTypeName)
					);
					bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					);
					isOfTypeSudokuGrid = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
					);

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					typeName = isOfTypeCells
						? CellsTypeName
						: isOfTypeCandidates ? CandidatesTypeName : SudokuGridTypeName;

					break;
				}
				case DefaultExpressionSyntax
				{
					Parent: not EqualsValueClauseSyntax { Parent: ParameterSyntax }
				} node
				when semanticModel.GetOperation(node) is IDefaultValueOperation
				{
					Kind: OperationKind.DefaultValue,
					Type: var typeSymbol
				}:
				{
					bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CellsFullTypeName)
					);
					bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					);
					isOfTypeSudokuGrid = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
					);

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					typeName = isOfTypeCells
						? CellsTypeName
						: isOfTypeCandidates ? CandidatesTypeName : SudokuGridTypeName;

					break;
				}
				case LiteralExpressionSyntax
				{
					Parent: not EqualsValueClauseSyntax { Parent: ParameterSyntax },
					RawKind: (int)SyntaxKind.DefaultLiteralExpression
				} node
				when semanticModel.GetOperation(node) is IDefaultValueOperation
				{
					Kind: OperationKind.DefaultValue,
					Type: var typeSymbol
				}:
				{
					bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CellsFullTypeName)
					);
					bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					);
					isOfTypeSudokuGrid = SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
					);

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					typeName = isOfTypeCells
						? CellsTypeName
						: isOfTypeCandidates ? CandidatesTypeName : SudokuGridTypeName;

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
						id: DiagnosticIds.SD0303,
						title: Titles.SD0303,
						messageFormat: Messages.SD0303,
						category: Categories.Performance,
						defaultSeverity: DiagnosticSeverity.Warning,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.SD0303
					),
					location: originalNode.GetLocation(),
					messageArgs: new[]
					{
						typeName,
						isOfTypeSudokuGrid ? SudokuGridEmptyPropertyName : EmptyPropertyName
					}
				)
			);
		}
	}
}
