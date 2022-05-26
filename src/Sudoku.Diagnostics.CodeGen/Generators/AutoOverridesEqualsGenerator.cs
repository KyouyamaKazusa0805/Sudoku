namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates for the code that is for the overriden of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverridesEqualsGenerator : IIncrementalGenerator
{
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

		const string attributeFullName = "System.Diagnostics.CodeGen.AutoOverridesEqualsAttribute";
		var attributeTypeSymbol = compilation.GetTypeByMetadataName(attributeFullName);
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
			if (
#pragma warning disable IDE0055
				v is not (
					{ TypeKind: var typeKind, IsRecord: var isRecord, IsRefLikeType: var isRefStruct } type,
					{ ConstructorArguments: [var firstArg, ..] } attributeData
				)
#pragma warning restore IDE0055
			)
			{
				continue;
			}

			if (recordedList.FindIndex(e => SymbolEqualityComparer.Default.Equals(e, type)) != -1)
			{
				continue;
			}

			var members = type.GetAllMembers();
			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			var targetSymbolsRawString = new List<string>();
			bool isClass = typeKind == TypeKind.Class, isStruct = typeKind == TypeKind.Struct;
			if (isClass)
			{
				targetSymbolsRawString.Add("other is not null");
			}

			foreach (var typedConstant in firstArg is { Kind: TypedConstantKind.Array, Values: var array } ? array : ImmutableArray.Create(firstArg))
			{
				string memberName = (string)typedConstant.Value!;

				// Checks whether the specified member is in the target type.
				var selectedMembers = (from member in members where member.Name == memberName select member).ToArray();
				if (selectedMembers is not [var memberSymbol, ..])
				{
					continue;
				}

				switch (memberSymbol)
				{
					case IFieldSymbol { Name: var fieldName, Type: var fieldType }:
					{
						targetSymbolsRawString.Add($"{fieldName} == other.{fieldName}");
						break;
					}
					case IPropertySymbol { GetMethod.ReturnType: var propertyGetterType, Name: var propertyName }:
					{
						targetSymbolsRawString.Add($"{propertyName} == other.{propertyName}");
						break;
					}
					case IMethodSymbol
					{
						Name: var methodName,
						ReturnType: { SpecialType: not SpecialType.System_Void } methodReturnType,
						Parameters: []
					}:
					{
						targetSymbolsRawString.Add($"{methodName}() == other.{methodName}()");
						break;
					}
				}
			}

			var namedArgs = attributeData.NamedArguments;
			string inKeyword = attributeData.GetNamedArgument<bool>("EmitsInKeyword") ? "in " : string.Empty;
			string sealedKeyword = attributeData.GetNamedArgument<bool>("EmitsSealedKeyword") && isClass
				? "sealed "
				: string.Empty;
			bool isExplicitImpl = attributeData.GetNamedArgument<bool>("UseExplicitImplementation");

			string fullTypeName = type.ToDisplayString(TypeFormats.FullName);
			string typeKindString = type.GetTypeKindModifier();
			string nullableAttribute = isClass ? "[NotNullWhen(true)] " : string.Empty;
			string equalsObjectImpl = isStruct
				? $"obj is {fullTypeName} comparer && Equals(comparer)"
				: $"Equals(comparer as {fullTypeName})";
			string objectEquals = isRecord
				? $"\t// The method '{nameof(Equals)}(object?)' exists."
				: isRefStruct
				? "\t// The method cannot be generated because the type is a ref struct."
				: $$"""
					/// <inheritdoc cref="object.{{nameof(Equals)}}(object?)"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverridesEqualsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public {{sealedKeyword}}override {{readOnlyKeyword}}bool {{nameof(Equals)}}([global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
						=> {{equalsObjectImpl}};
				""";

			bool methodPredicate(IMethodSymbol method)
				=> method is
				{
					Name: nameof(object.Equals),
					Parameters: [{ Type: var parameterType, RefKind: var refKind }],
					ReturnType.SpecialType: SpecialType.System_Boolean
				} && SymbolEqualityComparer.Default.Equals(parameterType, type) && !isRecord
				&& refKind == (string.IsNullOrEmpty(inKeyword) ? RefKind.None : RefKind.In);
			string nullableAnnotation = typeKind == TypeKind.Class ? "?" : string.Empty;
			bool containsGenericEquals = type.GetMembers().OfType<IMethodSymbol>().Any(methodPredicate);
			string genericEqualsMethod = containsGenericEquals
				? "\t// The method already exists."
				: $$"""
					/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverridesEqualsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public {{readOnlyKeyword}}bool {{nameof(Equals)}}({{nullableAttribute}}{{inKeyword}}{{fullTypeName}}{{nullableAnnotation}} other)
						=> {{string.Join(" && ", targetSymbolsRawString)}};
				""";
			string genericEquals = isExplicitImpl switch
			{
				true => $$"""
					{{genericEqualsMethod}}
					
					/// <inheritdoc/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverridesEqualsGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					{{readOnlyKeyword}}bool global::System.IEquatable<{{fullTypeName}}>.{{nameof(Equals)}}({{nullableAttribute}}{{fullTypeName}}{{nullableAnnotation}} other)
						=> {{nameof(Equals)}}(other);
				""",
				_ => genericEqualsMethod
			};

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoOverridesEquals}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{typeKindString}} {{type.Name}}{{genericParameterList}}
				{
				{{objectEquals}}
				
				{{genericEquals}}
				}
				"""
			);

			recordedList.Add(type);
		}
	}
}
