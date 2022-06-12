namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for overloading on comparison operators
/// <c><![CDATA[>]]></c>, <c><![CDATA[<]]></c>, <c><![CDATA[>=]]></c> and <c><![CDATA[<=]]></c>.
/// </summary>
#if false
[Generator(LanguageNames.CSharp)]
#endif
public sealed class AutoOverloadsComparisonOperatorsGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoOverloadsComparisonOperatorsAttribute";


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(NodePredicate, GetValuesProvider)
				.Where(static element => element is not null)
				.Collect(),
			OutputSource
		);


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

		var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>();
		if (methods.Any(static m => m.Name is "op_GreaterThan" or "op_GreaterThanOrEqual" or "op_LessThan" or "op_LessThanOrEqual"))
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
			string fullName = type.ToDisplayString(TypeFormats.FullName);
			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoOverloadsComparisonOperators}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is less than
					/// the <paramref name="right"/>-side one.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverloadsComparisonOperatorsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator <({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) < 0;
					
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is less than
					/// the <paramref name="right"/>-side one, or they are considered equal.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverloadsComparisonOperatorsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator <=({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) <= 0;
					
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is greater than
					/// the <paramref name="right"/>-side one.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverloadsComparisonOperatorsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator >({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) > 0;
					
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is greater than
					/// the <paramref name="right"/>-side one, or they are considered equal.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverloadsComparisonOperatorsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator >=({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) >= 0;
				}
				"""
			);

			recordedList.Add(type);
		}
	}
}
