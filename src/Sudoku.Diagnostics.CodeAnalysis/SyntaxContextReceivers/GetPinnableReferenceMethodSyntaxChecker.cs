namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0402", "SCA0403", "SCA0404", "SCA0405", "SCA0406")]
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

		// Deconstruct the symbol.
		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken)!;
		if (
			symbol is not
			{
				IsAbstract: var isAbstract,
				IsStatic: var isStatic,
				ContainingType.TypeKind: var typeKind,
				ReturnType: var returnType,
				RefKind: var refKind,
				Parameters.IsEmpty: true
			}
		)
		{
			return;
		}

		// We don't check any interface members on this case.
		if (typeKind == TypeKind.Interface || isAbstract)
		{
			return;
		}

		// Can't be static.
		if (isStatic)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0403, identifier.GetLocation(), messageArgs: null));
			return;
		}


		// Can't return void.
		if (returnType.SpecialType == SpecialType.System_Void)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0404, identifier.GetLocation(), messageArgs: null));
			return;
		}

		// Must return ref or ref-readonly type.
		if (refKind is not (RefKind.Ref or RefKind.RefReadOnly))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0406, identifier.GetLocation(), messageArgs: null));
			return;
		}

		// Checks whether the [EditorBrowsable(EditorBrowsableState.Never)] exists.
		// Please note that 'object' typed value can't use 'is constant' pattern to check,
		// because 'o is constant' is equivalent to 'o is type && (type)o == constant',
		// where 'o is type' always returns false (because here 'o' is an 'object' instead of 'type').
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(EditorBrowsableAttribute).FullName);
		var attributeData = symbol.GetAttributes().FirstOrDefault(
			attributeData =>
				attributeData is { AttributeClass: var attribute, ConstructorArguments.Length: 1 }
				&& SymbolEqualityComparer.Default.Equals(attributeSymbol, attribute)
		);

		var (descriptor, location) = attributeData switch
		{
			null => (SCA0402, identifier.GetLocation()),

			{
				ConstructorArguments: [{ Value: EditorBrowsableState cArg }],
				ApplicationSyntaxReference: { Span: var span, SyntaxTree: var syntaxTree }
			}
			when cArg == EditorBrowsableState.Always => (SCA0405, Location.Create(syntaxTree, span)),

			_ => (null, null)
		};

		if (descriptor is not null)
		{
			Diagnostics.Add(Diagnostic.Create(descriptor, location!, messageArgs: null));
		}
	}
}
