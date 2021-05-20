using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>,
	/// to check whether the input value in the initializer is invalid.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class CellsAndCandidatesInitializerAnalyzer : DiagnosticAnalyzer
	{
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
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			switch (originalNode)
			{
				case BaseObjectCreationExpressionSyntax { Initializer: { Expressions: var expressions } } node
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
					if (!isOfTypeCells && !isOfTypeCandidates)
					{
						return;
					}

					foreach (var expression in expressions)
					{
						switch (expression)
						{
							case LiteralExpressionSyntax
							{
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token: { ValueText: var v } token
							}
							when int.Parse(v) >= (isOfTypeCells ? 81 : 729):
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0305,
										location: token.GetLocation(),
										messageArgs: null
									)
								);

								break;
							}
							case PrefixUnaryExpressionSyntax
							{
								RawKind: (int)SyntaxKind.BitwiseNotExpression,
								Operand: LiteralExpressionSyntax
								{
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token: { ValueText: var v }
								}
							} expr
							when int.Parse(v) >= (isOfTypeCells ? 81 : 729):
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0305,
										location: expr.GetLocation(),
										messageArgs: null
									)
								);

								break;
							}
						}
					}

					break;
				}
			}
		}
	}
}
