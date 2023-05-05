namespace Sudoku.Diagnostics.CodeGen;

using static CommonMethods;
using static global::CodeGen.Constants;
using static NullableAnnotation;
using static SpecialType;
using NamedArgs = ImmutableArray<KeyValuePair<string, TypedConstant>>;

/// <summary>
/// Represents a source generator type that runs multiple different usage of source output services on compiling code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public unsafe void Initialize(IncrementalGeneratorInitializationContext context)
	{
		//
		// Elementary generators
		//
		#region Elementary generators
		// Primary Constructors
		{
			const string name = "System.Diagnostics.CodeGen.PrimaryConstructorParameterAttribute";
			context.Register<PrimaryConstructorHandler, PrimaryConstructorCollectedResult>(name, &SyntaxNodeTypePredicate<ParameterSyntax>);
		}

		// Default Overridden
		{
			const string name = "System.Diagnostics.CodeGen.GeneratedOverridingMemberAttribute";
			context.Register<EqualsOverriddenHandler, EqualsOverriddenCollectedResult>(name, &IsPartialMethodPredicate);
			context.Register<GetHashCodeOveriddenHandler, GetHashCodeCollectedResult>(name, &IsPartialMethodPredicate);
			context.Register<ToStringOverriddenHandler, ToStringCollectedResult>(name, &IsPartialMethodPredicate);
		}

		// Instance Deconstruction Methods
		{
			const string name = "System.Diagnostics.CodeGen.DeconstructionMethodAttribute";
			context.Register<InstanceDeconstructionMethodHandler, InstanceDeconstructionMethodCollectedResult>(name, &IsPartialMethodPredicate);
		}
		#endregion

		//
		// Advanced generators
		//
		#region Advanced generators
		// StepSearcher Default Imports
		{
			context.Register<StepSearcherDefaultImportingHandler>("Sudoku.Analytics");
		}
		#endregion
	}
}

/// <summary>
/// Represents a file-local constraint for generators,
/// which can be used for <see cref="IncrementalGeneratorInitializationContext.SyntaxProvider"/>,
/// with <see cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>.
/// </summary>
/// <typeparam name="T">The type of the final data structure.</typeparam>
/// <seealso cref="IncrementalGeneratorInitializationContext.SyntaxProvider"/>
/// <seealso cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>
file interface IIncrementalGeneratorAttributeHandler<T> where T : notnull
{
	/// <summary>
	/// Transform the target result from the specified <see cref="SyntaxNode"/> and its semantic values.
	/// </summary>
	/// <param name="gasc">The context used for getting basic information for a <see cref="SyntaxNode"/>.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel generating.</param>
	/// <returns>The result. The value can be <see langword="null"/>.</returns>
	T? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken);

	/// <summary>
	/// Try to generate the source.
	/// </summary>
	/// <param name="spc">The context used for generating.</param>
	/// <param name="values">The values.</param>
	void Output(SourceProductionContext spc, ImmutableArray<T> values);
}

/// <summary>
/// Represents a file-local constraint for generators,
/// which can be used for <see cref="IncrementalGeneratorInitializationContext.CompilationProvider"/>.
/// </summary>
/// <seealso cref="IncrementalGeneratorInitializationContext.CompilationProvider"/>
file interface IIncrementalGeneratorCompilationHandler
{
	/// <summary>
	/// Try to generate the source.
	/// </summary>
	/// <param name="spc">The context used for generating.</param>
	/// <param name="compilation">
	/// The <see cref="Compilation"/> instance that provides the information for the calling project.
	/// </param>
	void Output(SourceProductionContext spc, Compilation compilation);
}

