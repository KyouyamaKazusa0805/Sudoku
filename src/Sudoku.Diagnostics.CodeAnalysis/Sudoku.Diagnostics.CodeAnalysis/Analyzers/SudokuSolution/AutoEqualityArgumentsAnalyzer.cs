namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0405")]
public sealed partial class AutoEqualityArgumentsAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[]
			{
				SyntaxKind.ClassDeclaration,
				SyntaxKind.StructDeclaration,
				SyntaxKind.RecordDeclaration
			}
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

		if (
			originalNode is not TypeDeclarationSyntax
			{
				AttributeLists: { Count: not 0 } attributeLists,
				Members: { Count: not 0 } members
			}
		)
		{
			return;
		}

		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		var attributesData = semanticModel.GetDeclaredSymbol(originalNode, cancellationToken)!.GetAttributes();
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoEqualityAttribute).FullName);
		SyntaxNode? attribute = null;
		foreach (var a in attributesData)
		{
			if (f(a.AttributeClass, attributeSymbol))
			{
				foreach (var attributeList in attributeLists)
				{
					foreach (var currentAttribute in attributeList.Attributes)
					{
						if (
							currentAttribute is
							{
								Name: IdentifierNameSyntax
								{
									Identifier.ValueText: nameof(AutoEqualityAttribute) or "AutoEquality"
								}
							}
						)
						{
							attribute = currentAttribute;

							goto DetermineSyntaxNode;
						}
					}
				}
			}
		}

	DetermineSyntaxNode:
		if (attribute is not AttributeSyntax { ArgumentList.Arguments: { Count: not 0 } arguments })
		{
			return;
		}

		// Get built-in type symbols.
		var builtInTypes = GetBuiltInTypesWithoutOperatorEquality(compilation);

		// Iterate on each argument.
		foreach (var argument in arguments)
		{
			// Check the type symbol of the argument.
			if (
				argument is not
				{
					Expression: InvocationExpressionSyntax
					{
						Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" },
						ArgumentList.Arguments: { Count: 1 } nameofArgs
					} expression
				}
			)
			{
				continue;
			}

			var nameofArgExpr = nameofArgs[0].Expression;
			if (
				semanticModel.GetOperation(nameofArgExpr, cancellationToken) is not
				{
					Type: INamedTypeSymbol { BaseType: var baseType } argTypeSymbol
				}
			)
			{
				continue;
			}

			// Check whether the type is an enumeration type. If so, the enumeration has already implemented
			// the operator ==, so we should ignore on this case.
			if (baseType?.ToDisplayString(FormatOptions.TypeFormat) == typeof(Enum).FullName)
			{
				continue;
			}

			// Check whether the type contains the operator '=='.
			bool containsOperatorEquality = argTypeSymbol.MemberNames.Contains("op_Equality");
			if (containsOperatorEquality
				|| !containsOperatorEquality
				&& builtInTypes.Contains(argTypeSymbol, SymbolEqualityComparer.Default))
			{
				continue;
			}

			// If not, report it.
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0405,
					location: nameofArgExpr.GetLocation(),
					messageArgs: null
				)
			);
		}
	}

	/// <summary>
	/// Get all built in types that don't contain <c>operator ==</c>.
	/// </summary>
	/// <param name="compilation">The compilation.</param>
	/// <returns>The symbols.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static INamedTypeSymbol[] GetBuiltInTypesWithoutOperatorEquality(Compilation compilation) => new[]
	{
		compilation.GetSpecialType(SpecialType.System_Object),
		compilation.GetSpecialType(SpecialType.System_Boolean),
		compilation.GetSpecialType(SpecialType.System_SByte),
		compilation.GetSpecialType(SpecialType.System_Byte),
		compilation.GetSpecialType(SpecialType.System_Char),
		compilation.GetSpecialType(SpecialType.System_Single),
		compilation.GetSpecialType(SpecialType.System_Double),
		compilation.GetSpecialType(SpecialType.System_Int16),
		compilation.GetSpecialType(SpecialType.System_Int32),
		compilation.GetSpecialType(SpecialType.System_Int64),
		compilation.GetSpecialType(SpecialType.System_UInt16),
		compilation.GetSpecialType(SpecialType.System_UInt32),
		compilation.GetSpecialType(SpecialType.System_UInt64)
	};
}
