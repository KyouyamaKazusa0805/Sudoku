namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0402")]
public sealed partial class GetPinnableReferenceMethodSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		// Deconstruct the instance.
		if (
			context is not
			{
				Node: MethodDeclarationSyntax
				{
					Identifier: { ValueText: "GetPinnableReference" } identifier
				} node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (symbol is { ContainingType.TypeKind: TypeKind.Interface })
		{
			// We don't check any interface members on this case.
			return;
		}

		if (symbol is not { IsStatic: false, IsAbstract: false, ReturnType: var returnType })
		{
			return;
		}

		var voidSymbol = compilation.GetSpecialType(SpecialType.System_Void);
		if (SymbolEqualityComparer.Default.Equals(returnType, voidSymbol))
		{
			return;
		}

		// Checks whether the [EditorBrowsable(EditorBrowsableState.Never)] exists.
		// Please note that 'object' typed value can't use 'is constant' pattern to check,
		// because 'o is constant' is equivalent to 'o is type && (type)o == constant',
		// where 'o is type' always returns false (because here 'o' is an 'object' instead of 'type').
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(EditorBrowsableAttribute).FullName);
		if (
			symbol.GetAttributes().Any(
				attributeData =>
					attributeData is
					{
						AttributeClass: var attribute,
						ConstructorArguments: { Length: 1 } constructorArguments
					}
					&& SymbolEqualityComparer.Default.Equals(attributeSymbol, attribute)
					&& (EditorBrowsableState)constructorArguments[0].Value! == EditorBrowsableState.Never
			)
		)
		{
			return;
		}

		// Add it into the collection.
		Diagnostics.Add(Diagnostic.Create(SCA0402, identifier.GetLocation(), messageArgs: null));
	}
}
