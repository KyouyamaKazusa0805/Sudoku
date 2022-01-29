namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0303", "SCA0304")]
public sealed partial class ReadOnlyMemberSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (context is not { Node: var node, SemanticModel: { Compilation: var compilation } semanticModel })
		{
			return;
		}

		const string attributeFullName = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
		var attribute = compilation.GetTypeByMetadataName(attributeFullName);
		if (attribute is null)
		{
			return;
		}

		switch (node)
		{
			case StructDeclarationSyntax:
			{
				var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
				if (
					symbol is not INamedTypeSymbol
					{
						TypeKind: TypeKind.Struct,
						DeclaringSyntaxReferences: [var symbolSyntaxRef, ..]
					} typeSymbol
				)
				{
					return;
				}

				foreach (var member in typeSymbol.GetMembers())
				{
					switch (member)
					{
						case IPropertySymbol
						{
							GetMethod: { DeclaringSyntaxReferences: [var syntaxRef, ..] } getter,
							IsStatic: false
						}:
						{
							var attributesData = getter.GetAttributes();
							if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
							{
								continue;
							}

							var syntaxNode = syntaxRef.GetSyntax(_cancellationToken);
							if (syntaxNode is not AccessorDeclarationSyntax { Keyword: var keywordToken })
							{
								continue;
							}

							Diagnostics.Add(Diagnostic.Create(SCA0303, keywordToken.GetLocation(), messageArgs: null));

							break;
						}
						case IEventSymbol { IsStatic: false }:
						{
							var attributesData = symbol.GetAttributes();
							if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
							{
								continue;
							}

							var syntaxNode = symbolSyntaxRef.GetSyntax(_cancellationToken);
							if (syntaxNode is not EventDeclarationSyntax { Identifier: var identifier })
							{
								continue;
							}

							Diagnostics.Add(Diagnostic.Create(SCA0303, identifier.GetLocation(), messageArgs: null));

							break;
						}
						case IMethodSymbol { IsStatic: false }:
						{
							var attributesData = symbol.GetAttributes();
							if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
							{
								continue;
							}

							var syntaxNode = symbolSyntaxRef.GetSyntax(_cancellationToken);
							if (syntaxNode is not MethodDeclarationSyntax { Identifier: var identifier })
							{
								continue;
							}

							Diagnostics.Add(Diagnostic.Create(SCA0303, identifier.GetLocation(), messageArgs: null));

							break;
						}
					}
				}

				break;
			}

			case PropertyDeclarationSyntax
			{
				ExpressionBody.Expression: var expr,
				Identifier: var identifier
			}:
			{
				var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
				if (
					symbol is not IPropertySymbol
					{
						IsStatic: false,
						ContainingType.TypeKind: TypeKind.Struct,
						SetMethod: null,
						GetMethod: { } getter
					}
				)
				{
					return;
				}

				var attributesData = getter.GetAttributes();
				if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
				{
					return;
				}

				switch (expr)
				{
					// => this.MethodInvocation();
					case InvocationExpressionSyntax
					{
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: ThisExpressionSyntax
						} methodRef
					}
					when isReadOnlyMethodRef(methodRef):
					{
						r();

						break;
					}

					// => constantValue;
					case LiteralExpressionSyntax:
					{
						r();

						break;
					}

					// => default;
					case DefaultExpressionSyntax:
					{
						r();

						break;
					}

					// => this.DataMember;
					case MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: ThisExpressionSyntax
					} dataMemberRef
					when isReadOnlyDataMemberRef(dataMemberRef):
					{
						r();

						break;
					}

					// => MethodInvocation();
					case InvocationExpressionSyntax { Expression: var methodRef } when isReadOnlyMethodRef(methodRef):
					{
						r();

						break;
					}

					// => DataMember;
					case IdentifierNameSyntax dataMemberRef when isReadOnlyDataMemberRef(dataMemberRef):
					{
						r();

						break;
					}
				}

				break;


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void r() => Diagnostics.Add(
					Diagnostic.Create(
						descriptor: SCA0304,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				bool isReadOnlyMethodRef(SyntaxNode methodRef) =>
					semanticModel.GetOperation(methodRef, _cancellationToken) is IMethodReferenceOperation
					{
						Method.IsReadOnly: true
					};

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				bool isReadOnlyDataMemberRef(SyntaxNode dataMemberRef) =>
					semanticModel.GetOperation(dataMemberRef, _cancellationToken) is IFieldReferenceOperation or IPropertyReferenceOperation
					{
						Property.IsReadOnly: true
					};
			}
		}
	}
}
