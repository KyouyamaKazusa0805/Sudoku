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

		CheckPlainDefaultExpression(node, semanticModel, gridSymbol);
		CheckDefaultExpressionUsages(node, semanticModel, gridSymbol);
		CheckEnumerator(node, semanticModel, gridSymbol);
		CheckNullAsFirstParseArgument(node, semanticModel, compilation, gridSymbol);
	}

	private void CheckPlainDefaultExpression(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol gridSymbol)
	{
		var nodeOperation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			nodeOperation is not IDefaultValueOperation
			{
				Type: var typeSymbol,
				//Parent: not IParameterInitializerOperation
			}
		)
		{
			return;
		}

		if (node.Parent is EqualsValueClauseSyntax { Parent: ParameterSyntax })
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(typeSymbol, gridSymbol))
		{
			return;
		}

		if (ContainingTypeIsGrid(node, gridSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0501, node.GetLocation(), messageArgs: null));
	}

	private void CheckDefaultExpressionUsages(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol gridSymbol)
	{
		var nodeOperation = semanticModel.GetOperation(node, _cancellationToken);
		switch (nodeOperation)
		{
			// obj == default
			// obj != default
			// obj == default(Grid)
			// obj != default(Grid)
			// default == obj
			// default != obj
			// default(Grid) == obj
			// default(Grid) != obj
			case IBinaryOperation
			{
				LeftOperand: { Type: var leftSymbol } leftOperand,
				RightOperand: { Type: var rightSymbol } rightOperand,
				OperatorKind: BinaryOperatorKind.Equals or BinaryOperatorKind.NotEquals
			}:
			{
				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, leftSymbol))
				{
					return;
				}

				if (!SymbolEqualityComparer.Default.Equals(gridSymbol, rightSymbol))
				{
					return;
				}

				if (
					(Left: leftOperand, Right: rightOperand) is not (
						(Left: IDefaultValueOperation, Right: not IDefaultValueOperation)
						or (Left: not IDefaultValueOperation, Right: IDefaultValueOperation)
					)
				)
				{
					return;
				}

				Diagnostics.Add(Diagnostic.Create(SCA0502, node.GetLocation(), messageArgs: null));

				break;
			}

			// obj.Equals(default(Grid))
			// default(Grid).Equals(obj)
			case IInvocationOperation
			{
				TargetMethod: { Name: "Equals", IsStatic: false },
				Instance: var instanceOperation,
				Type: var typeSymbol,
				Arguments: { Length: 1 } arguments
			}:
			{
				if (!SymbolEqualityComparer.Default.Equals(typeSymbol, gridSymbol))
				{
					return;
				}

				var argumentOperation = arguments[0];

				if (
					(Left: instanceOperation!, Right: argumentOperation) switch
					{
						(
							Left: IDefaultValueOperation { Type: var leftOperationType },
							Right: { Type: var rightOperationType } r
						)
						when f(leftOperationType, rightOperationType) => r is not IDefaultValueOperation,

						(
							Left: { Type: var leftOperationType } l,
							Right: IDefaultValueOperation { Type: var rightOperationType }
						)
						when f(leftOperationType, rightOperationType) => l is not IDefaultValueOperation,

						_ => default(bool?)
					} is not { } status
				)
				{
					return;
				}

				Diagnostics.Add(
					Diagnostic.Create(
						status switch { true => SCA0502, false => SCA0504 },
						node.GetLocation(),
						messageArgs: null
					)
				);

				break;


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				bool f(ITypeSymbol? a, ITypeSymbol? b) =>
					SymbolEqualityComparer.Default.Equals(a, gridSymbol)
					&& SymbolEqualityComparer.Default.Equals(b, gridSymbol);
			}

			// Grid.Equals(obj, default)
			// Grid.Equals(obj, default(Grid))
			// Grid.Equals(default, obj)
			// Grid.Equals(default(Grid), obj)
			case IInvocationOperation
			{
				TargetMethod: { Name: "Equals", IsStatic: true },
				Instance: null,
				Type: var typeSymbol,
				Arguments: { Length: 2 } arguments
			}:
			{
				if (!SymbolEqualityComparer.Default.Equals(typeSymbol, gridSymbol))
				{
					return;
				}

				var argument1Operation = arguments[0];
				var argument2Operation = arguments[1];
				if (
					(Left: argument1Operation, Right: argument2Operation) switch
					{
						(
							Left: IDefaultValueOperation { Type: var leftOperationType },
							Right: { Type: var rightOperationType } r
						)
						when f(leftOperationType, rightOperationType) => r is not IDefaultValueOperation,

						(
							Left: { Type: var leftOperationType } l,
							Right: IDefaultValueOperation { Type: var rightOperationType }
						)
						when f(leftOperationType, rightOperationType) => l is not IDefaultValueOperation,

						_ => default(bool?)
					} is not { } status
				)
				{
					return;
				}

				Diagnostics.Add(
					Diagnostic.Create(
						status switch { true => SCA0502, false => SCA0504 },
						node.GetLocation(),
						messageArgs: null
					)
				);

				break;


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				bool f(ITypeSymbol? a, ITypeSymbol? b) =>
					SymbolEqualityComparer.Default.Equals(a, gridSymbol)
					&& SymbolEqualityComparer.Default.Equals(b, gridSymbol);
			}
		}
	}

	private void CheckEnumerator(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol gridSymbol)
	{
		var nodeOperation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			nodeOperation is not IInvocationOperation
			{
				TargetMethod.Name: "EnumerateCandidates",
				Arguments.IsEmpty: true,
				Type: var typeSymbol
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(gridSymbol, typeSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0503, node.GetLocation(), messageArgs: null));
	}

	private void CheckNullAsFirstParseArgument(
		SyntaxNode node,
		SemanticModel semanticModel,
		Compilation compilation,
		INamedTypeSymbol gridSymbol
	)
	{
		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IInvocationOperation
			{
				TargetMethod.Name: "Parse" or "TryParse",
				Type: var operationTypeSymbol,
				Arguments: { Length: >= 1 } arguments
			}
		)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(operationTypeSymbol, gridSymbol))
		{
			return;
		}

		var argumentTypeSymbol = arguments[0].Type;
		var pCharSymbol = compilation.CreatePointerTypeSymbol(compilation.GetSpecialType(SpecialType.System_Char));
		if (argumentTypeSymbol?.SpecialType != SpecialType.System_String
			&& !SymbolEqualityComparer.Default.Equals(argumentTypeSymbol, pCharSymbol))
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0505, node.GetLocation(), messageArgs: null));
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
