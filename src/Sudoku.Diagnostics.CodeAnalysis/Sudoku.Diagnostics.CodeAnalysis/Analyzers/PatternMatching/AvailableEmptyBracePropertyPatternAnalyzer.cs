namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0634")]
public sealed partial class AvailableEmptyBracePropertyPatternAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[] { SyntaxKind.IsPatternExpression, SyntaxKind.LogicalAndExpression }
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode, _, cancellationToken) = context;

		switch (originalNode)
		{
			// o is object variable
			case IsPatternExpressionSyntax
			{
				Expression: var expr,
				Pattern: DeclarationPatternSyntax
				{
					Type: PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.ObjectKeyword },
					Designation: SingleVariableDesignationSyntax { Identifier.ValueText: var variableName }
				}
			}
			when !expressionIsOfPointerTypeOrNull(expr):
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0634,
						location: originalNode.GetLocation(),
						messageArgs: new[] { expr.ToString(), variableName, "{ }" },
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[] { new("VariableName", variableName) }
						),
						additionalLocations: new[] { expr.GetLocation() }
					)
				);

				break;
			}

			// o is not null and var variable
			case IsPatternExpressionSyntax
			{
				Expression: var expr,
				Pattern: BinaryPatternSyntax
				{
					RawKind: (int)SyntaxKind.AndPattern,
					Left: var leftPattern,
					Right: var rightPattern
				}
			}
			when !expressionIsOfPointerTypeOrNull(expr):
			{
				switch ((Left: leftPattern, Right: rightPattern))
				{
					case (
						Left: UnaryPatternSyntax
						{
							Pattern: ConstantPatternSyntax
							{
								Expression.RawKind: (int)SyntaxKind.NullLiteralExpression
							}
						} or RecursivePatternSyntax
						{
							PositionalPatternClause: null,
							PropertyPatternClause.Subpatterns.Count: 0,
							Designation: null
						} or TypePatternSyntax
						{
							Type: PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.ObjectKeyword }
						},
						Right: VarPatternSyntax
						{
							Designation: SingleVariableDesignationSyntax
							{
								Identifier.ValueText: var variableName
							}
						}
					):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0634,
								location: originalNode.GetLocation(),
								messageArgs: new[] { expr.ToString(), variableName, "{ }" },
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("VariableName", variableName) }
								),
								additionalLocations: new[] { expr.GetLocation() }
							)
						);

						break;
					}
					case (
						Left: VarPatternSyntax
						{
							Designation: SingleVariableDesignationSyntax { Identifier.ValueText: var variableName }
						},
						Right: UnaryPatternSyntax
						{
							Pattern: ConstantPatternSyntax
							{
								Expression.RawKind: (int)SyntaxKind.NullLiteralExpression
							}
						} or RecursivePatternSyntax
						{
							PositionalPatternClause: null,
							PropertyPatternClause.Subpatterns.Count: 0,
							Designation: null
						} or TypePatternSyntax
						{
							Type: PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.ObjectKeyword }
						}
					):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0634,
								location: originalNode.GetLocation(),
								messageArgs: new[] { expr.ToString(), variableName, "{ }" },
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("VariableName", variableName) }
								),
								additionalLocations: new[] { expr.GetLocation() }
							)
						);

						break;
					}
				}

				break;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool expressionIsOfPointerTypeOrNull(ExpressionSyntax expr) =>
				semanticModel.GetOperation(expr, cancellationToken) is
				{
					// I Don't know why pointer types may let the symbol null... Is there a bug?
					//     |
					//     ↓
					Type: null or IPointerTypeSymbol or IFunctionPointerTypeSymbol
				};
		}
	}
}
