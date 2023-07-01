namespace Sudoku.SourceGeneration.Handlers;

internal static class GetHashCodeHandler
{
	public static GetHashCodeCollectedResult_NewStyled? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
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

		const string primaryConstructorParameterAttributeTypeName = "System.SourceGeneration.PrimaryConstructorParameterAttribute";
		var primaryConstructorParameterAttributeSymbol = compilation.GetTypeByMetadataName(primaryConstructorParameterAttributeTypeName);
		if (primaryConstructorParameterAttributeSymbol is null)
		{
			return null;
		}

		const string Property = nameof(Property), Field = nameof(Field);
		const string hashCodeMemberAttributeTypeName = "System.SourceGeneration.HashCodeMemberAttribute";
		var hashCodeMemberAttributeSymbol = compilation.GetTypeByMetadataName(hashCodeMemberAttributeTypeName);
		if (hashCodeMemberAttributeSymbol is null)
		{
			return null;
		}

		var members = type.GetAllMembers().ToArray();
		var baseMembers =
			from member in members
			where member is IFieldSymbol or IPropertySymbol && member.GetAttributes().Any(hashCodeMemberAttributeMatcher)
			let needCombine = mayNotInvokeHashCodeCombine(member)
			select (member.Name, NeedCombine: needCombine);
		var referencedMembers = parameterList is null
			? baseMembers.ToArray()
			: (
				from parameter in parameterList.Parameters
				select semanticModel.GetDeclaredSymbol(parameter, cancellationToken) into parameterSymbol
				where !parameterSymbol.Type.IsRefLikeType // Ref structs cannot participate in the hashing.
				let attributesData = parameterSymbol.GetAttributes()
				where attributesData.Any(hashCodeMemberAttributeMatcher)
				let primaryConstructorParameterAttributeData = attributesData.FirstOrDefault(primaryConstructorParameterAttributeMatcher)
				where primaryConstructorParameterAttributeData is { ConstructorArguments: [{ Value: string }] }
				let parameterKind = (string)primaryConstructorParameterAttributeData.ConstructorArguments[0].Value!
				where parameterKind is Property or Field
				let memberConversion = parameterKind switch { Property => ">@", Field => "_<@", _ => "@" }
				let namedArguments = primaryConstructorParameterAttributeData.NamedArguments
				let parameterName = parameterSymbol.Name
				let referencedMemberName = PrimaryConstructor.GetTargetMemberName(namedArguments, parameterName, memberConversion)
				let needCombine = mayNotInvokeHashCodeCombine(parameterSymbol)
				select (Name: referencedMemberName, NeedCombine: needCombine)
			).Concat(baseMembers).ToArray();

		var behavior = (isRefStruct, attribute) switch
		{
			(true, _) => Behavior.Throw,
			(_, { ConstructorArguments: [{ Value: int behaviorRawValue }] }) => behaviorRawValue switch
			{
				0 => referencedMembers switch
				{
					[] => Behavior.ReturnNegativeOne,
					[(_, true)] => Behavior.Direct,
					[(_, false)] => Behavior.EnumExplicitCast,
					{ Length: > 8 } => Behavior.HashCodeAdd,
					_ => Behavior.Specified
				},
				1 => Behavior.Throw,
				_ => throw new InvalidOperationException("Invalid status.")
			},
			_ => throw new InvalidOperationException("Invalid status.")
		};

		var codeBlock = behavior switch
		{
			Behavior.ReturnNegativeOne
				=> """
					=> -1;
				""",
			Behavior.Direct
				=> $"""
					=> {referencedMembers[0].Name};
				""",
			Behavior.EnumExplicitCast
				=> $"""
					=> (int){referencedMembers[0].Name};
				""",
			Behavior.Specified
				=> $"""
					=> global::System.HashCode.Combine({string.Join(", ", from pair in referencedMembers select pair.Name)});
				""",
			Behavior.Throw
				=> """
					=> throw new global::System.NotSupportedException("This method is not supported or disallowed by author.");
				""",
			Behavior.HashCodeAdd
				=> $$"""
					{
						var hashCode = new global::System.HashCode();
						{{string.Join("\r\n\r\n", from member in referencedMembers select $"hashCode.Add({member.Name});")}}
						return hashCode.ToHashCode();
					}
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
		var attributesMarked = isRefStruct
			? behavior == Behavior.ReturnNegativeOne
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
					public {{otherModifiersString}}override {{readOnlyModifier}}int GetHashCode()
					{{codeBlock}}
			{{enable0809}}}
			}
			""";

		return new(finalString);


		bool primaryConstructorParameterAttributeMatcher(AttributeData a)
			=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, primaryConstructorParameterAttributeSymbol);

		bool hashCodeMemberAttributeMatcher(AttributeData a)
			=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, hashCodeMemberAttributeSymbol);

		static bool? mayNotInvokeHashCodeCombine(ISymbol symbol)
			=> symbol switch
			{
				IFieldSymbol { Type.SpecialType: System_Byte or System_SByte or System_Int16 or System_UInt16 or System_Int32 } => true,
				IFieldSymbol { Type.SpecialType: System_Enum } => false,
				IPropertySymbol { Type.SpecialType: System_Byte or System_SByte or System_Int16 or System_UInt16 or System_Int32 } => true,
				IPropertySymbol { Type.SpecialType: System_Enum } => false,
				IParameterSymbol { Type.SpecialType: System_Byte or System_SByte or System_Int16 or System_UInt16 or System_Int32 } => true,
				IParameterSymbol { Type.SpecialType: System_Enum } => false,
				_ => null
			};
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<GetHashCodeCollectedResult_NewStyled> value)
		=> spc.AddSource(
			"GetHashCode.g.cs",
			$"""
			// <auto-generated/>

			#nullable enable
			
			{string.Join("\r\n\r\n", from element in value select element.FinalString)}
			"""
		);
}

file enum Behavior
{
	ReturnNegativeOne,
	Direct,
	EnumExplicitCast,
	Specified,
	Throw,
	HashCodeAdd
}
