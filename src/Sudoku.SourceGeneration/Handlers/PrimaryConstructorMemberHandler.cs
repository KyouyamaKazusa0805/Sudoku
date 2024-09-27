namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// The generator handler for primary constructor member parameters.
/// </summary>
internal static class PrimaryConstructorMemberHandler
{
	/// <inheritdoc/>
	public static void Output(SourceProductionContext spc, ImmutableArray<CollectedResult> values)
	{
		var types = new List<string>();
		foreach (var valuesGrouped in
			from tuple in values
			group tuple by $"{tuple.Namesapce}.{tuple.Type}" into @group
			select (CollectedResult[])[.. @group])
		{
			var (namespaceName, fieldDeclarations, propertyDeclarations) = (valuesGrouped[0].Namesapce, new List<string>(), new List<string>());
			foreach (
				var (
					parameterName, typeKind, refKind, scopedKind, nullableAnnotation, parameterType, typeSymbol, isReadOnly,
					_, _, _, attributesData, comment
				) in valuesGrouped
			)
			{
				switch (attributesData)
				{
					case [{ ConstructorArguments: [{ Value: LocalMemberKinds.Field }], NamedArguments: var namedArgs }]:
					{
						var targetMemberName = PrimaryConstructor.GetTargetMemberName(namedArgs, parameterName, "_<@");
						var accessibilityModifiers = getAccessibilityModifiers(namedArgs, "private ");
						var readonlyModifier = getReadOnlyModifier(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, true, true);
						var refModifiers = getRefModifiers(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, true);
						var docComments = getDocComments(comment);
						var parameterTypeName = getParameterType(parameterType, nullableAnnotation);
						var assigning = getAssigningExpression(refModifiers, parameterName);
						fieldDeclarations.Add(
							$"""
							/// <summary>
									{docComments ?? $"/// The generated field declaration for parameter <c>{parameterName}</c>."}
									/// </summary>
									[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{typeof(PrimaryConstructorMemberHandler).FullName}", "{Value}")]
									[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
									{accessibilityModifiers}{readonlyModifier}{refModifiers}{parameterTypeName}{targetMemberName} = {assigning};
							"""
						);

						break;
					}
					case [{ ConstructorArguments: [{ Value: LocalMemberKinds.Property }], NamedArguments: var namedArgs }]:
					{
						var targetMemberName = PrimaryConstructor.GetTargetMemberName(namedArgs, parameterName, ">@");
						var accessibilityModifiers = getAccessibilityModifiers(namedArgs, "public ");
						var setter = namedArgs.TryGetValueOrDefault<string>("SetterExpression", out var setterExpression)
							? $" {setterExpression};"
							: string.Empty;
						var readonlyModifier = getReadOnlyModifier(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, false, setter is []);
						var refModifiers = getRefModifiers(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, false);
						var docComments = getDocComments(comment);
						var parameterTypeName = getParameterType(parameterType, nullableAnnotation);
						var assigning = getAssigningExpression(refModifiers, parameterName);
						var memberNotNullAttribute = namedArgs.TryGetValueOrDefault<string>("MembersNotNull", out var memberNotNullExpr)
							&& (memberNotNullExpr?.Contains(':') ?? false)
							&& memberNotNullExpr.Split(':') is [var booleanExpr, var memberNamesExpr]
							&& booleanExpr.Trim().ToCamelCasing() is var finalBooleanStringValue and ("true" or "false")
							&& (string[])[.. from memberName in memberNamesExpr.Split(',') select memberName.Trim()] is var memberNamesArray and not []
							&& (from memberName in memberNamesArray select $"nameof({memberName})") is var nameOfExpressionList
							&& string.Join(", ", nameOfExpressionList) is var nameOfExpressions
							? $"""
							[global::System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute({finalBooleanStringValue}, {nameOfExpressions})]
									
							"""
							: string.Empty;

						propertyDeclarations.Add(
							$$"""
							/// <summary>
									{{docComments ?? $"/// The generated property declaration for parameter <c>{parameterName}</c>."}}
									/// </summary>
									[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(PrimaryConstructorMemberHandler).FullName}}", "{{Value}}")]
									[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
									{{memberNotNullAttribute}}{{accessibilityModifiers}}{{readonlyModifier}}{{refModifiers}}{{parameterTypeName}}{{targetMemberName}} { get;{{setter}} } = {{assigning}};
							"""
						);

						break;
					}
				}
			}

			var typeKindString = valuesGrouped[0].TypeSymbol.GetTypeKindModifier();
			var genericTypeParameters = valuesGrouped[0].TypeSymbol switch
			{
				{ IsGenericType: true, TypeParameters: var typeParameters }
					=> $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>",
				_ => string.Empty
			};

			types.Add(
				$$"""
				namespace {{namespaceName}}
				{
				#line 1 "{{valuesGrouped[0].Type}}"
					partial {{typeKindString}} {{valuesGrouped[0].Type}}{{genericTypeParameters}}
					{
						{{(fieldDeclarations.Count == 0 ? "// No field members." : string.Join("\r\n\r\n\t\t", fieldDeclarations))}}
				
				
						{{(propertyDeclarations.Count == 0 ? "// No property members." : string.Join("\r\n\r\n\t\t", propertyDeclarations))}}
					}
				#line default
				}
				"""
			);


			static string getAccessibilityModifiers(NamedArgs namedArgs, string @default)
				=> namedArgs.TryGetValueOrDefault<string>("Accessibility", out var a) && a is not null ? $"{a.Trim().ToLower()} " : @default;

			static string getReadOnlyModifier(NamedArgs namedArgs, ScopedKind scopedKind, RefKind refKind, TypeKind typeKind, bool isRefStruct, bool isReadOnly, bool isField, bool setterIsEmpty)
				=> (!namedArgs.TryGetValueOrDefault<bool>("IsImplicitlyReadOnly", out var a) || a) switch
				{
					true => (scopedKind, refKind, typeKind, isReadOnly, isRefStruct, isField, setterIsEmpty) switch
					{
						(0, RefKind.In or RefKind.RefReadOnlyParameter, TypeKind.Struct, false, true, _, true) => "readonly ",
						(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, false, true, _, true) => "readonly ",
						(_, _, TypeKind.Struct, _, _, true, true) => "readonly ",
						(_, _, TypeKind.Struct, false, _, _, true) => "readonly ",
						_ => string.Empty
					},
					_ => string.Empty
				};

			static string getRefModifiers(NamedArgs namedArgs, ScopedKind scopedKind, RefKind refKind, TypeKind typeKind, bool isRefStruct, bool isReadOnly, bool isField)
				=> (namedArgs.TryGetValueOrDefault<string>("RefKind", out var l) && l is not null ? $"{l} " : null)
				?? (scopedKind, refKind, typeKind, isReadOnly, isRefStruct, isField) switch
				{
					(0, RefKind.In or RefKind.RefReadOnlyParameter, TypeKind.Struct, false, true, _) => "ref readonly ",
					(0, RefKind.In or RefKind.RefReadOnlyParameter, TypeKind.Struct, true, true, _) => "ref readonly ",
					(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, false, true, _) => "ref ",
					(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, true, true, _) => "ref ",
					_ => null
				}
				?? string.Empty;

			static string getParameterType(ITypeSymbol parameterType, NullableAnnotation nullableAnnotation)
			{
				var formatOptions = SymbolDisplayFormat.FullyQualifiedFormat
					.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
				return $"{parameterType.ToDisplayString(formatOptions)} ";
			}

			static string getAssigningExpression(string refModifiers, string parameterName)
				=> refModifiers switch { not "" => $"ref {parameterName}", _ => parameterName };

			static string? getDocComments(string? comment)
				=> comment switch
				{
					null or "" => null,
					_ => string.Join(
						"\r\n",
						from line in comment.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries) select $"/// {line.Trim()}"
					)
				};
		}

