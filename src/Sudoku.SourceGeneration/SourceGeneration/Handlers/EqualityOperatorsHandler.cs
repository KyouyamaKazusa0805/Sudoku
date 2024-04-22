namespace Sudoku.SourceGeneration.Handlers;

internal static class EqualityOperatorsHandler
{
	public static string? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawBehavior }] } attribute],
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

		var hasSelfTypeArgument = kind == TypeKind.Interface
			? typeParameters is not [{ ConstraintTypes: var constraintTypes }] || !constraintTypes.Contains(type, SymbolEqualityComparer.Default)
			: default(bool?);
		if (hasSelfTypeArgument is false)
		{
			// If the type is an interface, we should check for whether its first type parameter is a self type parameter,
			// which means it should implement its containing interface type.
			return null;
		}

		var isLargeStructure = type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, largeStructAttribute));
		var behavior = rawBehavior switch
		{
			0 => (isRecord, kind, isLargeStructure) switch
			{
				(true, TypeKind.Class, _) => Behavior.DoNothing,
				(_, TypeKind.Class, _) => Behavior.Default,
				(true, TypeKind.Struct, true) => Behavior.WithScopedInButDeprecated,
				(true, TypeKind.Struct, _) => Behavior.DefaultButDeprecated,
				(_, TypeKind.Struct, true) => Behavior.WithScopedIn,
				(_, TypeKind.Struct, _) => Behavior.Default,
				(_, TypeKind.Interface, _) => Behavior.StaticAbstract,
				_ => throw new InvalidOperationException("Invalid state.")
			},
			1 => Behavior.StaticVirtual,
			2 => Behavior.StaticAbstract,
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
			(_, TypeKind.Interface) => "interface",
			_ => throw new InvalidOperationException("Invalid state.")
		};
		var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
		var typeArgumentsString = typeParameters is []
			? string.Empty
			: $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>";
		var typeNameString = $"{typeName}{typeArgumentsString}";
		var fullTypeNameString = $"global::{namespaceString}.{typeNameString}";
		const string nullableToken = "?";
		var nullabilityToken = (attribute.GetNamedArgument<int>("NullabilityPrefer"), kind) switch
		{
			(0, TypeKind.Class) or (2, _) => nullableToken,
			(0, TypeKind.Struct) or (1, _) => string.Empty,
			(0, TypeKind.Interface) => typeParameters[0] switch
			{
				{ HasNotNullConstraint: true } => nullableToken, // Unknown T.
				{ HasUnmanagedTypeConstraint: true } or { HasValueTypeConstraint: true } => string.Empty,
				{ HasReferenceTypeConstraint: true } => nullableToken,
				{ ReferenceTypeConstraintNullableAnnotation: Annotated } => nullableToken, // Reference type inferred.
				{ ConstraintNullableAnnotations: var annotations } when annotations.Contains(Annotated) => nullableToken, // Reference type inferred.
				_ => string.Empty
			},
			_ => throw new InvalidOperationException("Invalid state.")
		};
		var attributesMarked = behavior switch
		{
			Behavior.StaticAbstract => "\r\n\t\t",
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
		var (i1, i2) = nullabilityToken switch
		{
			nullableToken => (
				$$"""(left, right) switch { (null, null) => true, ({ } a, { } b) => a.Equals({{inKeyword}}b), _ => false }""",
				"!(left == right)"
			),
			_ => ($"left.Equals({inKeyword}right)", $"!(left == right)")
		};

		var explicitImplementation = string.Empty;
		var equalityOperatorsType = compilation.GetTypeByMetadataName("System.Numerics.IEqualityOperators`3")!
			.Construct(type, type, compilation.GetSpecialType(System_Boolean));
		if (behavior is Behavior.WithScopedIn or Behavior.WithScopedInButDeprecated
			&& type.AllInterfaces.Contains(equalityOperatorsType, SymbolEqualityComparer.Default))
		{
			explicitImplementation =
				$$"""
				/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IEqualityOperators<{{fullTypeNameString}}, {{fullTypeNameString}}, bool>.operator ==({{fullTypeNameString}} left, {{fullTypeNameString}} right) => left == right;

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IEqualityOperators<{{fullTypeNameString}}, {{fullTypeNameString}}, bool>.operator !=({{fullTypeNameString}} left, {{fullTypeNameString}} right) => left != right;
				""";
		}

		var operatorDeclaration = behavior switch
		{
			Behavior.Default or Behavior.DefaultButDeprecated
				=> $$"""
				/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator ==({{fullTypeNameString}}{{nullabilityToken}} left, {{fullTypeNameString}}{{nullabilityToken}} right)
							=> {{i1}};

						/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator !=({{fullTypeNameString}}{{nullabilityToken}} left, {{fullTypeNameString}}{{nullabilityToken}} right)
							=> {{i2}};
				""",
			Behavior.WithScopedIn or Behavior.WithScopedInButDeprecated
				=> $$"""
				/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator ==(in {{fullTypeNameString}}{{nullabilityToken}} left, in {{fullTypeNameString}}{{nullabilityToken}} right)
							=> {{i1}};

						/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						public static bool operator !=(in {{fullTypeNameString}}{{nullabilityToken}} left, in {{fullTypeNameString}}{{nullabilityToken}} right)
							=> {{i2}};

						{{explicitImplementation}}
				""",
			Behavior.StaticVirtual when typeParameters is [{ Name: var selfTypeParameterName }]
				=> $$"""
				/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						static virtual bool operator ==({{selfTypeParameterName}} left, {{selfTypeParameterName}} right)
							=> {{i1}};

						/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						static virtual bool operator !=({{selfTypeParameterName}} left, {{selfTypeParameterName}} right)
							=> {{i2}};
				""",
			Behavior.StaticAbstract when typeParameters is [{ Name: var selfTypeParameterName }]
				=> $$"""
				/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						static abstract bool operator ==({{selfTypeParameterName}}{{nullabilityToken}} left, {{selfTypeParameterName}}{{nullabilityToken}} right);

						/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualityOperatorsHandler).FullName}}", "{{Value}}")]
						static virtual bool operator !=({{selfTypeParameterName}}{{nullabilityToken}} left, {{selfTypeParameterName}}{{nullabilityToken}} right) => {{i2}};
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
			"EqualityOperators.g.cs",
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
	StaticVirtual,
	StaticAbstract
}
