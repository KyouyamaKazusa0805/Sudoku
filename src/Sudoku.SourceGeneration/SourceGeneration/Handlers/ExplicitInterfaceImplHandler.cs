namespace Sudoku.SourceGeneration.Handlers;

internal static class ExplicitInterfaceImplHandler
{
	public static string? Transform(GeneratorSyntaxContext gsc, CancellationToken ct)
	{
		const string comma = ", ";
		if (gsc is not { Node: TypeDeclarationSyntax node, SemanticModel: { Compilation: var compilation } semanticModel })
		{
			return null;
		}

		const string attributeTypeName = "System.SourceGeneration.ExplicitInterfaceImplAttribute";
		if (compilation.GetTypeByMetadataName(attributeTypeName) is not { } attributeTypeSymbol)
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(node, ct) is not { } typeSymbol)
		{
			return null;
		}

		const string largeStructAttributeName = "System.SourceGeneration.LargeStructureAttribute";
		if (compilation.GetTypeByMetadataName(largeStructAttributeName) is not { } largeStructAttribute)
		{
			return null;
		}

		var dataList = new List<Data>();
		foreach (var methodSymbol in
			// Don't do this. Due to having used 'partial' keyword, a type can be split into multiple files.
			// However, a syntax node may not contain all members defined in one file when a type is partial.
			// If we do this, we can get all members defined in a type because such API has already merged members from multiple files.
			// Therefore, we may get a runtime error from source generator, telling us that generation may contain duplicate.
			//typeSymbol.GetMembers().OfType<IMethodSymbol>()
			from member in node.DescendantNodes()
			where member is MethodDeclarationSyntax or OperatorDeclarationSyntax or ConversionOperatorDeclarationSyntax
			select semanticModel.GetDeclaredSymbol(member, ct) as IMethodSymbol into methodSymbol
			where methodSymbol is not null
			select methodSymbol
		)
		{
			var attributesData = (
				from attributeData in methodSymbol.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attributeTypeSymbol, attributeData.AttributeClass)
				select attributeData
			).ToArray();
			if (attributesData.Length == 0)
			{
				continue;
			}

