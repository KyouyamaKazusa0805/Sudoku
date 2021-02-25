using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Triplet = System.ValueTuple<string, string, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax>;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class CellsOrCandidatesAnalyzer
	{
		partial void CheckSudoku018(
			GeneratorExecutionContext context, SyntaxNode root,
			Compilation compilation, SemanticModel semanticModel)
		{
			var collector = new InnerWalker_Count(semanticModel, compilation);
			collector.Visit(root);

			// If the syntax tree doesn't contain any dynamically called clause,
			// just skip it.
			if (collector.Collection is not null)
			{
				// Iterate on each location.
				foreach (var (expr, eqToken, node) in collector.Collection)
				{
					// No calling conversion.
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
							messageArgs: new[] { expr, eqToken, eqToken == "==" ? string.Empty : "!" }
						)
					);
				}
			}
		}

		/// <summary>
		/// Bound by <see cref="CheckSudoku018"/>.
		/// </summary>
		/// <seealso cref="CheckSudoku018"/>
		private sealed class InnerWalker_Count : CSharpSyntaxWalker
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
			public InnerWalker_Count(SemanticModel semanticModel, Compilation compilation)
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
				if (
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
				if (
					operation is not
					{
						Kind: OperationKind.LocalReference,
						Type: var typeSymbol
					}
				)
				{
					return;
				}

				if (
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
