namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0110", "SCA0111", "SCA0112", "SCA0113", "SCA0114", "SCA0115", "SCA0116")]
public sealed partial class AnonymousInnerTypeSyntaxChecker : ISyntaxContextReceiver
{
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

		var attribute = compilation.GetTypeByMetadataName(typeof(AnonymousInnerTypeAttribute).FullName)!;
		CheckLocalVariables(node, semanticModel, attribute);
		CheckPropetiesOnSymbol(node, semanticModel, attribute);
		CheckUsages(node, semanticModel, attribute);
	}

	private void CheckLocalVariables(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol attribute)
	{
		var nodeOperation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			nodeOperation is not IMethodBodyOperation
			{
				BlockBody: var blockBody,
				ExpressionBody: var expressionBody
			}
		)
		{
			return;
		}

		switch ((Block: blockBody, Expression: expressionBody))
		{
			case (Block: { Locals: { Length: not 0 } locals }, Expression: null):
			{
				f(locals);

				break;
			}

			case (Block: null, Expression: { Locals: { Length: not 0 } locals }):
			{
				f(locals);

				break;
			}


			void f(ImmutableArray<ILocalSymbol> locals)
			{
				foreach (var local in locals)
				{
					var type = local.Type;
					var attributesData = type.GetAttributes();
					if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
					{
						continue;
					}

					var syntaxReference = local.DeclaringSyntaxReferences[0];
					var location = Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
					Diagnostics.Add(Diagnostic.Create(SCA0110, location, messageArgs: null));
				}
			}
		}
	}

	private void CheckPropetiesOnSymbol(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol attribute)
	{
		if (node is not TypeDeclarationSyntax { Identifier: var identifier })
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (
			symbol is not INamedTypeSymbol
			{
				TypeKind: var typeKind,
				DeclaredAccessibility: var accessibilty,
				IsSealed: var isSealed,
				BaseType: var baseType,
				Interfaces: var baseInterfaces
			} typeSymbol
		)
		{
			return;
		}

		var attributesData = symbol.GetAttributes();
		if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
		{
			return;
		}

		if (
			!(
				from member in typeSymbol.GetMembers().OfType<IMethodSymbol>()
				where member is { IsStatic: false, Name: ".ctor" }
				select member
			).Any()
		)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0111, identifier.GetLocation(), messageArgs: null));

			return;
		}

		if (typeKind is not TypeKind.Class or TypeKind.Interface)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0112, identifier.GetLocation(), messageArgs: null));
		}

		if (accessibilty == Accessibility.Public)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0113, identifier.GetLocation(), messageArgs: null));
		}

		if (typeKind == TypeKind.Class && !isSealed)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0114, identifier.GetLocation(), messageArgs: null));
		}

		switch ((BaseType: baseType, BaseInterfaces: baseInterfaces))
		{
			case (BaseType: null, BaseInterfaces: { Length: not 0 }):
			case (BaseType: not null, _):
			{
				return;
			}

			default:
			{
				Diagnostics.Add(Diagnostic.Create(SCA0115, identifier.GetLocation(), messageArgs: null));

				return;
			}
		}
	}

	private void CheckUsages(SyntaxNode node, SemanticModel semanticModel, INamedTypeSymbol attribute)
	{
		switch (node)
		{
			case GenericNameSyntax:
			{
				var symbol = semanticModel.GetTypeInfo(node, _cancellationToken).Type!;
				if (
					symbol is not INamedTypeSymbol
					{
						TypeArguments: { Length: not 0 } typeArguments
					}
				)
				{
					return;
				}

				foreach (var typeArgument in typeArguments)
				{
					var attributesData = typeArgument.GetAttributes();
					if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
					{
						continue;
					}

					Diagnostics.Add(Diagnostic.Create(SCA0116, node.GetLocation(), messageArgs: null));
				}

				break;
			}

			case NullableTypeSyntax { ElementType: var elementType }:
			{
				var symbol = semanticModel.GetTypeInfo(elementType, _cancellationToken).Type!;
				if (
					symbol is not INamedTypeSymbol
					{
						TypeKind: TypeKind.Struct,
						TypeArguments: { Length: not 0 } typeArguments
					}
				)
				{
					return;
				}

				foreach (var typeArgument in typeArguments)
				{
					var attributesData = typeArgument.GetAttributes();
					if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
					{
						continue;
					}

					Diagnostics.Add(Diagnostic.Create(SCA0116, node.GetLocation(), messageArgs: null));
				}

				break;
			}

			case ArrayTypeSyntax { ElementType: var elementType }:
			{
				var symbol = semanticModel.GetTypeInfo(elementType, _cancellationToken).Type!;
				if (
					symbol is not INamedTypeSymbol
					{
						TypeArguments: { Length: not 0 } typeArguments
					}
				)
				{
					return;
				}

				foreach (var typeArgument in typeArguments)
				{
					var attributesData = typeArgument.GetAttributes();
					if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
					{
						continue;
					}

					Diagnostics.Add(Diagnostic.Create(SCA0116, node.GetLocation(), messageArgs: null));
				}

				break;
			}

			case MethodDeclarationSyntax:
			{
				var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
				if (symbol is not IMethodSymbol { ReturnType: var returnType, Parameters: var parameters })
				{
					return;
				}

				var attributesData = returnType.GetAttributes();
				if (attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
				{
					Diagnostics.Add(Diagnostic.Create(SCA0116, node.GetLocation(), messageArgs: null));

					return;
				}

				foreach (var parameter in parameters)
				{
					var parameterType = parameter.Type;
					var parameterAttributesData = parameterType.GetAttributes();
					if (parameterAttributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
					{
						continue;
					}

					Diagnostics.Add(Diagnostic.Create(SCA0116, node.GetLocation(), messageArgs: null));
				}

				break;
			}
		}
	}
}