/// <summary>
/// The generator handler for primary constructor parameters.
/// </summary>
file sealed class PrimaryConstructorHandler : IIncrementalGeneratorAttributeHandler<PrimaryConstructorCollectedResult>
{
	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, ImmutableArray<PrimaryConstructorCollectedResult> values)
	{
		foreach (var (fileName, valuesGrouped) in
			from tuple in values
			group tuple by $"{tuple.Namesapce}.{tuple.Type}" into @group
			let fileName = $"{@group.Key}.g.{Shortcuts.PrimaryConstructorParameter}.cs"
			select (FileName: fileName, Values: @group.ToArray()))
		{
			var (fieldDeclarations, propertyDeclarations) = (new List<string>(), new List<string>());
			foreach (
				var (
					parameterName, typeKind, refKind, scopedKind, nullableAnnotation, parameterType, typeSymbol, isReadOnly,
					namespaceString, typeName, isRecord, attributesData, comment
				) in valuesGrouped
			)
			{
				switch (attributesData)
				{
					case [{ ConstructorArguments: [{ Value: LocalMemberKinds.Field }], NamedArguments: var namedArgs }]:
					{
						var targetMemberName = getTargetMemberName(namedArgs, parameterName, "_<@");
						var accessibilityModifiers = getAccessibilityModifiers(namedArgs, "private ");
						var readonlyModifier = getReadOnlyModifier(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, true);
						var refModifiers = getRefModifiers(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, true);
						var docComments = getDocComments(comment);
						var parameterTypeName = getParameterType(parameterType, nullableAnnotation);
						var assigning = getAssigningExpression(refModifiers, parameterName);
						var pragmaWarningDisable = assigning.StartsWith("ref")
							? """
							#pragma warning disable CS9094
									
							"""
							: "\t";
						var pragmaWarningRestor = assigning.StartsWith("ref")
							? """

							#pragma warning restore CS9094
							"""
							: string.Empty;
						fieldDeclarations.Add(
							$"""
							/// <summary>
								{docComments ?? $"/// The generated field declaration for parameter <c>{parameterName}</c>."}
								/// </summary>
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							{pragmaWarningDisable}{accessibilityModifiers}{readonlyModifier}{refModifiers}{parameterTypeName}{targetMemberName} = {assigning};{pragmaWarningRestor}
							"""
						);

						break;
					}
					case [{ ConstructorArguments: [{ Value: LocalMemberKinds.Property }], NamedArguments: var namedArgs }]:
					{
						var targetMemberName = getTargetMemberName(namedArgs, parameterName, ">@");
						var accessibilityModifiers = getAccessibilityModifiers(namedArgs, "public ");
						var readonlyModifier = getReadOnlyModifier(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, false);
						var refModifiers = getRefModifiers(namedArgs, scopedKind, refKind, typeKind, typeSymbol.IsRefLikeType, isReadOnly, false);
						var docComments = getDocComments(comment);
						var parameterTypeString = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
						var parameterTypeName = getParameterType(parameterType, nullableAnnotation);
						var assigning = getAssigningExpression(refModifiers, parameterName);
						var memberNotNullAttribute = namedArgs.TryGetValueOrDefault<string>("MembersNotNull", out var memberNotNullExpr)
							&& (memberNotNullExpr?.Contains(':') ?? false)
							&& memberNotNullExpr.Split(':') is [var booleanExpr, var memberNamesExpr]
							&& booleanExpr.Trim().ToCamelCasing() is var finalBooleanStringValue and ("true" or "false")
							&& (from memberName in memberNamesExpr.Split(',') select memberName.Trim()).ToArray() is var memberNamesArray and not []
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
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								{{memberNotNullAttribute}}{{accessibilityModifiers}}{{readonlyModifier}}{{refModifiers}}{{parameterTypeName}}{{targetMemberName}} { get; } = {{assigning}};
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
			spc.AddSource(
				fileName,
				$$"""
				// <auto-generated/>

				#nullable enable
				namespace {{valuesGrouped[0].Namesapce}};

				partial {{typeKindString}} {{valuesGrouped[0].Type}}{{genericTypeParameters}}
				{
					{{(fieldDeclarations.Count == 0 ? "// No field members." : string.Join("\r\n\r\n\t", fieldDeclarations))}}


					{{(propertyDeclarations.Count == 0 ? "// No property members." : string.Join("\r\n\r\n\t", propertyDeclarations))}}
				}
				"""
			);


			static string getTargetMemberName(NamedArgs namedArgs, string parameterName, string defaultPattern)
				=> namedArgs.TryGetValueOrDefault<string>("GeneratedMemberName", out var customizedFieldName)
				&& customizedFieldName is not null
					? customizedFieldName
					: namedArgs.TryGetValueOrDefault<string>("NamingRule", out var namingRule) && namingRule is not null
						? namingRule.InternalHandle(parameterName)
						: defaultPattern.InternalHandle(parameterName);

			static string getAccessibilityModifiers(NamedArgs namedArgs, string @default)
				=> namedArgs.TryGetValueOrDefault<string>("Accessibility", out var a) && a is not null ? $"{a.Trim().ToLower()} " : @default;

			static string getReadOnlyModifier(NamedArgs namedArgs, ScopedKind scopedKind, RefKind refKind, TypeKind typeKind, bool isRefStruct, bool isReadOnly, bool isField)
				=> (scopedKind, refKind, typeKind, isReadOnly, isRefStruct, isField) switch
				{
					(0, RefKind.In, TypeKind.Struct, false, true, _) => "readonly ",
					(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, false, true, _) => "readonly ",
					(_, _, TypeKind.Struct, _, _, true) => "readonly ",
					(_, _, TypeKind.Struct, false, _, _) => "readonly ",
					_ => string.Empty
				};

			static string getRefModifiers(NamedArgs namedArgs, ScopedKind scopedKind, RefKind refKind, TypeKind typeKind, bool isRefStruct, bool isReadOnly, bool isField)
				=> (namedArgs.TryGetValueOrDefault<string>("RefKind", out var l) && l is not null ? $"{l} " : null)
				?? (scopedKind, refKind, typeKind, isReadOnly, isRefStruct, isField) switch
				{
					(0, RefKind.In, TypeKind.Struct, false, true, _) => "ref readonly ",
					(0, RefKind.In, TypeKind.Struct, true, true, _) => "ref readonly ",
					(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, false, true, _) => "ref ",
					(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, true, true, _) => "ref ",
					_ => null
				}
				?? string.Empty;

			static string getParameterType(ITypeSymbol parameterType, NullableAnnotation nullableAnnotation)
				=> parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) is var r
				&& parameterType.TypeKind != TypeKind.Struct && nullableAnnotation == Annotated
					? $"{r}? "
					: $"{r} ";

			static string getAssigningExpression(string refModifiers, string parameterName)
				=> refModifiers switch { not "" => $"ref {parameterName}", _ => parameterName };

			static string? getDocComments(string? comment)
				=> comment switch
				{
					null or "" => null,
					_ => string.Join(
						Environment.NewLine,
						from line in comment.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) select $"/// {line.Trim()}"
					)
				};
		}
	}

	/// <inheritdoc/>
	public PrimaryConstructorCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
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
					attributesData.ToArray(),
					// Gets the documentation comments.
					// Please note that Roslyn may contain a bug that primary constructor parameters don't contain any documentation comments,
					// so the result value of this variable may be string.Empty ("").
					parameterSymbol.GetDocumentationCommentXml(cancellationToken: cancellationToken)
				),
			_ => null
		};
}

