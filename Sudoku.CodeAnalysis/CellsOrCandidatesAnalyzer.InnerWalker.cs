using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Triplet = System.ValueTuple<string, string, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>;

namespace Sudoku.CodeAnalysis
{
	public sealed partial class CellsOrCandidatesAnalyzer
	{
		/// <summary>
		/// Indicates the searcher that stores the syntax nodes of the dynamically invocation of
		/// that <see langword="dynamic"/> field <c>Current</c>.
		/// </summary>
		private sealed class InnerWalker : CSharpSyntaxWalker
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
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			/// <param name="compilation">The compilation.</param>
			public InnerWalker(SemanticModel semanticModel, Compilation compilation)
			{
				_semanticModel = semanticModel;
				_compilation = compilation;
			}


			/// <summary>
			/// Indicates the valid collection of the <c>Current</c> dynamically invocation.
			/// </summary>
			public IList<Triplet>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitBinaryExpression(BinaryExpressionSyntax node)
			{
				if
				(
					node is not
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
					}
				)
				{
					return;
				}

				var operation = _semanticModel.GetOperation(exprNode);
				if
				(
					operation is not
					{
						Kind: OperationKind.LocalReference,
						Type: var typeSymbol
					}
				)
				{
					return;
				}

				if
				(
					!SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(CellsFullTypeName)
					)
					&& !SymbolEqualityComparer.Default.Equals(
						typeSymbol,
						_compilation.GetTypeByMetadataName(CandidatesFullTypeName)
					)
				)
				{
					return;
				}

				Collection ??= new List<Triplet>();

				Collection.Add(
					(
						exprNode.ToString(),
						kind == (int)SyntaxKind.NotEqualsExpression ? "!=" : "==",
						node
					)
				);
			}
		}
	}
}
