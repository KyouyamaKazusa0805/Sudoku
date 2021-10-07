namespace Sudoku.Diagnostics.CodeAnalysis;

[Generator]
[CodeAnalyzer("SD0305", "SD0306F", "SD0307", "SD0308F")]
public sealed partial class CellsOrCandidatesInitializerAnalyzer : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var diagnostic in ((CodeAnalyzer)context.SyntaxContextReceiver!).DiagnosticList)
		{
			context.ReportDiagnostic(diagnostic);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new CodeAnalyzer(context.CancellationToken));


	/// <summary>
	/// Defines the syntax receiver.
	/// </summary>
	private sealed class CodeAnalyzer : IAnalyzer
	{
		/// <summary>
		/// Initializes a <see cref="CodeAnalyzer"/> instance via the specified cancellation token.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
		public CodeAnalyzer(CancellationToken cancellationToken) => CancellationToken = cancellationToken;


		/// <inheritdoc/>
		public CancellationToken CancellationToken { get; }

		/// <inheritdoc/>
		public IList<Diagnostic> DiagnosticList { get; } = new List<Diagnostic>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: BaseObjectCreationExpressionSyntax
					{
						ArgumentList: var argumentList,
						Initializer.Expressions: var expressions
					} baseObjectCreationExpression,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			var possibleOperation = semanticModel.GetOperation(baseObjectCreationExpression, CancellationToken);
			if (
				possibleOperation is not IObjectCreationOperation
				{
					Kind: OperationKind.ObjectCreation,
					Type: var typeSymbol
				}
			)
			{
				return;
			}

			var cellsSymbol = compilation.GetTypeByMetadataName("Sudoku.Data.Cells");
			var candidatesSymbol = compilation.GetTypeByMetadataName("Sudoku.Data.Candidates");
			bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(typeSymbol, cellsSymbol);
			bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(typeSymbol, candidatesSymbol);
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
					semanticModel.GetOperation(expression, CancellationToken) is
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
						DiagnosticList.Add(
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
						DiagnosticList.Add(
							(realValue is < 0 or >= 81) switch
							{
								true => Diagnostic.Create(
									descriptor: SD0305,
									location: expr.GetLocation(),
									messageArgs: null
								),
								false => Diagnostic.Create(
									descriptor: SD0307,
									location: expr.GetLocation(),
#if SUPPORT_CODE_FIXER
									properties: ImmutableDictionary.CreateRange(
										new KeyValuePair<string, string?>[]
										{
											new("RealValue", realValue.ToString())
										}
									),
#endif
									messageArgs: new[] { realValue.ToString() }
								)
							}
						);

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
						DiagnosticList.Add(
							(realValue <= 0 || realValue >= limit + 1) switch
							{
								true => Diagnostic.Create(
									descriptor: SD0305,
									location: expr.GetLocation(),
									messageArgs: null
								),
								false => Diagnostic.Create(
									descriptor: SD0307,
									location: expr.GetLocation(),
#if SUPPORT_CODE_FIXER
									properties: ImmutableDictionary.CreateRange(
										new KeyValuePair<string, string?>[]
										{
											new("RealValue", (realValue - 1).ToString())
										}
									),
#endif
									messageArgs: new[] { $"~{realValue - 1}" }
								)
							}
						);

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
							DiagnosticList.Add(
								Diagnostic.Create(
									descriptor: SD0305,
									location: expr.GetLocation(),
									messageArgs: null
								)
							);
						}
						else if (argumentList is not { Arguments.Count: not 0 })
						{
							DiagnosticList.Add(
								Diagnostic.Create(
									descriptor: SD0306,
									location: expr.GetLocation(),
									messageArgs: new[]
									{
										isOfTypeCells ? "Sudoku.Data.Cells" : "Sudoku.Data.Candidates"
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
						DiagnosticList.Add(
							Diagnostic.Create(
								descriptor: SD0308,
								location: comparisonNode.GetLocation(),
#if SUPPORT_CODE_FIXER
								additionalLocations: new[] { comparisonNode.GetLocation() },
#endif
								messageArgs: new[] { v1 >= 0 ? v1.ToString() : $"~{-v1 - 1}" }
							)
						);
					}
				}
			}
		}
	}
}
