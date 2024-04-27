namespace Sudoku.SourceGeneration.Handlers;

internal static class EqualsHandler
{
	public static string? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
	{
		if (gasc is not
			{
				Attributes: [var attribute],
				TargetSymbol: INamedTypeSymbol
				{
					TypeKind: var kind and (TypeKind.Struct or TypeKind.Class),
					Name: var typeName,
					IsRecord: false, // Records cannot manually overrides 'Equals' method.
					IsReadOnly: var isReadOnly,
					IsRefLikeType: var isRefStruct,
					TypeParameters: var typeParameters,
					ContainingNamespace: var @namespace,
					ContainingType: null // Must be top-level type.
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
		var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
		var behavior = attribute switch
		{
			{ ConstructorArguments: [{ Value: int behaviorRawValue }] } => behaviorRawValue switch
			{
				0 => (isRefStruct, kind) switch
				{
					(true, _) => Behavior.ReturnFalse,
					(_, TypeKind.Struct) => Behavior.IsCast,
					(_, TypeKind.Class) => Behavior.AsCast,
					_ => throw new InvalidOperationException("Invalid state.")
				},
				1 => Behavior.Throw,
				2 => Behavior.MakeAbstract,
				_ => throw new InvalidOperationException("Invalid state.")
			}
		};
		var otherModifiers = attribute.GetNamedArgument<string>("OtherModifiers") switch
		{
			{ } str => str.Split([' '], StringSplitOptions.RemoveEmptyEntries),
			_ => []
		};
		var typeArgumentsString = typeParameters is []
			? string.Empty
			: $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>";
		var typeNameString = $"{typeName}{typeArgumentsString}";
		var fullTypeNameString = $"global::{namespaceString}.{typeNameString}";
		var typeKindString = kind switch
		{
			TypeKind.Class => "class",
			TypeKind.Struct => "struct",
			_ => throw new InvalidOperationException("Invalid state.")
		};
		var otherModifiersString = otherModifiers.Length == 0 ? string.Empty : $"{string.Join(" ", otherModifiers)} ";
		if (behavior == Behavior.MakeAbstract)
		{
			return $$"""
				namespace {{namespaceString}}
				{
					partial {{typeKindString}} {{typeNameString}}
					{
						/// <inheritdoc cref="object.Equals(object?)"/>
						public {{otherModifiersString}}abstract override bool Equals([global::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj);
					}
				}
				""";
		}
		else
		{
			var inKeyword = isLargeStructure ? "in " : string.Empty;
			var expressionString = behavior switch
			{
				Behavior.ReturnFalse => "false",
				Behavior.IsCast => $"obj is {fullTypeNameString} comparer && Equals({inKeyword}comparer)",
				Behavior.AsCast => $"Equals(obj as {fullTypeNameString})",
				Behavior.Throw => """throw new global::System.NotSupportedException("This method is not supported or disallowed by author.")""",
				_ => throw new InvalidOperationException("Invalid state.")
			};
			var attributesMarked = isRefStruct
				? behavior == Behavior.ReturnFalse
					? """
					[global::System.ObsoleteAttribute("Calling this method is unexpected because author disallow you call this method on purpose.", true)]
					"""
					: """
					[global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
							[global::System.ObsoleteAttribute("Calling this method is unexpected because author disallow you call this method on purpose.", true)]
					"""
				: """
				[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				""";
			var readOnlyModifier = kind == TypeKind.Struct && !isReadOnly ? "readonly " : string.Empty;
			var isDeprecated = attributesMarked.Contains("ObsoleteAttribute");
			var suppress0809 = isDeprecated ? "#pragma warning disable CS0809\r\n\t" : "\t";
			var enable0809 = isDeprecated ? "#pragma warning restore CS0809\r\n\t" : string.Empty;
			return $$"""
				namespace {{namespaceString}}
				{
				{{suppress0809}}partial {{typeKindString}} {{typeNameString}}
					{
						/// <inheritdoc cref="object.Equals(object?)"/>
						{{attributesMarked}}
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(EqualsHandler).FullName}}", "{{Value}}")]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						public {{otherModifiersString}}override {{readOnlyModifier}}bool Equals([global::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj)
							=> {{expressionString}};
					}
				{{enable0809}}}
				""";
		}
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<string> value)
		=> spc.AddSource(
			"Equals.g.cs",
			$"""
			{Banner.AutoGenerated}

			#nullable enable
			
			{string.Join("\r\n\r\n", value)}
			"""
		);
}

file enum Behavior
{
	ReturnFalse,
	IsCast,
	AsCast,
	Throw,
	MakeAbstract
}
