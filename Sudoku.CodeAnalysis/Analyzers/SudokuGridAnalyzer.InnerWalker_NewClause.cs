using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using ExplicitNew = Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax;
using ImplicitNew = Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class SudokuGridAnalyzer
	{
		partial void CheckSudoku021(
			GeneratorExecutionContext context, SyntaxNode root,
			SemanticModel semanticModel, Compilation compilation)
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
			foreach (var node in collector.Collection)
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
						messageArgs: new[] { "SudokuGrid", "Undefined" }
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
			/// Indicates the full type name of the sudoku grid.
			/// </summary>
			private const string SudokuGridFullTypeName = "Sudoku.Data.SudokuGrid";


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
			public IList<SyntaxNode>? Collection { get; private set; }


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

				if (
					!SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<SyntaxNode>();

				Collection.Add(node);
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

				if (
					!SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<SyntaxNode>();

				Collection.Add(node);
			}

			/// <summary>
			/// Bound by <see cref="VisitImplicitObjectCreationExpression"/>
			/// and <see cref="VisitObjectCreationExpression"/>.
			/// </summary>
			private void VisitNewClause(BaseObjectCreationExpressionSyntax node)
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

				if (
					!SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<SyntaxNode>();

				Collection.Add(node);
			}
		}
	}
}