/// <summary>
/// The generator handler for default overridden of <c>Equals</c>.
/// </summary>
file sealed class EqualsOverriddenHandler : IIncrementalGeneratorAttributeHandler<EqualsOverriddenCollectedResult>
{
	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, ImmutableArray<EqualsOverriddenCollectedResult> values)
	{
		var codeSnippets = new List<string>();
		foreach (var value in values)
		{
			if (value is not (var mode, var modifiers, { Name: var typeName, ContainingNamespace: var @namespace } type, var paramName))
			{
				continue;
			}

			var (_, _, _, _, genericParamList, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);
			var extraAttributeStr = mode switch
			{
				0 => """
				[global::System.Obsolete(global::System.Runtime.Messages.RefStructDefaultImplementationMessage.OverriddenEqualsMethod, false)]
						[global::System.Diagnostics.DebuggerHiddenAttribute]
						[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
							
				""",
				_ => string.Empty
			};
			var targetExpression = mode switch
			{
				0 => "false",
				1 => $"{paramName} is {type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} comparer && Equals(comparer)",
				2 => $"Equals({paramName} as {type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})"
			};
			var attributeStr = mode switch
			{
				0 => string.Empty,
				1 or 2 => "[global::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] "
			};

			var namespaceStr = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
			codeSnippets.Add(
				$$"""
				namespace {{namespaceStr}}
				{
					partial {{type.GetTypeKindModifier()}} {{typeName}}{{genericParamList}}
					{
						/// <inheritdoc cref="object.Equals(object?)"/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(Generator).FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						{{extraAttributeStr}}{{modifiers}} bool Equals({{attributeStr}}object? {{paramName}})
							=> {{targetExpression}};
					}
				}
				"""
			);
		}

		spc.AddSource(
			$"DefaultOverrides.g.{Shortcuts.GeneratedOverriddenMemberEquals}.cs",
			$"""
			// <auto-generated/>

			#nullable enable

			{string.Join("\r\n\r\n", codeSnippets)}
			"""
		);
	}

	/// <inheritdoc/>
	public EqualsOverriddenCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawMode }] }],
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers },
				TargetSymbol: IMethodSymbol
				{
					OverriddenMethod: var overriddenMethod,
					ContainingType: { } type,
					Name: nameof(Equals),
					IsOverride: true,
					IsStatic: false,
					ReturnType.SpecialType: System_Boolean,
					IsGenericMethod: false,
					Parameters: [{ Name: var parameterName, Type: { SpecialType: System_Object, NullableAnnotation: Annotated } }]
				} method
			})
		{
			return null;
		}

		// Check whether the method is overridden from object.Equals(object?).
		var rootMethod = overriddenMethod;
		var currentMethod = method;
		for (; rootMethod is not null; rootMethod = rootMethod.OverriddenMethod, currentMethod = currentMethod!.OverriddenMethod) ;
		if (currentMethod!.ContainingType.SpecialType is not (System_Object or System_ValueType))
		{
			return null;
		}

		if ((rawMode, type) switch
		{
			(0, { TypeKind: TypeKind.Struct, IsRefLikeType: true }) => false,
			(1, _) => false,
			(2, { TypeKind: TypeKind.Class }) => false,
			_ => true
		})
		{
			return null;
		}

		return new(rawMode, modifiers, type, parameterName);
	}
}

