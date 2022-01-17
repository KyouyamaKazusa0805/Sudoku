namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0510", "SCA0511", "SCA0512", "SCA0513", "SCA0514", "SCA0515", "SCA0516", "SCA0517", "SCA0518", "SCA0519", "SCA0520")]
public sealed partial class CellSyntaxChecker : ISyntaxContextReceiver
{
	/// <summary>
	/// Indicates the regular expression pattern to match a cell (coordinate) string.
	/// </summary>
	private static readonly Regex CellPatternRegex = new(
		@"(R[1-9]C[1-9]|r[1-9]c[1-9])",
		RegexOptions.Compiled,
		TimeSpan.FromSeconds(5)
	);


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: var node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		const string cellsSymbolFullName = "Sudoku.Data.Cells";
		var cellsSymbol = compilation.GetTypeByMetadataName(cellsSymbolFullName);
		if (cellsSymbol is null)
		{
			return;
		}

		CheckConstructorArguments(node, semanticModel, cellsSymbol);
		CheckStackAllocExpressionAsArgument(node, semanticModel, cellsSymbol);
		CheckAvailableEmptyPropertyCases(node, semanticModel, cellsSymbol);
		CheckAvailableIsEmptyPropertyCases(node, semanticModel, cellsSymbol);
		CheckAvailableCoveredLinePropertyCases(node, semanticModel, compilation, cellsSymbol);
		CheckIndexArgument(node, semanticModel, cellsSymbol);
		CheckInitializerValues(node, semanticModel, cellsSymbol);
		CheckEqualsMethodUsages(node, semanticModel, cellsSymbol);
		CheckNullAsParseMethodGroupArgument(node, semanticModel, cellsSymbol);
		CheckAvailableOperatorSubtractCases(node, semanticModel, cellsSymbol);
		CheckInitializerBoundWithParameterlessConstructor(node, semanticModel, cellsSymbol);
	}


	private void CheckConstructorArguments(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IObjectCreationOperation
			{
				Constructor:
				{
					ContainingType: var containingTypeSymbol,
					Parameters: { Length: var parameterLength and (2 or 3) } parameters
				},
				Initializer: null,
				Arguments: var arguments
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol))
		{
			return;
		}

		switch (parameterLength)
		{
			// long (40), long (41)
			case 2 when parameters.All(static p => p.Type.SpecialType == SpecialType.System_Int64):
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					var argument = arguments[i];
					if (
						argument is not
						{
							ConstantValue: { HasValue: true, Value: long data },
							Syntax: var argumentSyntaxNode
						}
					)
					{
						continue;
					}

					if (data < (i == 0 ? 0x100_0000_0000 : 0x200_0000_0000))
					{
						continue;
					}

					Diagnostics.Add(
						Diagnostic.Create(
							SCA0510,
							argumentSyntaxNode.GetLocation(),
							messageArgs: new[] { i == 0 ? "high" : "low", i == 0 ? "40" : "41" }
						)
					);
				}

				break;
			}

			// int (27), int (27), int (27)
			case 3 when parameters.All(static p => p.Type.SpecialType == SpecialType.System_Int32):
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					var argument = arguments[i];
					if (
						argument is not
						{
							ConstantValue: { HasValue: true, Value: long data },
							Syntax: var argumentSyntaxNode
						}
					)
					{
						continue;
					}

					if (data < 0x800_0000)
					{
						continue;
					}

					Diagnostics.Add(
						Diagnostic.Create(
							SCA0510,
							argumentSyntaxNode.GetLocation(),
							messageArgs: new[] { i switch { 0 => "high", 1 => "mid", 2 => "low" }, "27" }
						)
					);
				}

				break;
			}
		}
	}

	private void CheckStackAllocExpressionAsArgument(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IObjectCreationOperation
			{
				Constructor.ContainingType: var containingTypeSymbol,
				Initializer: null,
				Arguments: [
				{
					Syntax: ArgumentSyntax
					{
						Expression:
						StackAllocArrayCreationExpressionSyntax
						or ImplicitStackAllocArrayCreationExpressionSyntax
					} argumentNode
				}]
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0511, argumentNode.GetLocation(), messageArgs: null));
	}

	private void CheckAvailableEmptyPropertyCases(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		switch (semanticModel.GetOperation(node, _cancellationToken))
		{
			case IObjectCreationOperation
			{
				Constructor:
				{
					ContainingType: var containingTypeSymbol,
					Parameters.IsEmpty: true
				},
				Initializer: null,
				Syntax: var syntaxNode
			}
			when SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol)
			&& !ContainingTypeIsCells(syntaxNode, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0512, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}

			case ILiteralOperation
			{
				Type: var typeSymbol,
				Syntax: LiteralExpressionSyntax
				{
					RawKind: (int)SyntaxKind.DefaultLiteralExpression
				} syntaxNode
			}
			when SymbolEqualityComparer.Default.Equals(typeSymbol, cellsSymbol)
			&& !ContainingTypeIsCells(syntaxNode, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0512, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}
		}
	}

	private void CheckAvailableIsEmptyPropertyCases(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IBinaryOperation
			{
				OperatorKind: BinaryOperatorKind.Equals or BinaryOperatorKind.NotEquals,
				LeftOperand: var leftOperand,
				RightOperand: var rightOperand,
				Syntax: var syntaxNode
			}
		)
		{
			return;
		}

		switch ((Left: leftOperand, Right: rightOperand))
		{
			case (
				Left: IPropertyReferenceOperation
				{
					Property: { Name: "Count", ContainingType: var containingTypeSymbol }
				},
				Right: { ConstantValue: { HasValue: true, Value: 0 } }
			)
			when SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol)
			&& !ContainingTypeIsCells(syntaxNode, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0513, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}

			case (
				Left: { ConstantValue: { HasValue: true, Value: 0 } },
				Right: IPropertyReferenceOperation
				{
					Property: { Name: "Count", ContainingType: var containingTypeSymbol }
				}
			)
			when SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol)
			&& !ContainingTypeIsCells(syntaxNode, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0513, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}
		}
	}

	private void CheckAvailableCoveredLinePropertyCases(
		SyntaxNode node,
		SemanticModel semanticModel,
		Compilation compilation,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IInvocationOperation
			{
				TargetMethod: { Name: "TrailingZerosCount", ContainingType: var containingTypeSymbol },
				Arguments: [IBinaryOperation
				{
					OperatorKind: BinaryOperatorKind.And,
					LeftOperand: var leftOperand,
					RightOperand: var rightOperand,
					Syntax: var syntaxNode
				}] arguments
			}
		)
		{
			return;
		}

		const string bitOperationsSymbolFullName = "System.Numerics.BitOperations";
		var bitOperationsSymbol = compilation.GetTypeByMetadataName(bitOperationsSymbolFullName);
		if (bitOperationsSymbol is null)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(bitOperationsSymbol, containingTypeSymbol))
		{
			return;
		}

		switch ((Left: leftOperand, Right: rightOperand))
		{
			case (
				Left: IPropertyReferenceOperation
				{
					Property:
					{
						Name: "CoveredRegions",
						ContainingType: var propertyContainingTypeSymbol
					}
				},
				Right: IUnaryOperation
				{
					OperatorKind: UnaryOperatorKind.BitwiseNegation,
					Operand.ConstantValue: { HasValue: true, Value: 511 }
				}
			)
			when SymbolEqualityComparer.Default.Equals(propertyContainingTypeSymbol, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0514, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}

			case (
				Left: IUnaryOperation
				{
					OperatorKind: UnaryOperatorKind.BitwiseNegation,
					Operand.ConstantValue: { HasValue: true, Value: 511 }
				},
				Right: IPropertyReferenceOperation
				{
					Property:
					{
						Name: "CoveredRegions",
						ContainingType: var propertyContainingTypeSymbol
					}
				}
			)
			when SymbolEqualityComparer.Default.Equals(propertyContainingTypeSymbol, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0514, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}
		}
	}

	private void CheckIndexArgument(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IPropertyReferenceOperation
			{
				Property.ContainingType: var containingTypeSymbol,
				Arguments: [
				{
					ConstantValue: { HasValue: true, Value: >= 0 and < 81 },
					Syntax: var argumentSyntaxNode
				}] arguments
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0515, argumentSyntaxNode.GetLocation(), messageArgs: null));
	}

	private void CheckInitializerValues(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IObjectOrCollectionInitializerOperation
			{
				Initializers: [.., _] initializers,
				Type: var typeSymbol
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(typeSymbol, cellsSymbol))
		{
			return;
		}

		foreach (var initializer in initializers)
		{
			if (
				initializer is not
				{
					ConstantValue: { HasValue: true, Value: var value },
					Syntax: var initializerSyntax
				}
			)
			{
				continue;
			}

			switch (value)
			{
				case int and (< 0 or >= 81):
				{
					Diagnostics.Add(Diagnostic.Create(SCA0516, initializerSyntax.GetLocation(), messageArgs: null));

					break;
				}
				case string s when !CellPatternRegex.IsMatch(s):
				{
					Diagnostics.Add(Diagnostic.Create(SCA0516, initializerSyntax.GetLocation(), messageArgs: null));

					break;
				}
			}
		}
	}

	private void CheckEqualsMethodUsages(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IInvocationOperation
			{
				TargetMethod:
				{
					Name: "Equals",
					IsStatic: false,
					ReturnType.SpecialType: SpecialType.System_Boolean
				},
				Instance.Type: var instanceType,
				Arguments: [{ Type: var argumentType } argument],
				Syntax: var syntaxNode
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(instanceType, cellsSymbol)
			|| !SymbolEqualityComparer.Default.Equals(argumentType, cellsSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0517, syntaxNode.GetLocation(), messageArgs: null));
	}

	private void CheckNullAsParseMethodGroupArgument(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IInvocationOperation
			{
				TargetMethod:
				{
					Name: "Parse" or "TryParse",
					ContainingType: var containingTypeSymbol,
					Parameters.Length: not 0,
					IsStatic: true
				},
				Arguments: [
				{
					ConstantValue: { HasValue: true, Value: null },
					Syntax: var argumentSyntaxNode
				}, ..] arguments
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0518, argumentSyntaxNode.GetLocation(), messageArgs: null));
	}

	private void CheckAvailableOperatorSubtractCases(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IBinaryOperation
			{
				OperatorKind: BinaryOperatorKind.And,
				LeftOperand: var leftOperand,
				RightOperand: var rightOperand,
				Syntax: var syntaxNode
			}
		)
		{
			return;
		}

		switch ((Left: leftOperand, Right: rightOperand))
		{
			case (
				Left: { Type: var leftOperandType },
				Right: IUnaryOperation
				{
					OperatorKind: UnaryOperatorKind.BitwiseNegation,
					Operand.Type: var rightOperandType
				}
			)
			when SymbolEqualityComparer.Default.Equals(leftOperandType, cellsSymbol)
			&& SymbolEqualityComparer.Default.Equals(rightOperandType, cellsSymbol)
			&& !ContainingTypeIsCells(syntaxNode, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0519, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}

			case (
				Left: IUnaryOperation
				{
					OperatorKind: UnaryOperatorKind.BitwiseNegation,
					Operand.Type: var leftOperandType
				},
				Right: { Type: var rightOperandType }
			)
			when SymbolEqualityComparer.Default.Equals(rightOperandType, cellsSymbol)
			&& SymbolEqualityComparer.Default.Equals(leftOperandType, cellsSymbol)
			&& !ContainingTypeIsCells(syntaxNode, cellsSymbol):
			{
				Diagnostics.Add(Diagnostic.Create(SCA0519, syntaxNode.GetLocation(), messageArgs: null));

				break;
			}
		}
	}

	private void CheckInitializerBoundWithParameterlessConstructor(
		SyntaxNode node,
		SemanticModel semanticModel,
		INamedTypeSymbol cellsSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IObjectCreationOperation
			{
				Constructor.ContainingType: var containingTypeSymbol,
				Initializer: not null,
				Arguments: [
					not
					{
						Value.Type: IArrayTypeSymbol
						{
							Rank: 1,
							ElementType.SpecialType: SpecialType.System_Int32
						}
					}
				] arguments,
				Syntax: var syntaxNode
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(containingTypeSymbol, cellsSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0520, syntaxNode.GetLocation(), messageArgs: null));
	}


	/// <summary>
	/// Determines whether the specified <see cref="SyntaxNode"/> is located in the source code files
	/// of type <c>Cells</c>.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <param name="cellsSymbol">The symbol that corresponding to type <c>Cells</c>.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainingTypeIsCells(SyntaxNode node, INamedTypeSymbol cellsSymbol) =>
		cellsSymbol.DeclaringSyntaxReferences.Any(r => r.SyntaxTree.IsEquivalentTo(node.SyntaxTree));
}
