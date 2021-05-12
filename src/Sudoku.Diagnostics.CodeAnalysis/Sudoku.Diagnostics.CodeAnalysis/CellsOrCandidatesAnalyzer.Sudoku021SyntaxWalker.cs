using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using ExplicitNew = Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax;
using ImplicitNew = Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax;
using New = Microsoft.CodeAnalysis.CSharp.Syntax.BaseObjectCreationExpressionSyntax;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class CellsOrCandidatesAnalyzer
	{
		/// <summary>
		/// Bound by <see cref="CheckSudoku021"/>.
		/// </summary>
		/// <seealso cref="CheckSudoku021"/>
		private sealed class Sudoku021SyntaxWalker : CSharpSyntaxWalker
		{
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
			public Sudoku021SyntaxWalker(SemanticModel semanticModel, Compilation compilation)
			{
				_semanticModel = semanticModel;
				_compilation = compilation;
			}


			/// <summary>
			/// Indicates the collection that stores all possible and valid information.
			/// </summary>
			public IList<(string, SyntaxNode)>? Collection { get; private set; }


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

				Collection ??= new List<(string, SyntaxNode)>();

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

				Collection ??= new List<(string, SyntaxNode)>();

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

				bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					_compilation.GetTypeByMetadataName(CellsFullTypeName)
				);
				bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(
					typeSymbol,
					_compilation.GetTypeByMetadataName(CandidatesFullTypeName)
				);
				if (!isOfTypeCells && !isOfTypeCandidates)
				{
					return;
				}

				Collection ??= new List<(string, SyntaxNode)>();

				Collection.Add((isOfTypeCells ? CellsTypeName : CandidatesTypeName, node));
			}
		}
	}
}