/// <summary>
/// The generator handler for default overridden of <c>GetHashCode</c>.
/// </summary>
file sealed class GetHashCodeOveriddenHandler : IIncrementalGeneratorAttributeHandler<GetHashCodeCollectedResult>
{
	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, ImmutableArray<GetHashCodeCollectedResult> values)
	{
		var codeSnippets = new List<string>();
		foreach (var value in values)
		{
			if (value is not (var mode, var modifiers, { Name: var typeName, ContainingNamespace: var @namespace } type, var rawMemberNames))
			{
				continue;
			}

			var (_, _, _, _, genericParamList, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);
			var needCast = mode switch
			{
				1 when rawMemberNames.First() is var name
					=> (from m in type.GetMembers() where m.Name == name select m).FirstOrDefault() switch
					{
						IFieldSymbol field => field is not { Type.SpecialType: System_Int32, RefKind: RefKind.None },
						IPropertySymbol property => property is not { Type.SpecialType: System_Int32, RefKind: RefKind.None },
						IMethodSymbol { Parameters: [] } method => method is not { ReturnType.SpecialType: System_Int32, RefKind: RefKind.None },
						_ => null
					},
				_ => default(bool?)
			};

			var extraAttributeStr = mode switch
			{
				0 => """
				[global::System.Obsolete(global::System.Runtime.Messages.RefStructDefaultImplementationMessage.OverriddenGetHashCodeMethod, false)]
						[global::System.Diagnostics.DebuggerHiddenAttribute]
						[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
							
				""",
				_ => string.Empty
			};
			var targetExpression = (mode, rawMemberNames.ToArray(), needCast) switch
			{
				(0, [], _)
					=> $"\t=> throw new global::System.NotSupportedException(global::System.Runtime.Messages.RefStructDefaultImplementationMessage.OverriddenGetHashCodeMethod);",
				(1, [var memberName], true or null)
					=> $"\t=> (int){memberName};",
				(1, [var memberName], _)
					=> $"\t=> {memberName};",
				(2, { Length: <= 8 } memberNames, _) when string.Join(", ", from e in memberNames select e) is var a
					=> $"\t=> global::System.HashCode.Combine({a});",
				(2, { Length: > 8 } memberNames, _) when string.Join("\r\n\t\t\t", from e in memberNames select $"result.Add({e});") is var a
					=> $$"""
					{
								var result = new global::System.HashCode();
								{{a}}
								return result.ToHashCode();
							}
					""",
			};

			var namespaceStr = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
			codeSnippets.Add(
				$$"""
				namespace {{namespaceStr}}
				{
					partial {{type.GetTypeKindModifier()}} {{typeName}}{{genericParamList}}
					{
						/// <inheritdoc cref="object.GetHashCode"/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(Generator).FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						{{extraAttributeStr}}{{modifiers}} int GetHashCode()
						{{targetExpression}}
					}
				}
				"""
			);
		}

		spc.AddSource(
			$"DefaultOverrides.g.{Shortcuts.GeneratedOverriddenMemberGetHashCode}.cs",
			$"""
			// <auto-generated/>

			#nullable enable

			{string.Join("\r\n\r\n", codeSnippets)}
			"""
		);
	}

	/// <inheritdoc/>
	public GetHashCodeCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawMode }, { Values: var extraArguments }] }],
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers },
				TargetSymbol: IMethodSymbol
				{
					OverriddenMethod: var overriddenMethod,
					ContainingType: { } type,
					Name: nameof(GetHashCode),
					IsOverride: true,
					IsStatic: false,
					ReturnType.SpecialType: System_Int32,
					IsGenericMethod: false,
					Parameters: []
				} method
			})
		{
			return null;
		}

		// Check whether the method is overridden from object.GetHashCode.
		var rootMethod = overriddenMethod;
		var currentMethod = method;
		for (; rootMethod is not null; rootMethod = rootMethod.OverriddenMethod, currentMethod = currentMethod!.OverriddenMethod) ;
		if (currentMethod!.ContainingType.SpecialType is not (System_Object or System_ValueType))
		{
			return null;
		}

		if ((rawMode, type) switch { (0, { TypeKind: TypeKind.Struct, IsRefLikeType: true }) => false, (1 or 2, _) => false, _ => true })
		{
			return null;
		}

		return new(rawMode, modifiers, type, from extraArgument in extraArguments select (string)extraArgument.Value!);
	}
}

/// <summary>
/// The generator handler for default overridden of <c>ToString</c>.
/// </summary>
file sealed class ToStringOverriddenHandler : IIncrementalGeneratorAttributeHandler<ToStringCollectedResult>
{
	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, ImmutableArray<ToStringCollectedResult> values)
	{
		var codeSnippets = new List<string>();
		foreach (var (mode, modifiers, type, attributeType, rawMemberNames) in values)
		{
			if (type is not { Name: var typeName, ContainingNamespace: var @namespace })
			{
				continue;
			}

			var (_, _, _, _, genericParamList, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			var needCast = mode switch
			{
				0 => (
					from IMethodSymbol method in type.GetAllMembers().OfType<IMethodSymbol>().Distinct(SymbolEqualityComparer.Default)
					where method is { Name: nameof(ToString), Parameters: [{ Type.IsReferenceType: true }] }
					select method
				).Take(2).Count() == 2,
				_ => default(bool?)
			};

			var targetExpression = (mode, rawMemberNames.ToArray(), needCast) switch
			{
				(0, [], true) => $"\t=> ToString(default(string));",
				(0, [], _) => $"\t=> ToString(null);",
				(1, [var memberName], _) => $"\t=> {memberName};",
				(2, var memberNames, _) when argStr(memberNames) is var a => $$$""""{{{"\t"}}}=> $$"""{{nameof({{{typeName}}})}} { {{{a}}} }""";"""",
			};

			var namespaceStr = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
			codeSnippets.Add(
				$$"""
				namespace {{namespaceStr}}
				{
					partial {{type.GetTypeKindModifier()}} {{typeName}}{{genericParamList}}
					{
						/// <inheritdoc cref="object.ToString"/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(Generator).FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						{{modifiers}} string ToString()
						{{targetExpression}}
					}
				}
				"""
			);


			string argStr(string[] memberNames)
			{
				bool attributePredicate(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType);
				return string.Join(
					", ",
					from memberName in memberNames
					let targetMember = (
						from internalMember in type.GetAllMembers()
						where internalMember is IFieldSymbol or IPropertySymbol
						let internalMemberName = internalMember.Name
						where internalMemberName == memberName
						select internalMember
					).FirstOrDefault()
					let foundAttribute = targetMember?.GetAttributes().FirstOrDefault(attributePredicate)
					let projectedMemberName = foundAttribute switch { { ConstructorArguments: [{ Value: string value }] } => value, _ => null }
					select $$$""""{{{(projectedMemberName is null ? $$$"""{{nameof({{{memberName}}})}}""" : projectedMemberName)}}} = {{{{{memberName}}}}}""""
				);
			}
		}

		spc.AddSource(
			$"DefaultOverrides.g.{Shortcuts.GeneratedOverriddenMemberToString}.cs",
			$"""
			// <auto-generated/>

			#nullable enable

			{string.Join("\r\n\r\n", codeSnippets)}
			"""
		);
	}

	/// <inheritdoc/>
	public ToStringCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawMode }, { Values: var extraArguments }] }],
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers },
				TargetSymbol: IMethodSymbol
				{
					OverriddenMethod: var overriddenMethod,
					ContainingType: { } type,
					Name: nameof(ToString),
					IsOverride: true,
					IsStatic: false,
					ReturnType.SpecialType: System_String,
					IsGenericMethod: false,
					Parameters: []
				} method,
				SemanticModel.Compilation: var compilation
			})
		{
			return null;
		}

		// Check whether the method is overridden from object.ToString.
		var rootMethod = overriddenMethod;
		var currentMethod = method;
		for (; rootMethod is not null; rootMethod = rootMethod.OverriddenMethod, currentMethod = currentMethod!.OverriddenMethod) ;
		if (currentMethod!.ContainingType.SpecialType is not (System_Object or System_ValueType))
		{
			return null;
		}

		var attributeType = compilation.GetTypeByMetadataName("System.Diagnostics.CodeGen.GeneratedDisplayNameAttribute");
		if (attributeType is null)
		{
			return null;
		}

		return new(rawMode, modifiers, type, attributeType, from extraArgument in extraArguments select (string)extraArgument.Value!);
	}
}

