using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using ExplicitNew = Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax;
using ImplicitNew = Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax;
using New = Microsoft.CodeAnalysis.CSharp.Syntax.BaseObjectCreationExpressionSyntax;
using Pair = System.ValueTuple<string, Microsoft.CodeAnalysis.SyntaxNode>;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class CellsOrCandidatesAnalyzer
	{
		partial void CheckSudoku021(
			GeneratorExecutionContext context, SyntaxNode root,
			Compilation compilation, SemanticModel semanticModel)
		{
			var collector = new InnerWalker_NewClause(semanticModel, compilation);
			collector.Visit(root);

			// If the syntax tree doesn't contain any dynamically called clause,
			// just skip it.
			if (collector.Collection is null)
			{
				return;
			}

			// Iterate on each dynamically called location.
			foreach (var (typeName, node) in collector.Collection)
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
						messageArgs: new[] { typeName, "Empty" }
					)
				);
			}
		}


		/// <summary>
		/// Bound by <see cref="CheckSudoku021"/>.
		/// </summary>
		/// <seealso cref="CheckSudoku021"/>
		private sealed class InnerWalker_NewClause : CSharpSyntaxWalker
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
			/// Indicates the full type name of the cells.
			/// </summary>
			private const string CellsFullTypeName = "Sudoku.Data.Cells";

			/// <summary>
			/// Indicates the full type name of the candidates.
			/// </summary>
			private const string CandidatesFullTypeName = "Sudoku.Data.Candidates";


			/// <summary>
			/// Indicates the semantic model.
			/// </summary>
			private readonly SemanticModel _semanticModel;

			/// <summary>
			/// Indicates the compilation.
			/// </summary>
			private readonly Compilation _compilation;


			/// <summary>
			/// Initializes an instance with the specified semantic model and the compilation.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			/// <param name="compilation">The compilation.</param>
			public InnerWalker_NewClause(SemanticModel semanticModel, Compilation compilation)
			{
				_semanticModel = semanticModel;
				_compilation = compilation;
			}


			/// <summary>
			/// Indicates the collection that stores all possible and valid information.
			/// </summary>
			public IList<Pair>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitImplicitObjectCreationExpression(ImplicitNew node) => VisitNewClause(node);

			/// <inheritdoc/>
			public override void VisitObjectCreationExpression(ExplicitNew node) => VisitNewClause(node);

			/// <inheritdoc/>
			public override void VisitDefaultExpression(DefaultExpressionSyntax node)
			{
				if (
					_semanticModel.GetOperation(node) is not IDefaultValueOperation
					{
						Kind: OperationKind.DefaultValue,
						Type: var typeSymbol
					}
				)
				{
					return;
				}

				bool isTypeCells = SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					_compilation.GetTypeByMetadataName(CellsFullTypeName)
				);
				if (
					!isTypeCells
					&& !SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<Pair>();

				Collection.Add((isTypeCells ? CellsTypeName : CandidatesTypeName, node));
			}

			/// <inheritdoc/>
			public override void VisitLiteralExpression(LiteralExpressionSyntax node)
			{
				if (node is not { RawKind: (int)SyntaxKind.DefaultLiteralExpression })
				{
					return;
				}

				if (
					_semanticModel.GetOperation(node) is not IDefaultValueOperation
					{
						Kind: OperationKind.DefaultValue,
						Type: var typeSymbol
					}
				)
				{
					return;
				}

				bool isTypeCells = SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					_compilation.GetTypeByMetadataName(CellsFullTypeName)
				);
				if (
					!isTypeCells
					&& !SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<Pair>();

				Collection.Add((isTypeCells ? CellsTypeName : CandidatesTypeName, node));
			}

			/// <summary>
			/// Bound by <see cref="VisitImplicitObjectCreationExpression"/>
			/// and <see cref="VisitObjectCreationExpression"/>.
			/// </summary>
			private void VisitNewClause(New node)
			{
				if (
					node is not
					{
						ArgumentList: { Arguments: { Count: 0 } },
						Initializer: null
					}
				)
				{
					return;
				}

				if (
					_semanticModel.GetOperation(node) is not IObjectCreationOperation
					{
						Kind: OperationKind.ObjectCreation,
						Type: var typeSymbol
					}
				)
				{
					return;
				}

				bool isTypeCells = SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					_compilation.GetTypeByMetadataName(CellsFullTypeName)
				);
				if (
					!isTypeCells
					&& !SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<Pair>();

				Collection.Add((isTypeCells ? CellsTypeName : CandidatesTypeName, node));
			}
		}
	}
}
