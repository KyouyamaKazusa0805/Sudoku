namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents a syntax rewriter that will:
/// <list type="number">
/// <item>
/// Remove declarations declared between comments
/// '<c>VARIABLE_DECLARATION_BEGIN</c>' and '<c>VARIABLE_DECLARATION_END</c>'
/// </item>
/// <item>Append extra <see langword="this"/> parameter <c>@this</c> if it is an instance method</item>
/// <item>Replace <see langword="this"/> references to parameter <c>@this</c> references</item>
/// </list>
/// </summary>
/// <param name="semanticModel">Indicates the bound semantic model of this syntax tree.</param>
/// <param name="cancellationToken">Indicates the cancellation token that can cancel the current operation.</param>
internal sealed class CachedMethodSyntaxRewriter(
	SemanticModel semanticModel,
	CancellationToken cancellationToken
) : CSharpSyntaxRewriter
{
	/// <summary>
	/// Changing code by adding a new parameter <c>this TypeName @this</c> to the parameter list
	/// if the referenced method is an instance method.
	/// </summary>
	/// <param name="node">The original node.</param>
	/// <returns>The replaced new node.</returns>
	public override SyntaxNode? VisitParameterList(ParameterListSyntax node)
	{
		if (node is not
			{
				Parameters: var originalParameters,
				Parent: MethodDeclarationSyntax
				{
					Modifiers: var modifiers,
					Parent: TypeDeclarationSyntax { Identifier.ValueText: var containgTypeName }
				}
			})
		{
			return null;
		}

		var thisParameter = SyntaxFactory.Parameter(
			SyntaxFactory.List<AttributeListSyntax>(),
			SyntaxFactory.TokenList(
				SyntaxFactory.Token(SyntaxKind.ThisKeyword)
			),
			SyntaxFactory.ParseTypeName(containgTypeName),
			SyntaxFactory.Identifier("@this"),
			null
		);
		if (!modifiers.Any(SyntaxKind.StaticKeyword))
		{
			node = node.Update(
				SyntaxFactory.Token(SyntaxKind.OpenParenToken),
				[thisParameter, .. originalParameters],
				SyntaxFactory.Token(SyntaxKind.CloseParenToken)
			);
		}
		return node;
	}

	/// <summary>
	/// Changing code by updating expression <c>MemberName</c> to <c>@this.MemberName</c>
	/// if <c>MemberName</c> is an instance member.
	/// </summary>
	/// <param name="node">The original node.</param>
	/// <returns>The replaced new node.</returns>
	public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
	{
		if (node is not { SyntaxTree: var syntaxTree, Identifier.ValueText: var memberName })
		{
			return null;
		}

		if (semanticModel.GetOperation(node, cancellationToken) is not IMemberReferenceOperation
			{
				Member:
				{
					ContainingType: var memberContainingType,
					IsStatic: false
				}
			})
		{
			return null;
		}

		var containingMethodDeclaration = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
		if (containingMethodDeclaration is null)
		{
			return null;
		}

		var methodSymbol = semanticModel.GetDeclaredSymbol(containingMethodDeclaration, cancellationToken);
		if (methodSymbol is not { ContainingType: var methodContainingType })
		{
			return null;
		}

		if (!SymbolEqualityComparer.Default.Equals(memberContainingType, methodContainingType))
		{
			return null;
		}

		var newNode = SyntaxFactory.MemberAccessExpression(
			default,
			SyntaxFactory.IdentifierName("@this"),
			SyntaxFactory.Token(SyntaxKind.DotToken),
			SyntaxFactory.IdentifierName(memberName)
		);
		return syntaxTree.GetRoot(cancellationToken).ReplaceNode(node, newNode);
	}

	/// <summary>
	/// Changing code by updating expression <c>this.MemberName</c> to <c>@this.MemberName</c>,
	/// or <c>base.MemberName</c> to <c>((BaseType)@this).MemberName</c>.
	/// </summary>
	/// <param name="node">The original node.</param>
	/// <returns>The replaced new node.</returns>
	public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
	{
		if (node is not
			{
				SyntaxTree: var syntaxTree,
				Expression:
				{
					RawKind: var rawKind and (
						(int)SyntaxKind.ThisExpression
							or (int)SyntaxKind.BaseExpression
					)
				} expression,
				Name.Identifier.ValueText: var memberName
			})
		{
			return null;
		}

		var thisOrBaseKeywordOperation = semanticModel.GetOperation(expression, cancellationToken);
		if (thisOrBaseKeywordOperation is not IInstanceReferenceOperation { Type: { } currentContainingType })
		{
			return null;
		}

		var factContainingType = currentContainingType;
		var isBaseKeyword = rawKind != (int)SyntaxKind.ThisExpression;
		if (isBaseKeyword)
		{
			// Try to fetch nearest base type that overrides this member.
			for (var type = currentContainingType?.BaseType; type is not null; type = type.BaseType)
			{
				if (type.GetMembers().Any(memberSymbol => memberSymbol.Name == memberName && memberSymbol.IsOverride))
				{
					factContainingType = type;
					break;
				}
			}
		}

		var newNode = SyntaxFactory.MemberAccessExpression(
			default,
			isBaseKeyword
				? SyntaxFactory.ParenthesizedExpression(
					SyntaxFactory.Token(SyntaxKind.OpenParenToken),
					SyntaxFactory.CastExpression(
						SyntaxFactory.Token(SyntaxKind.OpenParenToken),
						SyntaxFactory.ParseTypeName(factContainingType.Name),
						SyntaxFactory.Token(SyntaxKind.CloseParenToken),
						SyntaxFactory.IdentifierName("@this")
					),
					SyntaxFactory.Token(SyntaxKind.CloseParenToken)
				)
				: SyntaxFactory.IdentifierName("@this"),
			SyntaxFactory.Token(SyntaxKind.DotToken),
			SyntaxFactory.IdentifierName(memberName)
		);
		return syntaxTree.GetRoot(cancellationToken).ReplaceNode(node, newNode);
	}

	/// <summary>
	/// Changing code by updating expression <c>MemberName(parameters)</c> to <c>@this.MemberName(parameters)</c>.
	/// </summary>
	/// <param name="node">The original node.</param>
	/// <returns>The replaced new node.</returns>
	public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
	{
		return null;
	}

	/// <summary>
	/// Changing code by updating expression <c>this[parameters]</c> to <c>@this[parameters]</c>,
	/// or <c>base[parameters]</c> to <c>((BaseType)@this)[parameters]</c>.
	/// </summary>
	/// <param name="node">The original node.</param>
	/// <returns>The replaced new node.</returns>
	public override SyntaxNode? VisitElementAccessExpression(ElementAccessExpressionSyntax node)
	{
		return null;
	}
}