			dataList.Add(new(attributesData, methodSymbol));
		}
		if (dataList.Count == 0)
		{
			return null;
		}

		var finalMembers = new List<string>();
		var isFirstMeet = true;
		var globalNamespaceString = default(string);
		var globalTypeKindString = default(string)!;
		var globalTypeNameString = default(string)!;
		foreach (var data in dataList)
		{
			if (data is not
				{
					Attributes: var attributesData,
					TargetSymbol: IMethodSymbol
					{
						Name: var methodName,
						TypeParameters: var typeParametersMethod,
						ContainingType: INamedTypeSymbol
						{
							Name: var typeName,
							IsFileLocal: false,
							ContainingType: null,
							IsRecord: var isRecord,
							TypeKind: var kind and (TypeKind.Struct or TypeKind.Class or TypeKind.Interface),
							TypeParameters: var typeParametersType,
							ContainingNamespace: var @namespace,
							AllInterfaces: var impledInterfaces and not []
						}
					}
				})
			{
				continue;
			}

			var namespaceString = @namespace.ToName()["global::".Length..];
			var typeParametersMethodStr = typeParametersMethod is []
				? string.Empty
				: $"<{string.Join(comma, from typeParameter in typeParametersMethod select typeParameter.Name)}>";
			var typeParametersConstraintMethodStr = typeParametersMethod is []
				? "\t\t\t"
				: $$"""
					{{string.Join(
						"\r\n\t\t\t",
						from typeParameter in typeParametersMethod
						let constraintStr = typeParameter.GetConstraintFinalString()
						select $"where {typeParameter.Name} : {constraintStr}"
					)}}
							
				""";
			var typeParametersString = typeParametersType is []
				? string.Empty
				: $"<{string.Join(comma, from typeParameter in typeParametersType select typeParameter.Name)}>";
			var typeNameString = $"{typeName}{typeParametersString}";
			var fullTypeNameString = $"global::{namespaceString}.{typeNameString}";
			var typeKindString = (kind, isRecord) switch
			{
				(TypeKind.Class, true) => "record",
				(TypeKind.Class, _) => "class",
				(TypeKind.Struct, true) => "record struct",
				(TypeKind.Struct, _) => "struct",
				(TypeKind.Interface, _) => "interface",
				_ => throw new InvalidOperationException("Invalid state.")
			};

			if (isFirstMeet)
			{
				isFirstMeet = false;
				(globalNamespaceString, globalTypeKindString, globalTypeNameString) = (namespaceString, typeKindString, typeNameString);
			}

			var implsGroup = new List<string>();
			foreach (var attributeData in attributesData)
			{
				IMethodSymbol? methodOrOperator;
				INamedTypeSymbol containingInterfaceType;
				switch (attributeData.ConstructorArguments, impledInterfaces)
				{
					case ([{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol { TypeKind: TypeKind.Interface } interfaceTypeSymbol }], _):
					{
						var unboundedInterfaceTypeSymbol = interfaceTypeSymbol.Unbound();
						if (impledInterfaces.FirstOrDefault(interfaceMatcher) is not { } foundInterfaceType)
						{
							continue;
						}

						var methodsOrOperators = foundInterfaceType.GetMembers().OfType<IMethodSymbol>();
						if (methodsOrOperators.FirstOrDefault(methodMatcher) is not { } foundMember)
						{
							continue;
						}

						(methodOrOperator, containingInterfaceType) = (foundMember, foundInterfaceType);
						break;


						bool interfaceMatcher(INamedTypeSymbol i)
							=> SymbolEqualityComparer.Default.Equals(i.Unbound(), unboundedInterfaceTypeSymbol);

						bool methodMatcher(IMethodSymbol methodOrOperator) => methodOrOperator.Name == methodName;
					}
					default:
					{
						(methodOrOperator, containingInterfaceType) = (null, null!);
						break;
					}
				}
				if (methodOrOperator is null)
				{
					continue;
				}

				var parameters = methodOrOperator.Parameters;
				var staticModifier = methodOrOperator.IsStatic ? "static " : string.Empty;
				var readOnlyModifier = methodOrOperator.IsReadOnly ? "readonly " : string.Empty;
				var unsafeModifier = methodOrOperator.Parameters.Any(static param => param.Type is IPointerTypeSymbol or IFunctionPointerTypeSymbol)
					? "unsafe "
					: string.Empty;
				var conversionOperatorModifier = methodOrOperator switch
				{
					{ Name: "op_Explicit" } => "explicit ",
					{ Name: "op_Implicit" } => "implicit ",
					_ => string.Empty
				};
				var refOrRefReadOnlyReturnModifier = methodOrOperator switch
				{
					{ ReturnsByRefReadonly: true } => "ref readonly ",
					{ ReturnsByRef: true } => "ref ",
					_ => string.Empty
				};
				var returnTypeStr = methodOrOperator.ReturnType.ToName();
				var interfaceTypeName = containingInterfaceType.ToName();
				var interfaceTypeArguments = containingInterfaceType switch
				{
					{
						IsGenericType: true,
						TypeArguments: var typeArgs,
						TypeArgumentNullableAnnotations: var typeArgsNullability
					}
					when typeArgs.Zip(typeArgsNullability, static (a, b) => (Type: a, Nullability: b)) is var pairedTypeInfo
						=> $"<{string.Join(comma, from pair in pairedTypeInfo select $"{pair.Type.ToName()}{pair.Nullability.NullabilityToken()}")}>",
					_ => string.Empty
				};
				var parametersList = string.Join(
					comma,
					from parameter in parameters
					let attributeStrings = (
						from attribute in parameter.GetAttributes()
						let attributeTypeString = attribute.AttributeClass!.ToName()
						let constructorArguments = attribute.ConstructorArguments
						let constructorArgumentString = string.Join(
							comma,
							from argument in constructorArguments
							let kind = argument.Kind
							let value = argument.Value
							let type = argument.Type
							select kind switch
							{
								TypedConstantKind.Array => string.Join(", ", from argumentValue in argument.Values select value.ToString()),
								TypedConstantKind.Type => ((INamedTypeSymbol)value).ToName(),
								TypedConstantKind.Enum => $"({type.ToName()}){value!}",
								_ => value.ToString()
							}
						)
						let namedArguments = attribute.NamedArguments
						let namedArgumentString = namedArguments switch
						{
							not [] => $", {string.Join(
								comma,
								from argument in namedArguments
								let argumentName = argument.Key
								let argumentValue = argument.Value
								let kind = argumentValue.Kind
								let value = argumentValue.Value
								let type = argumentValue.Type
								select kind switch
								{
									TypedConstantKind.Array => string.Join(", ", from argumentValue in argumentValue.Values select value.ToString()),
									TypedConstantKind.Type => ((INamedTypeSymbol)value).ToName(),
									TypedConstantKind.Enum => $"({type.ToName()}){value!}",
									_ => value.ToString()
								}
							)}",
							_ => string.Empty
						}
						select $"{attributeTypeString}({constructorArgumentString}{namedArgumentString})"
					).ToArray()
					let finalAttributes = attributeStrings switch
					{
						{ Length: not 0 } => string.Concat(from attributeString in attributeStrings select $"[{attributeString}]"),
						_ => string.Empty
					}
					let parameterNullableToken = parameter switch
					{
						{ Type: { NullableAnnotation: Annotated, TypeKind: TypeKind.Class or TypeKind.Interface } } => "?",
						_ => string.Empty
					}
					let parameterIsInOperator = parameter.ContainingSymbol is IMethodSymbol { Name: ['o', 'p', '_', ..] }
					let modifier = (parameter.ScopedKind, parameter.RefKind) switch
					{
						(ScopedKind.None, RefKind.Ref) => "ref ",
						//(ScopedKind.None, RefKind.RefReadOnly) => "ref readonly ",
						(ScopedKind.None, RefKind.In or RefKind.RefReadOnlyParameter) => parameterIsInOperator ? "in " : "ref readonly ",
						(_, RefKind.Ref) => "ref ",
						//(_, RefKind.RefReadOnly) => "ref readonly ",
						(_, RefKind.In or RefKind.RefReadOnlyParameter) => parameterIsInOperator ? "in " : "ref readonly ",
						(_, RefKind.Out) => "out ",
						_ => string.Empty
					}
					select $"{finalAttributes}{modifier}{parameter.Type.ToName()}{parameterNullableToken} {parameter.Name}"
				);
				var parametersListWithoutType = string.Join(
					comma,
					from parameter in parameters
					let inKeyword = isLargeStructure(parameter.Type) ? "in " : string.Empty
					select $"{inKeyword}{parameter.Name}"
				);
				var expressionStr = methodName.GetMethodInvocation(parameters, returnTypeStr, parametersListWithoutType);
				var operatorSignStr = methodName.GetMethodDeclaration();
				var signature = methodName switch
				{
					"op_Explicit" or "op_CheckedExplicit" or "op_Implicit"
					when (methodName.Contains("Checked") ? "checked " : string.Empty) is var checkedModifier
						=> $$"""{{interfaceTypeName}}.operator {{checkedModifier}}{{returnTypeStr}}""",
					_ when methodName.StartsWith("op_")
						=> $$"""{{returnTypeStr}} {{interfaceTypeName}}.operator {{operatorSignStr}}""",
					_
						=> $$"""{{refOrRefReadOnlyReturnModifier}}{{returnTypeStr}} {{interfaceTypeName}}.{{methodName}}{{typeParametersMethodStr}}"""
				};
				implsGroup.Add(
					$$"""
					/// <inheritdoc/>
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ExplicitInterfaceImplHandler).FullName}}", "{{Value}}")]
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							{{staticModifier}}{{conversionOperatorModifier}}{{readOnlyModifier}}{{unsafeModifier}}{{signature}}({{parametersList}})
					{{typeParametersConstraintMethodStr}}=> {{expressionStr}};
					"""
				);
			}

			finalMembers.Add(
				$$"""
				//
						// Member '{{methodName}}'
						//
						{{string.Join("\r\n\r\n\t\t", implsGroup)}}
				"""
			);
		}

		if (globalNamespaceString is null)
		{
			// No members will satisfy the condition.
			return null;
		}

		return $$"""
			namespace {{globalNamespaceString}}
			{
				partial {{globalTypeKindString}} {{globalTypeNameString}}
				{
					{{string.Join("\r\n\r\n\r\n\t\t", finalMembers)}}
				}
			}
			""";


		bool isLargeStructure(ITypeSymbol type)
			=> type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, largeStructAttribute));
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<string> value)
		=> spc.AddSource(
			"ExplicitInterfaceImpl.g.cs",
			$"""
			{Banner.AutoGenerated}

			#pragma warning disable CS9192, CS9193, CS9195
			#nullable enable
			
			{string.Join("\r\n\r\n", value)}
			"""
		);
}

