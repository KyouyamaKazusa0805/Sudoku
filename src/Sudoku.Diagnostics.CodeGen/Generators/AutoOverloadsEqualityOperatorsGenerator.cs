namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the code that overloads the <c><see langword="operator"/> ==</c>
/// or <c><see langword="operator"/> !=</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverloadsEqualityOperatorsGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoOverloadsEqualityOperatorsAttribute";


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(NodePredicate, GetValuesProvider)
				.Where(static element => element is not null)
				.Collect(),
			OutputSource
		);

	private static bool NodePredicate(SyntaxNode node, CancellationToken _)
		=> node is TypeDeclarationSyntax { Modifiers: var modifiers, AttributeLists.Count: > 0 }
			&& modifiers.Any(SyntaxKind.PartialKeyword);

	private static (INamedTypeSymbol, AttributeData)? GetValuesProvider(GeneratorSyntaxContext gsc, CancellationToken ct)
	{
		if (gsc is not { Node: TypeDeclarationSyntax n, SemanticModel: { Compilation: { } compilation } semanticModel })
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(n, ct) is not { ContainingType: null } typeSymbol)
		{
			return null;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a
		).FirstOrDefault();
		if (attributeData is null)
		{
			return null;
		}

		if (typeSymbol.GetMembers().OfType<IMethodSymbol>().Any(static m => m.Name is "op_Equality" or "op_Inequality"))
		{
			return null;
		}

		return (typeSymbol, attributeData);
	}

	private static void OutputSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, AttributeData)?> list)
	{
		var recordedList = new List<INamedTypeSymbol>();
		foreach (var v in list)
		{
			if (v is not var (type, attributeData))
			{
				continue;
			}

			if (recordedList.FindIndex(e => SymbolEqualityComparer.Default.Equals(e, type)) != -1)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			string inKeyword = attributeData.GetNamedArgument<bool>("EmitsInKeyword") ? "in " : string.Empty;
			var (nullableAnnotation, realComparisonExpression) =
				attributeData.GetNamedArgument<bool>("WithNullableAnnotation")
					? (
						"?",
						"(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false }"
					) : (
						string.Empty,
						"left.Equals(right)"
					);

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoOverloadsEqualityOperators}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Determines whether two instances hold a same value.
					/// </summary>
					/// <param name="left">The left-side instance to take part in the comparison operation.</param>
					/// <param name="right">The right-side instance to take part in the comparison operation.</param>
					/// <returns>A <see cref="bool"/> value indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverloadsEqualityOperatorsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator ==({{inKeyword}}{{fullName}}{{nullableAnnotation}} left, {{inKeyword}}{{fullName}}{{nullableAnnotation}} right)
						=> {{realComparisonExpression}};
					
					/// <summary>
					/// Determines whether two instances don't hold a same value.
					/// </summary>
					/// <param name="left">The left-side instance to take part in the comparison operation.</param>
					/// <param name="right">The right-side instance to take part in the comparison operation.</param>
					/// <returns>A <see cref="bool"/> value indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverloadsEqualityOperatorsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator !=({{inKeyword}}{{fullName}}{{nullableAnnotation}} left, {{inKeyword}}{{fullName}}{{nullableAnnotation}} right)
						=> !(left == right);
				}
				"""
			);
		}
	}
}
