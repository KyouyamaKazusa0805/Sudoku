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
				case BaseObjectCreationExpressionSyntax
				{
					ArgumentList: var argumentList,
					Initializer: { Expressions: var expressions }
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
					if (!isOfTypeCells && !isOfTypeCandidates)
					{
						return;
					}

					int i = 0;
					int count = expressions.Count;
					var values = new (int ConstantValue, SyntaxNode Node)[count];
					int limit = isOfTypeCells ? 81 : 729;
					foreach (var expression in expressions)
					{
						values[i++] = (
							semanticModel.GetOperation(expression)?.ConstantValue is
							{
								HasValue: true,
								Value: int value
							} ? value : -1,
							expression
						);

						switch (expression)
						{
							case LiteralExpressionSyntax
							{
								RawKind: (int)SyntaxKind.NumericLiteralExpression,
								Token: { ValueText: var v } token
							}
							when int.TryParse(v, out int realValue) && realValue >= limit:
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
								RawKind: (int)SyntaxKind.UnaryPlusExpression,
								Operand: LiteralExpressionSyntax
								{
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token: { ValueText: var v }
								}
							} expr
							when int.TryParse(v, out int realValue):
							{
								if (realValue < 0 || realValue >= 81)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0305,
											location: expr.GetLocation(),
											messageArgs: null
										)
									);
								}
								else
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0307,
											location: expr.GetLocation(),
											messageArgs: new[] { realValue.ToString() }
										)
									);
								}

								break;
							}
							case PrefixUnaryExpressionSyntax
							{
								RawKind: (int)SyntaxKind.UnaryMinusExpression,
								Operand: LiteralExpressionSyntax
								{
									RawKind: (int)SyntaxKind.NumericLiteralExpression,
									Token: { ValueText: var v }
								}
							} expr
							when int.TryParse(v, out int realValue):
							{
								// -1 is ~0, -2 is ~1, -3 is ~2, ..., -81 is ~80.
								// From the sequence we can learn that the maximum valid value
								// in this unary minus expression is 81.
								if (realValue <= 0 || realValue >= limit + 1)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0305,
											location: expr.GetLocation(),
											messageArgs: null
										)
									);
								}
								else
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0307,
											location: expr.GetLocation(),
											messageArgs: new[] { $"~{realValue - 1}" }
										)
									);
								}

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
							when int.TryParse(v, out int realValue):
							{
								if (realValue >= limit)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0305,
											location: expr.GetLocation(),
											messageArgs: null
										)
									);
								}
								else if (argumentList is not { Arguments: { Count: not 0 } })
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0306,
											location: expr.GetLocation(),
											messageArgs: new[]
											{
												isOfTypeCells ? CellsTypeName : CandidatesTypeName
											}
										)
									);
								}

								break;
							}
						}
					}

					// Check whether the initialize contains the same value.
					for (i = 0; i < count - 1; i++)
					{
						for (int j = i + 1; j < count; j++)
						{
							var (v1, currentNode) = values[i];
							var (v2, _) = values[j];
							if (v1 == v2)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0308,
										location: currentNode.GetLocation(),
										messageArgs: new[] { v1 >= 0 ? v1.ToString() : $"~{-v1 - 1}" }
									)
								);
							}
						}
					}

					break;
				}
			}
		}
	}
}