/// <summary>
/// The generator handler for instance deconstruction methods.
/// </summary>
file sealed class InstanceDeconstructionMethodHandler : IIncrementalGeneratorAttributeHandler<InstanceDeconstructionMethodCollectedResult>
{
	private const string DeconstructionMethodArgumentAttributeName = "System.Diagnostics.CodeGen.DeconstructionMethodArgumentAttribute";


	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, ImmutableArray<InstanceDeconstructionMethodCollectedResult> values)
	{
		static INamedTypeSymbol containingTypeSelector(InstanceDeconstructionMethodCollectedResult data) => data.ContainingType;
		foreach (var group in values.GroupBy(containingTypeSelector, (IEqualityComparer<INamedTypeSymbol>)SymbolEqualityComparer.Default))
		{
			var containingType = group.Key;
			var typeName = containingType.Name;
			var @namespace = containingType.ContainingNamespace;
			var typeParameters = containingType.TypeParameters;

			var namespaceStr = @namespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) switch
			{
				{ } s => $"namespace {s["global::".Length..]};\r\n\r\n",
				_ => string.Empty
			};

			var typeParametersStr = typeParameters switch
			{
				[] => string.Empty,
				_ => $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>"
			};

			var codeSnippets = new List<string>();
			foreach (var element in group)
			{
				if (element is not
					{
						Method: { DeclaredAccessibility: var methodAccessibility } method,
						Parameters: var parameters,
						Modifiers: var modifiers,
						AttributeType: var attributeType,
						AssemblyName: var assemblyName
					})
				{
					continue;
				}

				var parameterNameData = new List<(string Parameter, string Member)>();
				foreach (var parameter in parameters)
				{
					var name = parameter.Name;
					bool predicate(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType);
					parameterNameData.Add(
						parameter.GetAttributes().FirstOrDefault(predicate) switch
						{
							{ ConstructorArguments: [{ Value: string s }] } => (name, s),
							_ => (name, localToPascalCasing(name))
						}
					);
				}

				var assignmentsCode = string.Join("\r\n\t\t", from t in parameterNameData select $"{t.Parameter} = {t.Member};");
				var argsStr = string.Join(
					", ",
					from parameter in parameters
					let parameterType = parameter.Type
					let name = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
					let annotation = parameterType is { NullableAnnotation: Annotated, IsReferenceType: true } ? "?" : string.Empty
					select $"out {name}{annotation} {parameter.Name}"
				);

				var includingReferenceLevel = assemblyName.StartsWith("SudokuStudio") ? "../../../" : "../../";
				codeSnippets.Add(
					$$"""
					/// <include file="{{includingReferenceLevel}}global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
						[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						{{modifiers}} void Deconstruct({{argsStr}})
						{
							{{assignmentsCode}}
						}
					"""
				);
			}

			spc.AddSource(
				$"{containingType.ToFileName()}.g.{Shortcuts.GeneratedDeconstruction}.cs",
				$$"""
				// <auto-generated/>
				
				#nullable enable
				
				{{namespaceStr}}partial {{containingType.GetTypeKindModifier()}} {{typeName}}{{typeParametersStr}}
				{
					{{string.Join("\r\n\r\n\t", codeSnippets)}}
				}
				"""
			);
		}

		static string localToPascalCasing(string name)
			=> name switch
			{
				['_', .. var slice] => localToPascalCasing(slice),
				[>= 'A' and <= 'Z', ..] => name,
				[var ch and >= 'a' and <= 'z', .. var slice] => $"{char.ToUpper(ch)}{slice}",
				_ => name
			};
	}

	/// <inheritdoc/>
	public InstanceDeconstructionMethodCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
		=> gasc switch
		{
			{
				Attributes.Length: 1,
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers } node,
				TargetSymbol: IMethodSymbol
				{
					Name: "Deconstruct",
					TypeParameters: [],
					Parameters: var parameters and not [],
					IsStatic: false,
					ReturnsVoid: true,
					ContainingType: { ContainingType: null, IsFileLocal: false } type
				} symbol,
				SemanticModel.Compilation: { AssemblyName: { } assemblyName } compilation
			}
			when parameters.AllOutParameters() => compilation.GetTypeByMetadataName(DeconstructionMethodArgumentAttributeName) switch
			{
				{ } argumentAttributeType => new(type, symbol, parameters, modifiers, argumentAttributeType, assemblyName),
				_ => null
			},
			_ => null
		};
}

