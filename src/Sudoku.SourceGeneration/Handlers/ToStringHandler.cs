namespace Sudoku.SourceGeneration.Handlers;

internal static class ToStringHandler
{
	public static string? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [var attribute],
				TargetSymbol: INamedTypeSymbol
				{
					Name: var typeName,
					ContainingNamespace: var @namespace,
					TypeParameters: var typeParameters,
					TypeKind: var kind and (TypeKind.Class or TypeKind.Struct),
					IsRecord: var isRecord,
					IsReadOnly: var isReadOnly,
					IsRefLikeType: var isRefStruct,
					ContainingType: null
				} type,
				TargetNode: TypeDeclarationSyntax { ParameterList: var parameterList }
					and (RecordDeclarationSyntax or ClassDeclarationSyntax or StructDeclarationSyntax),
				SemanticModel: { Compilation: var compilation } semanticModel
			})
		{
			return null;
		}

		var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
		var typeParametersString = typeParameters is []
			? string.Empty
			: $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>";
		var typeNameString = $"{typeName}{typeParametersString}";
		var fullTypeNameString = $"global::{namespaceString}.{typeNameString}";

		const string formattableTypeName = "System.IFormattable";
		if (compilation.GetTypeByMetadataName(formattableTypeName) is not { } formattableTypeSymbol)
		{
			return null;
		}

		const string dataMemberAttributeTypeName = "System.SourceGeneration.PrimaryConstructorParameterAttribute";
		var dataMemberAttributeTypeNameSymbol = compilation.GetTypeByMetadataName(dataMemberAttributeTypeName);
		if (dataMemberAttributeTypeNameSymbol is null)
		{
			return null;
		}

		const string stringMemberAttributeName = "System.SourceGeneration.StringMemberAttribute";
		var stringMemberAttributeSymbol = compilation.GetTypeByMetadataName(stringMemberAttributeName);
		if (stringMemberAttributeSymbol is null)
		{
			return null;
		}

		const string formatProviderTypeName = "System.IFormatProvider";
		var formatProviderSymbol = compilation.GetTypeByMetadataName(formatProviderTypeName);
		if (formatProviderSymbol is null)
		{
			return null;
		}

		var referencedMembers = PrimaryConstructor.GetCorrespondingMemberNames(
			type,
			semanticModel,
			parameterList,
			dataMemberAttributeTypeNameSymbol,
			a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, stringMemberAttributeSymbol),
			symbol => (string?)symbol.GetAttributes().First(stringMemberAttirbuteMatcher).ConstructorArguments[0].Value ?? symbol.Name,
			cancellationToken
		);

		var behavior = (int)attribute.ConstructorArguments[0].Value! switch
		{
			0 => (isRefStruct, referencedMembers) switch
			{
				(true, _) => Behavior.Throw,
				_ when hasImpledFormattable(type) is var p and not null => p switch
				{
					true => Behavior.CallOverloadImplicit,
					_ => Behavior.CallOverloadExplicit
				},
				(_, []) => Behavior.RecordLike,
				(_, { Length: 1 }) => Behavior.Specified,
				_ => Behavior.RecordLike
			},
			1 => Behavior.CallOverloadExplicit,
			2 when referencedMembers.Length == 1 => Behavior.Specified,
			3 when referencedMembers.Length != 0 => Behavior.RecordLike,
			4 => Behavior.Throw,
			5 => Behavior.MakeAbstract,
			_ => Behavior.ReturnTypeName
		};
		var kindString = (isRecord, kind) switch
		{
			(true, TypeKind.Class) => "record",
			(true, TypeKind.Struct) => "record struct",
			(_, TypeKind.Class) => "class",
			(_, TypeKind.Struct) => "struct",
			_ => throw new InvalidOperationException("Invalid state.")
		};
		var otherModifiers = attribute.GetNamedArgument<string>("OtherModifiers") switch
		{
			{ } str => str.Split([' '], StringSplitOptions.RemoveEmptyEntries),
			_ => []
		};
		var otherModifiersString = otherModifiers.Length == 0 ? string.Empty : $"{string.Join(" ", otherModifiers)} ";
		if (behavior == Behavior.MakeAbstract)
		{
			return $$"""
				namespace {{namespaceString}}
				{
					partial {{kindString}} {{typeNameString}}
					{
						/// <inheritdoc cref="object.ToString"/>
						public {{otherModifiersString}}abstract override string ToString();
					}
				}
				""";
		}
		else
		{
			var expression = behavior switch
			{
				Behavior.ReturnTypeName => fullTypeNameString,
				Behavior.CallOverloadImplicit => "ToString(default(string), default(global::System.IFormatProvider))",
				Behavior.CallOverloadExplicit => "((global::System.IFormattable)this).ToString(null, null)",
				Behavior.Specified => referencedMembers[0].Name,
				Behavior.Throw => """throw new global::System.NotSupportedException("This method is not supported or disallowed by author.")""",
				Behavior.RecordLike
					=> $$$"""
					$"{{{typeName}}} {{ {{{string.Join(", ", f(referencedMembers))}}} }}"
					""",
				_ => throw new InvalidOperationException("Invalid state.")
			};

			var attributesMarked = isRefStruct && behavior is Behavior.Throw or Behavior.ReturnTypeName
				? behavior == Behavior.ReturnTypeName
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
			var enable0809 = isDeprecated ? "#pragma warning restore CS0809\r\n\t" : "\t";
			return $$"""
				namespace {{namespaceString}}
				{
				{{suppress0809}}partial {{kindString}} {{typeNameString}}
					{
						/// <inheritdoc cref="object.ToString"/>
						{{attributesMarked}}
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ToStringHandler).FullName}}", "{{Value}}")]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						public {{otherModifiersString}}override {{readOnlyModifier}}string ToString()
							=> {{expression}};
				{{enable0809}}}
				}
				""";
		}


		static IEnumerable<string> f((string Name, string ExtraData)[] referencedMembers)
			=>
			from referencedMember in referencedMembers
			let displayName = referencedMember.ExtraData
			let name = referencedMember.Name
			select $$"""{{displayName ?? $$"""{nameof({{name}})}"""}} = {{{name}}}""";

		bool? hasImpledFormattable(INamedTypeSymbol type)
		{
			if (type.Interfaces.Contains(formattableTypeSymbol, SymbolEqualityComparer.Default))
			{
				// Extra check: check wheteher the method is explicitly-impl'ed. If so, we should return 'false'.
#pragma warning disable format
				return type.GetMembers().OfType<IMethodSymbol>().Any(
					method => method is
					{
						Name: "ToString",
						IsImplicitlyDeclared: false,
						ExplicitInterfaceImplementations: not [],
						TypeParameters: [],
						Parameters: [
							{
								Type.SpecialType: System_String,
								NullableAnnotation: not NotAnnotated
							},
							{
								Type: var formatProviderType
							}
						],
						ReturnType.SpecialType: System_String
					} && SymbolEqualityComparer.IncludeNullability.Equals(formatProviderType, formatProviderSymbol)
				);
#pragma warning restore format
			}

			return type.AllInterfaces.Contains(formattableTypeSymbol, SymbolEqualityComparer.Default) ? false : null;
		}

		bool stringMemberAttirbuteMatcher(AttributeData a)
			=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, stringMemberAttributeSymbol);
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<string> value)
		=> spc.AddSource(
			"ToString.g.cs",
			$"""
			{Banner.AutoGenerated}

			#nullable enable
			
			{string.Join("\r\n\r\n", value)}
			"""
		);
}

file enum Behavior
{
	Throw,
	ReturnTypeName,
	CallOverloadExplicit,
	CallOverloadImplicit,
	Specified,
	RecordLike,
	MakeAbstract
}

