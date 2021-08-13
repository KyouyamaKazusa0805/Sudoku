namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0305", "SD0306F", "SD0307", "SD0308F")]
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
		var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

		switch (originalNode)
		{
			case BaseObjectCreationExpressionSyntax
			{
				ArgumentList: var argumentList,
				Initializer.Expressions: var expressions
			} node
			when semanticModel.GetOperation(node, cancellationToken) is IObjectCreationOperation
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
						semanticModel.GetOperation(expression, cancellationToken) is
						{
							ConstantValue: { HasValue: true, Value: int value }
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
								Token.ValueText: var v
							}
						} expr
						when int.TryParse(v, out int realValue):
						{
							if (realValue is < 0 or >= 81)
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
										properties: ImmutableDictionary.CreateRange(
											new KeyValuePair<string, string?>[]
											{
												new("RealValue", realValue.ToString())
											}
										),
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
								Token.ValueText: var v
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
										properties: ImmutableDictionary.CreateRange(
											new KeyValuePair<string, string?>[]
											{
												new("RealValue", (realValue - 1).ToString())
											}
										),
										messageArgs: new[] { $"~{(realValue - 1).ToString()}" }
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
								Token.ValueText: var v
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
							else if (argumentList is not { Arguments.Count: not 0 })
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
				i = 0;
				for (int iterationCount = count - 1; i < iterationCount; i++)
				{
					var (v1, _) = values[i];

					for (int j = i + 1; j < count; j++)
					{
						var (v2, comparisonNode) = values[j];
						if (v1 == v2 && v1 != -1)
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SD0308,
									location: comparisonNode.GetLocation(),
									additionalLocations: new[] { comparisonNode.GetLocation() },
									messageArgs: new[] { v1 >= 0 ? v1.ToString() : $"~{(-v1 - 1).ToString()}" }
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