/// <summary>
/// The generator handler for step searcher default importing.
/// </summary>
file sealed class StepSearcherDefaultImportingHandler : IIncrementalGeneratorCompilationHandler
{
	private const string AreasPropertyName = "Areas";

	private const string StepSearcherTypeName = "Sudoku.Analytics.StepSearcher";

	private const string StepSearcherRunningAreaTypeName = "Sudoku.Analytics.Metadata.StepSearcherRunningArea";

	private const string StepSearcherLevelTypeName = "Sudoku.Analytics.Metadata.StepSearcherLevel";

	private const string StepSearcherImportAttributeName = "global::Sudoku.Analytics.Metadata.StepSearcherImportAttribute<>";

	private const string PolymorphismAttributeName = "Sudoku.Analytics.Metadata.PolymorphismAttribute";


	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, Compilation compilation)
	{
		// Checks whether the assembly has marked any attributes.
		if (compilation.Assembly.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
		{
			return;
		}

		var stepSearcherBaseType = compilation.GetTypeByMetadataName(StepSearcherTypeName);
		if (stepSearcherBaseType is null)
		{
			return;
		}

		var runningAreaTypeSymbol = compilation.GetTypeByMetadataName(StepSearcherRunningAreaTypeName)!;
		var levelTypeSymbol = compilation.GetTypeByMetadataName(StepSearcherLevelTypeName)!;
		var runningAreasFields = new Dictionary<byte, string>();
		var levelFields = new Dictionary<byte, string>();
		foreach (var fieldSymbol in runningAreaTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			if (fieldSymbol is { ConstantValue: byte value, Name: var fieldName })
			{
				runningAreasFields.Add(value, fieldName);
			}
		}
		foreach (var fieldSymbol in levelTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			if (fieldSymbol is { ConstantValue: byte value, Name: var fieldName })
			{
				levelFields.Add(value, fieldName);
			}
		}

		// Gather the valid attributes data.
		var foundAttributesData = new List<StepSearcherDefaultImportingCollectedResult>();
		const string comma = ", ";
		var priorityValue = 0;
		foreach (var attributeData in attributesData)
		{
			// Check validity.
#pragma warning disable format
			if (attributeData is not
				{
					AttributeClass:
					{
						IsGenericType: true,
						TypeArguments:
						[
							INamedTypeSymbol
							{
								IsRecord: false,
								ContainingNamespace: var containingNamespace,
								Name: var stepSearcherName,
								BaseType: { } baseType
							} stepSearcherType
						]
					} attributeClassSymbol,
					ConstructorArguments: [{ Type.TypeKind: TypeKind.Enum, Value: byte dl }],
					NamedArguments: var namedArguments
				})
#pragma warning restore format
			{
				continue;
			}

			// Checks whether the type is valid.
			var unboundAttributeTypeSymbol = attributeClassSymbol.ConstructUnboundGenericType();
			if (unboundAttributeTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != StepSearcherImportAttributeName)
			{
				continue;
			}

			// Check whether the step searcher can be used for deriving.
			var polymorphismAttributeType = compilation.GetTypeByMetadataName(PolymorphismAttributeName)!;
			var isPolymorphism = stepSearcherType.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, polymorphismAttributeType));

			// Adds the necessary info into the collection.
			foundAttributesData.Add(new(containingNamespace, baseType, priorityValue++, dl, stepSearcherName, namedArguments, isPolymorphism));
		}

		// Iterate on each valid attribute data, and checks the inner value to be used by the source generator to output.
		var generatedCodeSnippets = new List<string>();
		var namespaceUsed = foundAttributesData[0].Namespace;
		foreach (var (_, baseType, priority, level, name, namedArguments, isPolymorphism) in foundAttributesData)
		{
			// Checks whether the attribute has configured any extra options.
			var nullableRunningArea = default(byte?);
			if (namedArguments is not [])
			{
				foreach (var (k, v) in namedArguments)
				{
					if (k == AreasPropertyName && v is { Value: byte value })
					{
						nullableRunningArea = value;
					}
				}
			}

			// Gather the extra options on step searcher.
			var levelStr = createLevelExpression(level, levelFields);
			var runningAreaStr = nullableRunningArea switch
			{
				{ } runningArea => createRunningAreasExpression(runningArea, runningAreasFields),
				_ => null
			};

			var sb = new StringBuilder().Append(levelStr);
			_ = runningAreaStr is not null ? sb.Append(comma).Append(runningAreaStr) : default;

			// Output the generated code.
			var baseTypeFullName = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			generatedCodeSnippets.Add(
				isPolymorphism
					? $$"""
					partial class {{name}} : {{baseTypeFullName}}
					{
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().Name}}", "{{VersionValue}}")]
						public {{name}}() : base({{priority}}, {{levelStr}}{{(runningAreaStr is not null ? $", {runningAreaStr}" : string.Empty)}})
						{
						}

						/// <param name="priority">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, global::Sudoku.Analytics.Metadata.StepSearcherLevel, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='priority']"/>
						/// </param>
						/// <param name="level">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, global::Sudoku.Analytics.Metadata.StepSearcherLevel, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='level']"/>
						/// </param>
						/// <param name="runningArea">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, global::Sudoku.Analytics.Metadata.StepSearcherLevel, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='runningArea']"/>
						/// </param>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().Name}}", "{{VersionValue}}")]
						public {{name}}(
							int priority,
							global::Sudoku.Analytics.Metadata.StepSearcherLevel level,
							global::Sudoku.Analytics.Metadata.StepSearcherRunningArea runningArea = global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.Searching | global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.Gathering
						) : base(priority, level, runningArea)
						{
						}
					}
					"""
					: $"partial class {name}() : {baseTypeFullName}({priority}, {sb});"
			);
		}

		spc.AddSource(
			$"StepSearcherImports.g.{Shortcuts.StepSearcherImports}.cs",
			$$"""
			// <auto-generated/>

			#pragma warning disable CS1591
			#nullable enable
			namespace {{namespaceUsed.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..]}};
			
			{{string.Join($"{Environment.NewLine}{Environment.NewLine}", generatedCodeSnippets)}}
			"""
		);


		static string createRunningAreasExpression(byte field, IDictionary<byte, string> runningAreasFields)
		{
			var l = (int)field;
			if (l == 0)
			{
				return "0";
			}

			var targetList = new List<string>();
			for (var (temp, i) = (l, 0); temp != 0; temp >>= 1, i++)
			{
				if ((temp & 1) != 0)
				{
					targetList.Add($"global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.{runningAreasFields[(byte)(1 << i)]}");
				}
			}

			return string.Join(" | ", targetList);
		}

		static string createLevelExpression(byte field, IDictionary<byte, string> levelFields)
		{
			if (field == 0)
			{
				return "0";
			}

			foreach (var (v, n) in levelFields)
			{
				if (v == field)
				{
					return $"global::Sudoku.Analytics.Metadata.StepSearcherLevel.{n}";
				}
			}

			return string.Empty;
		}
	}
}

