namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0122", "SCA0123", "SCA0124", "SCA0125")]
public sealed partial class InitializationOnlySyntaxChecker : ISyntaxContextReceiver
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

		var attribute = compilation.GetTypeSymbol<InitializationOnlyAttribute>();
		switch (node)
		{
			case TypeDeclarationSyntax:
			{
				var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
				if (symbol is not INamedTypeSymbol namedTypeSymbol)
				{
					break;
				}

				CheckAutoImplProperty(namedTypeSymbol, attribute);
				CheckAccessibilityOnField(namedTypeSymbol, attribute);
				CheckAccessibilityOnProperty(namedTypeSymbol, attribute);

				break;
			}
			default:
			{
				const string moduleInitializerAttributeFullName = "System.Runtime.CompilerServices.ModuleInitializerAttribute";
				var moduleInitializerAttribute = compilation.GetTypeByMetadataName(moduleInitializerAttributeFullName);
				if (moduleInitializerAttribute is null)
				{
					break;
				}

				var operation = semanticModel.GetOperation(node, _cancellationToken);
				CheckUsages(operation, attribute, semanticModel, moduleInitializerAttribute);

				break;
			}
		}
	}

	private void CheckAutoImplProperty(INamedTypeSymbol symbol, INamedTypeSymbol attribute)
	{
		foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>())
		{
			var attributesData = member.GetAttributes();
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			if (member.IsAutoProperty())
			{
				continue;
			}

			if (member is not { DeclaringSyntaxReferences: [{ Span: var span, SyntaxTree: var syntaxTree }] })
			{
				continue;
			}

			var location = Location.Create(syntaxTree, span);
			Diagnostics.Add(Diagnostic.Create(SCA0122, location, messageArgs: null));
		}
	}

	private void CheckAccessibilityOnField(INamedTypeSymbol symbol, INamedTypeSymbol attribute)
	{
		foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
		{
			var attributesData = member.GetAttributes();
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			if (
				member is not
				{
					DeclaringSyntaxReferences: [{ Span: var span, SyntaxTree: var syntaxTree }],
					DeclaredAccessibility: Accessibility.Public
				}
			)
			{
				continue;
			}

			var location = Location.Create(syntaxTree, span);
			Diagnostics.Add(Diagnostic.Create(SCA0123, location, messageArgs: null));
		}
	}

	private void CheckAccessibilityOnProperty(INamedTypeSymbol symbol, INamedTypeSymbol attribute)
	{
		foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>())
		{
			var attributesData = member.GetAttributes();
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			switch (member.SetMethod)
			{
				case null:
				{
					continue;
				}

				case
				{
					DeclaredAccessibility: Accessibility.Public,
					DeclaringSyntaxReferences: [{ Span: var span, SyntaxTree: var syntaxTree }]
				}:
				{
					var location = Location.Create(syntaxTree, span);
					Diagnostics.Add(Diagnostic.Create(SCA0124, location, messageArgs: null));

					break;
				}
			}
		}
	}

	private void CheckUsages(
		IOperation? operation,
		INamedTypeSymbol attribute,
		SemanticModel semanticModel,
		INamedTypeSymbol moduleInitializerAttribute
	)
	{
		switch (operation)
		{
			case IFieldReferenceOperation { Field: var field, Syntax: var syntax }
			when isMarked(field, out var data):
			{
				report(data!, syntax);

				break;
			}

			case IPropertyReferenceOperation { Property: var property, Syntax: var syntax }
			when isMarked(property, out var data):
			{
				report(data!, syntax);

				break;
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool isMarked(ISymbol symbol, out AttributeData? attributeData)
			{
				foreach (var a in symbol.GetAttributes())
				{
					if (!SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute))
					{
						continue;
					}

					attributeData = a;
					return true;
				}

				attributeData = null;
				return false;
			}

			void report(AttributeData data, SyntaxNode syntax)
			{
				if (data is not { ConstructorArguments: [{ Value: InitializationCaller.ModuleInitializer }] })
				{
					return;
				}

				foreach (var parent in syntax.Ancestors())
				{
					var tempSymbol = semanticModel.GetDeclaredSymbol(parent, _cancellationToken)!;
					if (tempSymbol is not IMethodSymbol)
					{
						continue;
					}

					var attributesData = tempSymbol.GetAttributes();
					if (attributesData.All(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, moduleInitializerAttribute)))
					{
						continue;
					}

					Diagnostics.Add(Diagnostic.Create(SCA0125, syntax.GetLocation(), messageArgs: null));
				}
			}
		}
	}
}
