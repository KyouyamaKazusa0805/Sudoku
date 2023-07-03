namespace Sudoku.SourceGeneration.Handlers;

internal static class EqualityOperatorsHandler
{
	public static EqualityOperatorsCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
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
					TypeParameters: var typeParameters
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

		if (kind == TypeKind.Interface
			&& (typeParameters is not [{ ConstraintTypes: var constraintTypes }] || !constraintTypes.Contains(type, SymbolEqualityComparer.Default)))
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
				_ => throw new InvalidOperationException("Invalid status.")
			},
			1 => Behavior.StaticVirtual,
			2 => Behavior.StaticAbstract,
			_ => throw new InvalidOperationException("Invalid status.")
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
			_ => throw new InvalidOperationException("Invalid status.")
		};
		var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
		var otherModifiers = attribute.GetNamedArgument<string>("OtherModifiers") switch
		{
			{ } str => str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
			_ => Array.Empty<string>()
		};
		var typeArgumentsString = typeParameters is []
			? string.Empty
			: $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>";
		var typeNameString = $"{typeName}{typeArgumentsString}";
		var fullTypeNameString = $"{namespaceString}.{typeNameString}";
		var attributesMarked = behavior switch
		{
			Behavior.StaticAbstract
				=> "\r\n\t\t",
			Behavior.WithScopedInButDeprecated or Behavior.WithScopedRefReadOnlyButDeprecated or Behavior.DefaultButDeprecated
				=> """
				[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.ObsoleteAttribute("This operator is not recommended to be defined in a record struct, because it'll be auto-generated a pair of equality operators by compiler, without any modifiers modified two parameters.", false)]
				""",
			_
				=> """
				[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				"""
		};

		var operatorDeclaration = behavior switch
		{
			Behavior.Default or Behavior.DefaultButDeprecated
				=> $$"""
				/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						public static bool operator ==({{fullTypeNameString}} left, {{fullTypeNameString}} right)
							=> left.Equals(right);

						/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						public static bool operator !=({{fullTypeNameString}} left, {{fullTypeNameString}} right)
							=> !left.Equals(right);
				""",
			Behavior.WithScopedIn or Behavior.WithScopedInButDeprecated
				=> $$"""
				/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						public static bool operator ==(scoped in {{fullTypeNameString}} left, scoped in {{fullTypeNameString}} right)
							=> left.Equals(right);

						/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						public static bool operator !=(scoped in {{fullTypeNameString}} left, scoped in {{fullTypeNameString}} right)
							=> !left.Equals(right);
				""",
			Behavior.WithScopedRefReadOnly or Behavior.WithScopedRefReadOnlyButDeprecated
				=> $$"""
				/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						public static bool operator ==(scoped ref readonly {{fullTypeNameString}} left, scoped ref readonly {{fullTypeNameString}} right)
							=> left.Equals(right);

						/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						public static bool operator !=(scoped ref readonly {{fullTypeNameString}} left, scoped ref readonly {{fullTypeNameString}} right)
							=> !left.Equals(right);
				""",
			Behavior.StaticVirtual when typeParameters is [{ Name: var selfTypeParameterName }]
				=> $$"""
				/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						static virtual bool operator ==({{selfTypeParameterName}} left, {{selfTypeParameterName}} right)
							=> left.Equals(right);

						/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						static virtual bool operator !=({{selfTypeParameterName}} left, {{selfTypeParameterName}} right)
							=> !left.Equals(right);
				""",
			Behavior.StaticAbstract when typeParameters is [{ Name: var selfTypeParameterName }]
				=> $$"""
				/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						{{attributesMarked}}
						static abstract bool operator ==({{selfTypeParameterName}} left, {{selfTypeParameterName}} right);

						/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						{{attributesMarked}}
						static virtual bool operator !=({{selfTypeParameterName}} left, {{selfTypeParameterName}} right) => !(left == right);
				""",
			_ => null
		};
		if (operatorDeclaration is null)
		{
			return null;
		}

		return new(
			$$"""
			namespace {{namespaceString}}
			{
				partial {{typeKindString}} {{typeNameString}}
				{
					{{operatorDeclaration}}
				}
			}
			"""
		);
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<EqualityOperatorsCollectedResult> value)
		=> spc.AddSource(
			"EqualityOperators.g.cs",
			$"""
			// <auto-generated/>

			#nullable enable
			
			{string.Join("\r\n\r\n", from element in value select element.FinalString)}
			"""
		);
}

file enum Behavior
{
	DoNothing,
	Default,
	DefaultButDeprecated,
	WithScopedIn,
	WithScopedRefReadOnly,
	WithScopedInButDeprecated,
	WithScopedRefReadOnlyButDeprecated,
	StaticVirtual,
	StaticAbstract
}