/// <summary>
/// Indicates the data collected via <see cref="PrimaryConstructorHandler"/>.
/// </summary>
/// <seealso cref="PrimaryConstructorHandler"/>
file sealed record PrimaryConstructorCollectedResult(
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

/// <summary>
/// Indicates the data collected via <see cref="EqualsOverriddenHandler"/>
/// </summary>
/// <seealso cref="EqualsOverriddenHandler"/>
file sealed record EqualsOverriddenCollectedResult(
	int GeneratedMode,
	SyntaxTokenList MethodModifiers,
	INamedTypeSymbol Type,
	string ParameterName
);

/// <summary>
/// Indicates the data collected via <see cref="GetHashCodeOveriddenHandler"/>
/// </summary>
/// <seealso cref="GetHashCodeOveriddenHandler"/>
file sealed record GetHashCodeCollectedResult(
	int GeneratedMode,
	SyntaxTokenList MethodModifiers,
	INamedTypeSymbol Type,
	IEnumerable<string> ExpressionValueNames
);

/// <summary>
/// Indicates the data collected via <see cref="ToStringOverriddenHandler"/>
/// </summary>
/// <seealso cref="ToStringOverriddenHandler"/>
file sealed record ToStringCollectedResult(
	int GeneratedMode,
	SyntaxTokenList MethodModifiers,
	INamedTypeSymbol Type,
	INamedTypeSymbol SpecialAttributeType,
	IEnumerable<string> ExpressionValueNames
);

/// <summary>
/// Indicates the data collected via <see cref="InstanceDeconstructionMethodHandler"/>
/// </summary>
/// <seealso cref="InstanceDeconstructionMethodHandler"/>
file sealed record InstanceDeconstructionMethodCollectedResult(
	INamedTypeSymbol ContainingType,
	IMethodSymbol Method,
	ImmutableArray<IParameterSymbol> Parameters,
	SyntaxTokenList Modifiers,
	INamedTypeSymbol AttributeType,
	string AssemblyName
);

/// <summary>
/// Indicates the data collected via <see cref="StepSearcherDefaultImportingHandler"/>.
/// </summary>
/// <seealso cref="StepSearcherDefaultImportingHandler"/>
file sealed record StepSearcherDefaultImportingCollectedResult(
	INamespaceSymbol Namespace,
	INamedTypeSymbol BaseType,
	int PriorityValue,
	byte DifficultyLevel,
	string TypeName,
	NamedArgs NamedArguments,
	bool IsPolymorphism
);

/// <summary>
/// The member kind names.
/// </summary>
file static class LocalMemberKinds
{
	/// <summary>
	/// Indicates the generated member kind is fields.
	/// </summary>
	public const string Field = nameof(Field);

	/// <summary>
	/// Indicates the generated member kind is properties.
	/// </summary>
	public const string Property = nameof(Property);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Registers for a new generator function via attribute checking.
	/// </summary>
	/// <typeparam name="THandler">
	/// The type of the target handler. The handler type must implement <see cref="IIncrementalGeneratorAttributeHandler{T}"/>,
	/// and contain a parameterless constructor.
	/// </typeparam>
	/// <typeparam name="TCollectedResult">
	/// The type of the collected result. The type must be as a generic type argument of <typeparamref name="THandler"/>.
	/// </typeparam>
	/// <param name="this">The context.</param>
	/// <param name="attributeName">
	/// The attribute name. The value must be full name of the attribute, including its namespace, beginning with root-level one.
	/// </param>
	/// <param name="nodeFilter">The node filter method.</param>
	/// <seealso cref="IIncrementalGeneratorAttributeHandler{T}"/>
	public static unsafe void Register<THandler, TCollectedResult>(
		this scoped ref IncrementalGeneratorInitializationContext @this,
		string attributeName,
		delegate*<SyntaxNode, CancellationToken, bool> nodeFilter
	)
		where THandler : IIncrementalGeneratorAttributeHandler<TCollectedResult>, new()
		where TCollectedResult : class
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, (node, cancellationToken) => nodeFilter(node, cancellationToken), inst.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			inst.Output
		);
	}

	/// <summary>
	/// Registers for a new generator function via compilation.
	/// </summary>
	/// <typeparam name="THandler">The handler.</typeparam>
	/// <param name="this">The context.</param>
	/// <param name="projectName">The full name of the project that can filter compilation projects.</param>
	public static unsafe void Register<THandler>(this scoped ref IncrementalGeneratorInitializationContext @this, string projectName)
		where THandler : IIncrementalGeneratorCompilationHandler, new()
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.CompilationProvider,
			(spc, c) => { if (c.AssemblyName == projectName) { inst.Output(spc, c); } }
		);
	}

	/// <summary>
	/// Determines whether all parameters are <see langword="out"/> ones.
	/// </summary>
	/// <param name="this">A list of parameters.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool AllOutParameters(this ImmutableArray<IParameterSymbol> @this)
		=> @this.All(static parameter => parameter.RefKind == RefKind.Out);

	/// <summary>
	/// Internal handle the naming rule, converting it into a valid identifier via specified parameter name.
	/// </summary>
	/// <param name="this">The naming rule.</param>
	/// <param name="parameterName">The parameter name.</param>
	/// <returns>The final identifier.</returns>
	public static string InternalHandle(this string @this, string parameterName)
		=> @this
			.Replace("<@", parameterName.ToCamelCasing())
			.Replace(">@", parameterName.ToPascalCasing())
			.Replace("@", parameterName);

	/// <summary>
	/// Try to convert the specified identifier into pascal casing.
	/// </summary>
	public static string ToPascalCasing(this string @this) => $"{char.ToUpper(@this[0])}{@this[1..]}";

	/// <summary>
	/// Try to convert the specified identifier into camel casing.
	/// </summary>
	public static string ToCamelCasing(this string @this) => $"{char.ToLower(@this[0])}{@this[1..]}";
}

