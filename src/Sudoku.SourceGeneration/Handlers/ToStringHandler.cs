namespace Sudoku.SourceGeneration.Handlers;

internal static class ToStringHandler
{
	public static ToStringCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
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
		var fullTypeNameString = $"{namespaceString}.{typeNameString}";

		var simpleFormattableTypeName = "System.ISimpleFormattable";
		if (compilation.GetTypeByMetadataName(simpleFormattableTypeName) is not { } simpleFormattableTypeSymbol)
		{
			return null;
		}

		const string primaryConstructorParameterAttributeTypeName = "System.SourceGeneration.PrimaryConstructorParameterAttribute";
		var primaryConstructorParameterAttributeSymbol = compilation.GetTypeByMetadataName(primaryConstructorParameterAttributeTypeName);
		if (primaryConstructorParameterAttributeSymbol is null)
		{
			return null;
		}

		const string stringMemberAttributeName = "System.SourceGeneration.StringMemberAttribute";
		var stringMemberAttributeSymbol = compilation.GetTypeByMetadataName(stringMemberAttributeName);
		if (stringMemberAttributeSymbol is null)
		{
			return null;
		}

		var referencedMembers = PrimaryConstructor.GetCorrespondingMemberNames(
			type,
			semanticModel,
			parameterList,
			primaryConstructorParameterAttributeSymbol,
			a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, stringMemberAttributeSymbol),
			symbol => (string?)symbol.GetAttributes().First(stringMemberAttirbuteMatcher).ConstructorArguments[0].Value ?? symbol.Name,
			cancellationToken
		);

		var behavior = (int)attribute.ConstructorArguments[0].Value! switch
		{
			0 => (isRefStruct, referencedMembers) switch
			{
				(true, _) => Behavior.Throw,
				_ when hasImpledFormattable(type) => Behavior.CallOverload,
				(_, []) => Behavior.RecordLike,
				(_, { Length: 1 }) => Behavior.Specified,
				_ => Behavior.RecordLike
			},
			1 => Behavior.CallOverload,
			2 when referencedMembers.Length == 1 => Behavior.Specified,
			3 when referencedMembers.Length != 0 => Behavior.RecordLike,
			4 => Behavior.Throw,
			_ => Behavior.ReturnTypeName
		};

		var expression = behavior switch
		{
			Behavior.ReturnTypeName => fullTypeNameString,
			Behavior.CallOverload => "ToString(default(string))",
			Behavior.Specified => referencedMembers[0].Name,
			Behavior.Throw => """throw new global::System.NotSupportedException("This method is not supported or disallowed by author.")""",
			Behavior.RecordLike
				=> $$$"""
				$"{{{typeName}}} {{ {{{string.Join(", ", f(referencedMembers))}}} }}"
				""",
			_ => throw new InvalidOperationException("Invalid status.")
		};

		var kindString = (isRecord, kind) switch
		{
			(true, TypeKind.Class) => "record",
			(true, TypeKind.Struct) => "record struct",
			(_, TypeKind.Class) => "class",
			(_, TypeKind.Struct) => "struct",
			_ => throw new InvalidOperationException("Invalid status.")
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
		var suppress0809 = isDeprecated
			? "#pragma warning disable CS0809\r\n\t"
			: "\t";
		var enable0809 = isDeprecated
			? "#pragma warning restore CS0809\r\n\t"
			: "\t";
		var otherModifiers = attribute.GetNamedArgument<string>("OtherModifiers") switch
		{
			{ } str => str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
			_ => Array.Empty<string>()
		};
		var otherModifiersString = otherModifiers.Length == 0 ? string.Empty : $"{string.Join(" ", otherModifiers)} ";
		var finalString =
			$$"""
			namespace {{namespaceString}}
			{
			{{suppress0809}}partial {{kindString}} {{typeNameString}}
				{
					/// <inheritdoc cref="object.GetHashCode"/>
					{{attributesMarked}}
					[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(GetHashCodeHandler).FullName}}", "{{Value}}")]
					[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					public {{otherModifiersString}}override {{readOnlyModifier}}string ToString()
						=> {{expression}};
			{{enable0809}}}
			}
			""";

		return new(finalString);


		bool hasImpledFormattable(INamedTypeSymbol type)
			=> type.AllInterfaces.Contains(simpleFormattableTypeSymbol, SymbolEqualityComparer.Default);

		bool stringMemberAttirbuteMatcher(AttributeData a)
			=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, stringMemberAttributeSymbol);

		static IEnumerable<string> f((string Name, string ExtraData)[] referencedMembers)
			=>
			from referencedMember in referencedMembers
			let displayName = referencedMember.ExtraData
			let name = referencedMember.Name
			select $$"""{{displayName ?? $$"""{nameof({{name}})}"""}} = {{{name}}}""";
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<ToStringCollectedResult> value)
		=> spc.AddSource(
			"ToString.g.cs",
			$"""
			// <auto-generated/>

			#nullable enable
			
			{string.Join("\r\n\r\n", from element in value select element.FinalString)}
			"""
		);
}

file enum Behavior
{
	Throw,
	ReturnTypeName,
	CallOverload,
	Specified,
	RecordLike
}

