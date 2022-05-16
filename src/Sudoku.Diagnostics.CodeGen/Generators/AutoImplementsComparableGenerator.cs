namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation for the type <see cref="IComparable{T}"/>.
/// </summary>
/// <seealso cref="IComparable{T}"/>
[Generator(LanguageNames.CSharp)]
public sealed class AutoImplementsComparableGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoImplementsComparableAttribute";

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
		if (
			gsc is not
			{
				Node: TypeDeclarationSyntax { Modifiers: var modifiers } n,
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
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
		return attributeData is null ? null : (typeSymbol, attributeData);
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

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			string? memberName = (string?)attributeData.ConstructorArguments[0].Value;
			bool @explicit = attributeData.GetNamedArgument<bool>("UseExplicitImplementation");
			bool isStruct = type.TypeKind == TypeKind.Struct;
			string method = (@explicit, isStruct, memberName) switch
			{
				(false, true, not null) => $"""
					public {readOnlyKeyword}int CompareTo({fullName} other) => {memberName}.CompareTo({memberName});
				""",
				(false, true, null) => $"""
					public {readOnlyKeyword}int CompareTo({fullName} other) => CompareTo(other);
				""",
				(false, false, not null) => $$"""
					public {{readOnlyKeyword}}int CompareTo([global::System.Diagnostic.CodeAnalysis.DisallowNull] {{fullName}}? other)
					{
						global::System.ArgumentNullException.ThrowIfNull(other);
						
						return {{memberName}}.CompareTo({{memberName}});
					}
				""",
				(false, false, null) => $$"""
					public {{readOnlyKeyword}}int CompareTo([global::System.Diagnostic.CodeAnalysis.DisallowNull] {{fullName}}? other)
					{
						global::System.ArgumentNullException.ThrowIfNull(other);
						
						return CompareTo(other);
					}
				""",
				(true, true, not null) => $$"""
					{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}} other)
						=> {{memberName}}.CompareTo({{memberName}});
				""",
				(true, true, null) => $$"""
					{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}} other)
						=> CompareTo(other);
				""",
				(true, false, not null) => $$"""
					{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}}? other)
					{
						global::System.ArgumentNullException.ThrowIfNull(other);
						
						return {{memberName}}.CompareTo({{memberName}});
					}
				""",
				(true, false, null) => $$"""
					{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}}? other)
					{
						global::System.ArgumentNullException.ThrowIfNull(other);
						
						return CompareTo(other);
					}
				"""
			};

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoImplementsComparable}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Compares the current instance with another object of the same type and returns an integer
					/// that indicates whether the current instance precedes, follows, or occurs in the same position
					/// in the sort order as the other object.
					/// </summary>
					/// <param name="other">An object to compare with this instance.</param>
					/// <returns>
					/// A value that indicates the relative order of the objects being compared.
					/// The return value has these meanings:
					/// <list type="table">
					/// <listheader>
					/// <term>Value</term>
					/// <description>Meaning</description>
					/// </listheader>
					/// <item>
					/// <term>Less than zero</term>
					/// <description>This instance precedes <paramref name="other"/> in the sort order.</description>
					/// </item>
					/// <item>
					/// <term>Zero</term>
					/// <description>
					/// The instance occurs in the same position in the sort order as <paramref name="other"/>.
					/// </description>
					/// </item>
					/// <item>
					/// <term>Greater than zero</term>
					/// <description>The instance follows <paramref name="other"/> in the sort order.</description>
					/// </item>
					/// </list>
					/// </returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoImplementsComparableGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				{{method}}
				}
				"""
			);
		}
	}
}
