namespace Sudoku.SourceGeneration.Handlers;

internal static class ComparisonOperatorsHandler
{
	public static string? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
	{
		if (gasc is not
			{
				Attributes.Length: 1,
				TargetSymbol: INamedTypeSymbol
				{
					Name: var typeName,
					ContainingNamespace: var @namespace,
					ContainingType: null,
					IsRecord: var isRecord,
					TypeKind: var kind and (TypeKind.Class or TypeKind.Struct or TypeKind.Interface),
					TypeParameters: var typeParameters,
					IsRefLikeType: var isRefStruct
				} type,
				SemanticModel.Compilation: var compilation
			})
		{
			return null;
		}

		const string largeStructAttributeName = "System.SourceGeneration.LargeStructureAttribute";
		if (compilation.GetTypeByMetadataName(largeStructAttributeName) is not { } largeStructAttribute)
		{
			return null;
		}

		var isLargeStructure = type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, largeStructAttribute));
		var behavior = (isRecord, kind, isLargeStructure) switch
		{
			(true, TypeKind.Class, _) => Behavior.DoNothing,
			(_, TypeKind.Class, _) => Behavior.Default,
			(true, TypeKind.Struct, true) => Behavior.WithScopedInButDeprecated,
			(true, TypeKind.Struct, _) => Behavior.DefaultButDeprecated,
			(_, TypeKind.Struct, true) => Behavior.WithScopedIn,
			(_, TypeKind.Struct, _) => Behavior.Default,
			_ => throw new InvalidOperationException("Invalid state.")
		};
		if (behavior == Behavior.DoNothing)
		{
			return null;
		}

		var typeKindString = (isRecord, kind) switch
		{
			(true, TypeKind.Class) => "record",
			(_, TypeKind.Class) => "class",
			(true, TypeKind.Struct) => "record struct",
			(_, TypeKind.Struct) => "struct",
			_ => throw new InvalidOperationException("Invalid state.")
		};
		var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
		var typeArgumentsString = typeParameters is []
			? string.Empty
			: $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>";
		var typeNameString = $"{typeName}{typeArgumentsString}";
		var fullTypeNameString = $"global::{namespaceString}.{typeNameString}";
		var attributesMarked = behavior switch
		{
			Behavior.WithScopedInButDeprecated or Behavior.DefaultButDeprecated
				=> """
				[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.ObsoleteAttribute("This operator is not recommended to be defined in a record struct, because it'll be auto-generated a pair of equality operators by compiler, without any modifiers modified two parameters.", false)]
				""",
			_
				=> """
				[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				"""
		};
		var inKeyword = isLargeStructure ? "in " : string.Empty;
		var (i1, i2, i3, i4) = (
			$"left.CompareTo({inKeyword}right) > 0",
			$"left.CompareTo({inKeyword}right) < 0",
			$"left.CompareTo({inKeyword}right) >= 0",
			$"left.CompareTo({inKeyword}right) <= 0"
		);

		var explicitImplementation = string.Empty;
		var equalityOperatorsType = compilation.GetTypeByMetadataName("System.Numerics.IComparisonOperators`3")!
			.Construct(type, type, compilation.GetSpecialType(System_Boolean));
		if (behavior is Behavior.WithScopedIn or Behavior.WithScopedInButDeprecated
			&& type.AllInterfaces.Contains(equalityOperatorsType, SymbolEqualityComparer.Default))
		{
			explicitImplementation =
				$$"""
				/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IComparisonOperators<{{fullTypeNameString}}, {{fullTypeNameString}}, bool>.operator >({{fullTypeNameString}} left, {{fullTypeNameString}} right) => left > right;

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IComparisonOperators<{{fullTypeNameString}}, {{fullTypeNameString}}, bool>.operator <({{fullTypeNameString}} left, {{fullTypeNameString}} right) => left < right;

						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IComparisonOperators<{{fullTypeNameString}}, {{fullTypeNameString}}, bool>.operator >=({{fullTypeNameString}} left, {{fullTypeNameString}} right) => left >= right;

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IComparisonOperators<{{fullTypeNameString}}, {{fullTypeNameString}}, bool>.operator <=({{fullTypeNameString}} left, {{fullTypeNameString}} right) => left <= right;
				""";
		}

		var operatorDeclaration = behavior switch
		{
			Behavior.Default or Behavior.DefaultButDeprecated
				=> $$"""
				/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator >({{fullTypeNameString}} left, {{fullTypeNameString}} right)
							=> {{i1}};

						/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator <({{fullTypeNameString}} left, {{fullTypeNameString}} right)
							=> {{i2}};

						/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator >=({{fullTypeNameString}} left, {{fullTypeNameString}} right)
							=> {{i3}};

						/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator <=({{fullTypeNameString}} left, {{fullTypeNameString}} right)
							=> {{i4}};
				""",
			Behavior.WithScopedIn or Behavior.WithScopedInButDeprecated
				=> $$"""
				/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator >(in {{fullTypeNameString}} left, in {{fullTypeNameString}} right)
							=> {{i1}};

						/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator <(in {{fullTypeNameString}} left, in {{fullTypeNameString}} right)
							=> {{i2}};

						/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator >=(in {{fullTypeNameString}} left, in {{fullTypeNameString}} right)
							=> {{i3}};

						/// <inheritdoc cref="global::System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ComparisonOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator <=(in {{fullTypeNameString}} left, in {{fullTypeNameString}} right)
							=> {{i4}};

						{{explicitImplementation}}
				""",
			_ => null
		};
		if (operatorDeclaration is null)
		{
			return null;
		}

		return $$"""
			namespace {{namespaceString}}
			{
				partial {{typeKindString}} {{typeNameString}}
				{
					{{operatorDeclaration}}
				}
			}
			""";
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<string> value)
		=> spc.AddSource(
			"ComparisonOperators.g.cs",
			$"""
			{Banner.AutoGenerated}

			#nullable enable
			
			{string.Join("\r\n\r\n", value)}
			"""
		);
}

file enum Behavior
{
	DoNothing,
	Default,
	DefaultButDeprecated,
	WithScopedIn,
	WithScopedInButDeprecated,
}