		spc.AddSource(
			"PrimaryConstructorParameters.g.cs",
			$$"""
			{{Banner.AutoGenerated}}

			#nullable enable

			#pragma warning disable CS8500

			{{string.Join("\r\n\r\n", types)}}
			"""
		);
	}

	/// <inheritdoc/>
	public static CollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
		=> gasc switch
		{
			{
				TargetNode: ParameterSyntax { Parent.Parent: TypeDeclarationSyntax { Modifiers: var typeModifiers and not [] } },
				TargetSymbol: IParameterSymbol
				{
					Name: var parameterName,
					RefKind: var refKind,
					ScopedKind: var scopedKind,
					NullableAnnotation: var nullableAnnotation,
					Type: var parameterType,
					ContainingSymbol: IMethodSymbol
					{
						ContainingType:
						{
							ContainingNamespace: var @namespace,
							IsReadOnly: var isReadOnly,
							Name: var typeName,
							IsRecord: var isRecord,
							TypeKind: var typeKind
						} typeSymbol
					}
				} parameterSymbol,
				Attributes: { Length: 1 or 2 } attributesData
			}
			when typeModifiers.Any(SyntaxKind.PartialKeyword)
				=> new(
					parameterName,
					typeKind,
					refKind,
					scopedKind,
					nullableAnnotation,
					parameterType,
					typeSymbol,
					isReadOnly,
					@namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..],
					typeName,
					isRecord,
					[.. attributesData],
					// Gets the documentation comments.
					// Please note that Roslyn may contain a bug that primary constructor parameters don't contain any documentation comments,
					// so the result value of this variable may be string.Empty ("").
					parameterSymbol.GetDocumentationCommentXml(cancellationToken: cancellationToken)
				),
			_ => null
		};


	/// <summary>
	/// Indicates the data collected via <see cref="PrimaryConstructorMemberHandler"/>.
	/// </summary>
	/// <seealso cref="PrimaryConstructorMemberHandler"/>
	internal sealed record CollectedResult(
		string ParameterName,
		TypeKind TypeKind,
		RefKind RefKind,
		ScopedKind ScopedKind,
		NullableAnnotation NullableAnnotation,
		ITypeSymbol ParameterType,
		INamedTypeSymbol TypeSymbol,
		bool IsReadOnly,
		string Namesapce,
		string Type,
		bool IsRecord,
		AttributeData[] AttributesData,
		string? Comment
	);
}