/// <summary>
/// Represents a set of methods that can be used by the types in this file.
/// </summary>
file static class CommonMethods
{
	/// <summary>
	/// Determine whether the specified <see cref="SyntaxNode"/> is of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The possible type of the node.</typeparam>
	/// <param name="node">Indicates the target node.</param>
	/// <param name="_"/>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool SyntaxNodeTypePredicate<T>(SyntaxNode node, CancellationToken _) where T : SyntaxNode => node is T;

	/// <summary>
	/// Determine whether the specified <see cref="SyntaxNode"/> is a <see cref="MethodDeclarationSyntax"/>,
	/// and contains <see langword="partial"/> modifier.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <param name="_"/>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsPartialMethodPredicate(SyntaxNode node, CancellationToken _)
		=> node is MethodDeclarationSyntax { Modifiers: var m } && m.Any(SyntaxKind.PartialKeyword);

	/// <summary>
	/// Determine whether the value is not <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool NotNullPredicate<T>(T value) => value is not null;

	/// <summary>
	/// Try to get the internal value without nullability checking.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value with <c>?</c> token being annotated, but not <see langword="null"/> currently.</param>
	/// <param name="_"/>
	/// <returns>The value.</returns>
	public static T NotNullSelector<T>(T? value, CancellationToken _) where T : class => value!;

	/// <summary>
	/// Try to get the internal value without nullability checking.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value with <c>?</c> token being annotated, but not <see langword="null"/> currently.</param>
	/// <param name="_"/>
	/// <returns>The value.</returns>
	public static T NotNullSelector<T>(T? value, CancellationToken _) where T : struct => value!.Value;
}
