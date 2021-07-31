using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0303", "SD0304")]
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
		/// Indicates the sudoku grid type name.
		/// </summary>
		private const string GridTypeName = "Grid";

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
		/// Indicates the full type name of the sudoku grid.
		/// </summary>
		private const string GridFullTypeName = "Sudoku.Data.Grid";

		/// <summary>
		/// Indicates the field name to check in the diagnostic result <c>SD0303</c>.
		/// </summary>
		private const string EmptyPropertyName = "Empty";

		/// <summary>
		/// Indicates the field name to check in the diagnostic result <c>SD0303</c>.
		/// </summary>
		private const string SudokuGridEmptyPropertyName = "Undefined";

		/// <summary>
		/// Indicates the field name to check in the diagnostic result <c>SD0304</c>.
		/// </summary>
		private const string IsEmptyPropertyName = "IsEmpty";

		/// <summary>
		/// Indicates the field name to check in the diagnostic result <c>SD0304</c>.
		/// </summary>
		private const string IsUndefinedPropertyName = "IsUndefined";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[]
				{
					SyntaxKind.ObjectCreationExpression,
					SyntaxKind.ImplicitObjectCreationExpression,
					SyntaxKind.DefaultExpression,
					SyntaxKind.DefaultLiteralExpression
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			string typeName;
			bool isOfTypeSudokuGrid;
			SyntaxNode? parent;

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			Func<string, INamedTypeSymbol?> c = compilation.GetTypeByMetadataName;
			switch (originalNode)
			{
				case BaseObjectCreationExpressionSyntax
				{
					Parent: var parentNode,
					ArgumentList: { Arguments: { Count: 0 } },
					Initializer: null
				} node
				when semanticModel.GetOperation(node) is IObjectCreationOperation
				{
					Kind: OperationKind.ObjectCreation,
					Type: var typeSymbol
				}:
				{
					bool isOfTypeCells = f(typeSymbol, c(CellsFullTypeName));
					bool isOfTypeCandidates = f(typeSymbol, c(CandidatesFullTypeName));
					isOfTypeSudokuGrid = f(typeSymbol, c(SudokuGridFullTypeName)) || f(typeSymbol, c(GridFullTypeName));

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					parent = parentNode;
					typeName = isOfTypeCells
						? CellsTypeName
						: isOfTypeCandidates ? CandidatesTypeName : $"{SudokuGridTypeName} or {GridTypeName}";

					break;
				}
				case DefaultExpressionSyntax { Parent: var parentNode } node
				when semanticModel.GetOperation(node) is { Type: var typeSymbol }:
				{
					bool isOfTypeCells = f(typeSymbol, c(CellsFullTypeName));
					bool isOfTypeCandidates = f(typeSymbol, c(CandidatesFullTypeName));
					isOfTypeSudokuGrid = f(typeSymbol, c(SudokuGridFullTypeName)) || f(typeSymbol, c(GridFullTypeName));

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					parent = parentNode;
					typeName = isOfTypeCells
						? CellsTypeName
						: isOfTypeCandidates ? CandidatesTypeName : $"{SudokuGridTypeName} or {GridTypeName}";

					break;
				}
				case LiteralExpressionSyntax
				{
					Parent: var parentNode,
					RawKind: (int)SyntaxKind.DefaultLiteralExpression
				} node
				when semanticModel.GetOperation(node) is { Type: var typeSymbol }:
				{
					bool isOfTypeCells = f(typeSymbol, c(CellsFullTypeName));
					bool isOfTypeCandidates = f(typeSymbol, c(CandidatesFullTypeName));
					isOfTypeSudokuGrid = f(typeSymbol, c(SudokuGridFullTypeName)) || f(typeSymbol, c(GridFullTypeName));

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					parent = parentNode;
					typeName = isOfTypeCells
						? CellsTypeName
						: isOfTypeCandidates ? CandidatesTypeName : $"{SudokuGridTypeName} or {GridTypeName}";

					break;
				}
				default:
				{
					return;
				}
			}

			switch (parent)
			{
				case BinaryExpressionSyntax
				{
					Left: var expressionOrVariable,
					RawKind: var kind and (
						(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					)
				} binaryExpr:
				{
					string propertyName = isOfTypeSudokuGrid ? IsUndefinedPropertyName : IsEmptyPropertyName;
					string @operator = kind == (int)SyntaxKind.EqualsExpression ? string.Empty : "!";
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0304,
							location: binaryExpr.GetLocation(),
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("Variable", expressionOrVariable.ToString()),
									new("PropertyName", propertyName),
									new("Operator", @operator)
								}
							),
							messageArgs: new[] { expressionOrVariable.ToString(), propertyName, @operator }
						)
					);

					break;
				}
				default:
				{
					string propertyName = isOfTypeSudokuGrid ? SudokuGridEmptyPropertyName : EmptyPropertyName;
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0303,
							location: originalNode.GetLocation(),
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("TypeName", typeName),
									new("PropertyName", propertyName)
								}
							),
							messageArgs: new[] { typeName, propertyName }
						)
					);

					break;
				}
			}
		}
	}
}
