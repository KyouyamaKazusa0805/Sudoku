namespace Sudoku.SourceGeneration.Handlers;

internal static class ExplicitlyImplHandler
{
	public static ExplicitlyImplCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
	{
		const string comma = ", ";
		if (gasc is not
			{
				Attributes: var attributesData,
				TargetSymbol: IMethodSymbol
				{
					Name: var methodName,
					TypeParameters: var typeParametersMethod,
					ContainingType: INamedTypeSymbol
					{
						Name: var typeName,
						ContainingType: null,
						IsRecord: var isRecord,
						TypeKind: var kind and (TypeKind.Struct or TypeKind.Class or TypeKind.Interface),
						TypeParameters: var typeParametersType,
						ContainingNamespace: var @namespace,
						Interfaces: var impledInterfaces and not []
					}
				}
			})
		{
			return null;
		}

		var namespaceString = toName(@namespace)["global::".Length..];
		var typeParametersMethodStr = typeParametersMethod is []
			? string.Empty
			: $"<{string.Join(comma, from typeParameter in typeParametersMethod select typeParameter.Name)}>";
		var typeParametersConstraintMethodStr = typeParametersMethod is []
			? "\t\t\t"
			: $$"""
			{{string.Join(
				"\r\n\t\t\t",
				from typeParameter in typeParametersMethod
				let constraintStr = getConstraintFinalString(typeParameter)
				select $"where {typeParameter.Name} : {constraintStr}"
			)}}
						
			""";
		var typeArgumentsString = typeParametersType is []
			? string.Empty
			: $"<{string.Join(comma, from typeParameter in typeParametersType select typeParameter.Name)}>";
		var typeNameString = $"{typeName}{typeArgumentsString}";
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

		var implsGroup = new List<string>();
		foreach (var attributeData in attributesData)
		{
			IMethodSymbol? methodOrOperator;
			INamedTypeSymbol containingInterfaceType;
			switch (attributeData.ConstructorArguments, impledInterfaces)
			{
				case ([{ Kind: TypedConstantKind.Type, Value: null }], [var interfaceTypeDirectlyImpled]):
				{
					var methodsOrOperators = interfaceTypeDirectlyImpled.GetMembers().OfType<IMethodSymbol>();
					if (methodsOrOperators.FirstOrDefault(methodOrOperator => methodOrOperator.Name == methodName) is not { } foundMember)
					{
						continue;
					}

					methodOrOperator = foundMember;
					containingInterfaceType = interfaceTypeDirectlyImpled;
					break;
				}
				case ([{ Kind: TypedConstantKind.Type, Value: INamedTypeSymbol { TypeKind: TypeKind.Interface } interfaceTypeSymbol }], _):
				{
					var unboundedInterfaceTypeSymbol = unbound(interfaceTypeSymbol);
					if (impledInterfaces.FirstOrDefault(interfaceMatcher) is not { } foundInterfaceType)
					{
						continue;
					}

					var methodsOrOperators = foundInterfaceType.GetMembers().OfType<IMethodSymbol>();
					if (methodsOrOperators.FirstOrDefault(methodMatcher) is not { } foundMember)
					{
						continue;
					}

					methodOrOperator = foundMember;
					containingInterfaceType = foundInterfaceType;
					break;


					bool interfaceMatcher(INamedTypeSymbol i) => SymbolEqualityComparer.Default.Equals(unbound(i), unboundedInterfaceTypeSymbol);

					bool methodMatcher(IMethodSymbol methodOrOperator) => methodOrOperator.Name == methodName;
				}
				default:
				{
					methodOrOperator = null;
					containingInterfaceType = null!;
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
			var returnTypeStr = toName(methodOrOperator.ReturnType);
			var interfaceTypeName = toName(containingInterfaceType);
			var interfaceTypeArguments = containingInterfaceType switch
			{
				{
					IsGenericType: true,
					TypeArguments: var typeArgsForImpledInterface,
					TypeArgumentNullableAnnotations: var typeArgsNullabilityForImpledInterface
				}
				when typeArgsForImpledInterface.Zip(typeArgsNullabilityForImpledInterface, typeInfoMerger) is var pairedTypeInfo
					=> $"<{string.Join(comma, from pair in pairedTypeInfo select $"{toName(pair.Type)}{nullabilityToken(pair.Nullability)}")}>",
				_ => string.Empty
			};
			var parametersList = string.Join(
				comma,
				from parameter in parameters
				let modifier = (parameter.ScopedKind, parameter.RefKind) switch
				{
					(ScopedKind.None, RefKind.Ref) => "ref ",
					//(ScopedKind.None, RefKind.RefReadOnly) => "ref readonly ",
					(ScopedKind.None, RefKind.In) => "in ",
					(_, RefKind.Ref) => "scoped ref ",
					//(_, RefKind.RefReadOnly) => "scoped ref readonly ",
					(_, RefKind.In) => "scoped in ",
					(_, RefKind.Out) => "out ",
					_ => string.Empty
				}
				select $"{modifier}{toName(parameter.Type)} {parameter.Name}"
			);

			var parametersListWithoutType = string.Join(", ", from parameter in parameters select parameter.Name);
			var expressionStr = methodName switch
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
				"op_Negation" when parameters is [{ Name: var a }] => $"!{a}",
				"op_OnesComplement" when parameters is [{ Name: var a }] => $"~{a}",
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
				_ => $"{methodName}({parametersListWithoutType})"
			};
			var operatorSignStr = methodName switch
			{
				"op_Increment" => "++",
				"op_CheckedIncrement" => "checked ++",
				"op_Decrement" => "--",
				"op_CheckedDecrement" => "checked --",
				"op_UnaryPlus" => "+",
				"op_UnaryNegation" => "-",
				"op_CheckedUnaryNegation" => "checked -",
				"op_Negation" => "!",
				"op_OnesComplement" => "~",
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
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(ExplicitlyImplHandler).FullName}}", "{{Value}}")]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						{{staticModifier}}{{conversionOperatorModifier}}{{readOnlyModifier}}{{signature}}({{parametersList}})
				{{typeParametersConstraintMethodStr}}=> {{expressionStr}};
				"""
			);
		}

		return new(
			$$"""
			namespace {{namespaceString}}
			{
				partial {{typeKindString}} {{typeNameString}}
				{
					{{string.Join("\r\n\r\n\t\t", implsGroup)}}
				}
			}
			"""
		);


		static (ITypeSymbol Type, NullableAnnotation Nullability) typeInfoMerger(ITypeSymbol a, NullableAnnotation b)
			=> (Type: a, Nullability: b);

		static INamedTypeSymbol unbound(INamedTypeSymbol self) => self.IsGenericType ? self.ConstructUnboundGenericType() : self;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string toName<T>(T symbol) where T : ISymbol => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		static string nullabilityToken(NullableAnnotation annotation) => annotation == Annotated ? "?" : string.Empty;

		static string getConstraintFinalString(ITypeParameterSymbol symbol)
		{
			if (symbol.HasUnsupportedMetadata)
			{
				return string.Empty;
			}

			var constraintItems = new List<string>();
			if (symbol.HasReferenceTypeConstraint)
			{
				var token = symbol.ReferenceTypeConstraintNullableAnnotation == Annotated ? "?" : string.Empty;
				constraintItems.Add($"class{token}");
			}
			if (symbol.HasUnmanagedTypeConstraint)
			{
				constraintItems.Add("unmanaged");
			}
			if (symbol.HasValueTypeConstraint)
			{
				constraintItems.Add("struct");
			}
			if (symbol.HasNotNullConstraint)
			{
				constraintItems.Add("notnull");
			}
			if (symbol.BaseType is { } baseTypeForTypeParameter)
			{
				constraintItems.Add(toName(baseTypeForTypeParameter));
			}
			if (symbol is { ConstraintTypes: var impledInterfacesForTypeParameter and not [], ConstraintNullableAnnotations: var annotations })
			{
				constraintItems.Add(
					string.Join(
						comma,
						from pair in impledInterfacesForTypeParameter.Zip(annotations, typeInfoMerger)
						let nullabilityToken = pair.Nullability == Annotated ? "?" : string.Empty
						select $"{toName(pair.Type)}{nullabilityToken}"
					)
				);
			}
			if (symbol.HasConstructorConstraint)
			{
				constraintItems.Add("new()");
			}
			if (constraintItems.Count == 0)
			{
				constraintItems.Add("default");
			}

			return string.Join(comma, constraintItems);
		}
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<ExplicitlyImplCollectedResult> value)
		=> spc.AddSource(
			"ExplicitlyImpl.g.cs",
			$"""
			// <auto-generated/>

			#nullable enable
			
			{string.Join("\r\n\r\n", from element in value select element.FinalString)}
			"""
		);
}