/// <summary>
/// The internal collected data.
/// </summary>
/// <param name="Attributes">The attributes data.</param>
/// <param name="TargetSymbol">The target method symbol.</param>
file sealed record Data(AttributeData[] Attributes, IMethodSymbol TargetSymbol);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	public static string GetMethodInvocation(this string @this, ImmutableArray<IParameterSymbol> parameters, string returnTypeStr, string parametersListWithoutType)
		=> @this switch
		{
			"op_Explicit" when parameters is [{ Name: var a }] => $"({returnTypeStr}){a}",
			"op_CheckedExplicit" when parameters is [{ Name: var a }] => $"checked(({returnTypeStr}){a})",
			"op_Implicit" when parameters is [{ Name: var a }] => a,
			"op_Increment" when parameters is [{ Name: var a }] => $"{a}++",
			"op_CheckedIncrement" when parameters is [{ Name: var a }] => $"checked({a}++)",
			"op_Decrement" when parameters is [{ Name: var a }] => $"{a}--",
			"op_CheckedDecrement" when parameters is [{ Name: var a }] => $"checked({a}--)",
			"op_UnaryPlus" when parameters is [{ Name: var a }] => $"+{a}",
			"op_UnaryNegation" when parameters is [{ Name: var a }] => $"-{a}",
			"op_CheckedUnaryNegation" when parameters is [{ Name: var a }] => $"checked(-{a})",
			"op_LogicalNot" when parameters is [{ Name: var a }] => $"!{a}",
			"op_OnesComplement" when parameters is [{ Name: var a }] => $"~{a}",
			"op_True" when parameters is [{ Name: var a }] => $"{a} ? true : false",
			"op_False" when parameters is [{ Name: var a }] => $"!({a} ? true : false)",
			"op_Addition" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} + {b}",
			"op_CheckedAddition" when parameters is [{ Name: var a }, { Name: var b }] => $"checked({a} + {b})",
			"op_Subtraction" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} - {b}",
			"op_CheckedSubtraction" when parameters is [{ Name: var a }, { Name: var b }] => $"checked({a} - {b})",
			"op_Multiply" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} * {b}",
			"op_CheckedMultiply" when parameters is [{ Name: var a }, { Name: var b }] => $"checked({a} * {b})",
			"op_Division" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} / {b}",
			"op_CheckedDivision" when parameters is [{ Name: var a }, { Name: var b }] => $"checked({a} / {b})",
			"op_Modulus" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} % {b}",
			"op_ExclusiveOr" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} ^ {b}",
			"op_BitwiseAnd" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} & {b}",
			"op_BitwiseOr" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} | {b}",
			"op_LeftShift" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} << {b}",
			"op_RightShift" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} >> {b}",
			"op_UnsignedRightShift" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} >>> {b}",
			"op_Equality" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} == {b}",
			"op_Inequality" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} != {b}",
			"op_GreaterThan" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} > {b}",
			"op_LessThan" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} < {b}",
			"op_GreaterThanOrEqual" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} >= {b}",
			"op_LessThanOrEqual" when parameters is [{ Name: var a }, { Name: var b }] => $"{a} <= {b}",
			_ => $"{@this}({parametersListWithoutType})"
		};

	public static string GetMethodDeclaration(this string @this)
		=> @this switch
		{
			"op_Increment" => "++",
			"op_CheckedIncrement" => "checked ++",
			"op_Decrement" => "--",
			"op_CheckedDecrement" => "checked --",
			"op_UnaryPlus" => "+",
			"op_UnaryNegation" => "-",
			"op_CheckedUnaryNegation" => "checked -",
			"op_LogicalNot" => "!",
			"op_OnesComplement" => "~",
			"op_True" => "true",
			"op_False" => "false",
			"op_Addition" => "+",
			"op_CheckedAddition" => "checked +",
			"op_Subtraction" => "-",
			"op_CheckedSubtraction" => "checked -",
			"op_Multiply" => "*",
			"op_CheckedMultiply" => "checked *",
			"op_Division" => "/",
			"op_CheckedDivision" => "checked /",
			"op_Modulus" => "%",
			"op_ExclusiveOr" => "^",
			"op_BitwiseAnd" => "&",
			"op_BitwiseOr" => "|",
			"op_LeftShift" => "<<",
			"op_RightShift" => ">>",
			"op_UnsignedRightShift" => ">>>",
			"op_Equality" => "==",
			"op_Inequality" => "!=",
			"op_GreaterThan" => ">",
			"op_LessThan" => "<",
			"op_GreaterThanOrEqual" => ">=",
			"op_LessThanOrEqual" => "<=",
			_ => string.Empty
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToName<T>(this T @this) where T : ISymbol => @this.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

	public static string GetConstraintFinalString(this ITypeParameterSymbol @this)
	{
		if (@this.HasUnsupportedMetadata)
		{
			return string.Empty;
		}

		var constraintItems = new List<string>();
		if (@this.HasReferenceTypeConstraint)
		{
			var token = @this.ReferenceTypeConstraintNullableAnnotation == Annotated ? "?" : string.Empty;
			constraintItems.Add($"class{token}");
		}
		if (@this.HasUnmanagedTypeConstraint)
		{
			constraintItems.Add("unmanaged");
		}
		if (@this.HasValueTypeConstraint)
		{
			constraintItems.Add("struct");
		}
		if (@this.HasNotNullConstraint)
		{
			constraintItems.Add("notnull");
		}
		if (@this.BaseType is { } baseTypeForTypeParameter)
		{
			constraintItems.Add(baseTypeForTypeParameter.ToName());
		}
		if (@this is { ConstraintTypes: var impledInterfacesForTypeParameter and not [], ConstraintNullableAnnotations: var annotations })
		{
			constraintItems.Add(
				string.Join(
					", ",
					from pair in impledInterfacesForTypeParameter.Zip(annotations, static (a, b) => (Type: a, Nullability: b))
					let nullabilityToken = pair.Nullability == Annotated ? "?" : string.Empty
					select $"{pair.Type.ToName()}{nullabilityToken}"
				)
			);
		}
		if (@this.HasConstructorConstraint)
		{
			constraintItems.Add("new()");
		}
		if (constraintItems.Count == 0)
		{
			constraintItems.Add("default");
		}

		return string.Join(", ", constraintItems);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string NullabilityToken(this NullableAnnotation @this) => @this == Annotated ? "?" : string.Empty;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static INamedTypeSymbol Unbound(this INamedTypeSymbol @this) => @this.IsGenericType ? @this.ConstructUnboundGenericType() : @this;
}
