namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0501", "SCA0502", "SCA0503", "SCA0504", "SCA0505")]
public sealed partial class GridSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (context is not { Node: var node, SemanticModel: { Compilation: var compilation } semanticModel })
		{
			return;
		}

		var gridSymbol = compilation.GetTypeByMetadataName("Sudoku.Data.Grid");
		if (gridSymbol is null)
		{
			return;
		}

		CheckUsageOnUndefined(node, semanticModel, gridSymbol);
		CheckUsageOnIsUndefined(node, semanticModel, gridSymbol);
		CheckUsageOnEnumerator(node, semanticModel, gridSymbol);
	}

	private void CheckUsageOnUndefined(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol gridSymbol)
	{
		switch (node)
		{
			// default(Grid)
			case DefaultExpressionSyntax
			{
				Type: var typeNode,
				Parent: not EqualsValueClauseSyntax { Parent: ParameterSyntax }
			}:
			{
				var symbol = semanticModel.GetTypeInfo(typeNode, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(symbol, gridSymbol))
				{
					return;
				}

				if (ContainingTypeIsGrid(node, gridSymbol))
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0501, node.GetLocation(), messageArgs: null));

				break;
			}

			// default
			case LiteralExpressionSyntax
			{
				RawKind: (int)SyntaxKind.DefaultLiteralExpression,
				Parent: not EqualsValueClauseSyntax { Parent: ParameterSyntax }
			}:
			{
				var operation = semanticModel.GetOperation(node, _cancellationToken);
				if (operation is not { Type: var symbol })
				{
					return;
				}

				if (!SymbolEqualityComparer.Default.Equals(symbol, gridSymbol))
				{
					return;
				}

				if (ContainingTypeIsGrid(node, gridSymbol))
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0501, node.GetLocation(), messageArgs: null));

				break;
			}
		}
	}

	private void CheckUsageOnIsUndefined(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol gridSymbol)
	{
		switch (node)
		{
			// obj == default(Grid)
			// obj != default(Grid)
			case BinaryExpressionSyntax
			{
				Left: var leftExpr,
				Right: DefaultExpressionSyntax { Type: var rightType },
				RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
			}:
			{
				var leftSymbol = semanticModel.GetTypeInfo(leftExpr, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, leftSymbol))
				{
					return;
				}

				var rightSymbol = semanticModel.GetTypeInfo(rightType, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, rightSymbol))
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0502, node.GetLocation(), messageArgs: null));

				break;
			}

			// default(Grid) == obj
			// default(Grid) != obj
			case BinaryExpressionSyntax
			{
				Left: DefaultExpressionSyntax { Type: var leftType },
				Right: var rightExpr,
				RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
			}:
			{
				var rightSymbol = semanticModel.GetTypeInfo(rightExpr, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, rightSymbol))
				{
					return;
				}

				var leftSymbol = semanticModel.GetTypeInfo(leftType, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, leftSymbol))
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0502, node.GetLocation(), messageArgs: null));

				break;
			}

			// obj == default
			// obj != default
			case BinaryExpressionSyntax
			{
				Left: var leftExpr,
				Right: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.DefaultLiteralExpression } rightExpr,
				RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
			}:
			{
				var operation = semanticModel.GetOperation(rightExpr, _cancellationToken);
				if (operation is not { Type: var rightSymbol })
				{
					return;
				}

				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, rightSymbol))
				{
					return;
				}

				var leftSymbol = semanticModel.GetTypeInfo(leftExpr, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, leftSymbol))
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0502, node.GetLocation(), messageArgs: null));

				break;
			}

			// default == obj
			// default != obj
			case BinaryExpressionSyntax
			{
				Left: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.DefaultLiteralExpression } leftExpr,
				Right: var rightExpr,
				RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
			}:
			{
				var operation = semanticModel.GetOperation(leftExpr, _cancellationToken);
				if (operation is not { Type: var leftSymbol })
				{
					return;
				}

				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, leftSymbol))
				{
					return;
				}

				var rightSymbol = semanticModel.GetTypeInfo(rightExpr, _cancellationToken).Type!;
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, rightSymbol))
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0502, node.GetLocation(), messageArgs: null));

				break;
			}
		}
	}

	private void CheckUsageOnEnumerator(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol gridSymbol)
	{
		if (
			node is not InvocationExpressionSyntax
			{
				Expression: MemberAccessExpressionSyntax
				{
					Expression: var expr,
					Name.Identifier.ValueText: "EnumerateCandidates"
				},
				ArgumentList.Arguments.Count: 0
			}
		)
		{
			return;
		}

		var symbol = semanticModel.GetTypeInfo(expr, _cancellationToken).Type!;
		if (!SymbolEqualityComparer.Default.Equals(gridSymbol, symbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0503, node.GetLocation(), messageArgs: null));
	}


	/// <summary>
	/// Determines whether the specified <see cref="SyntaxNode"/> is located in the source code files
	/// of type <c>Grid</c>.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <param name="gridSymbol">The symbol that corresponding to type <c>Grid</c>.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainingTypeIsGrid(SyntaxNode node, INamedTypeSymbol gridSymbol) =>
		gridSymbol.DeclaringSyntaxReferences.Any(r => r.SyntaxTree.IsEquivalentTo(node.SyntaxTree));
}
