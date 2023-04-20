namespace Sudoku.Diagnostics.CodeGen.Generators;

using GatheredData = (string, TypeKind TypeKind, RefKind, ScopedKind, NullableAnnotation, ITypeSymbol, INamedTypeSymbol TypeSymbol, bool, string Namesapce, string Type, bool IsRecord, AttributeData[], string?);
using NamedArgs = ImmutableArray<KeyValuePair<string, TypedConstant>>;

/// <summary>
/// Represents a source generator that generates the source code for auto-binding logic for data members with primary constructor parameters
/// in non-<see langword="record"/> types.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PrimaryConstructorParameterGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					"System.Diagnostics.CodeGen.PrimaryConstructorParameterAttribute",
					static (node, _) => node is ParameterSyntax,
					transform
				)
				.Collect(),
			output
		);


		static GatheredData? transform(GeneratorAttributeSyntaxContext gasc, CancellationToken ct)
		{
			if (gasc is not
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
				})
			{
				return null;
			}

			if (!typeModifiers.Any(SyntaxKind.PartialKeyword))
			{
				return null;
			}

			var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			namespaceString = namespaceString["global::".Length..];

			// Gets the documentation comments.
			// Please note that Roslyn may contain a bug that primary constructor parameters don't contain any documentation comments,
			// so the result value of this variable may be string.Empty ("").
			var comment = parameterSymbol.GetDocumentationCommentXml(cancellationToken: ct);
			return (parameterName, typeKind, refKind, scopedKind, nullableAnnotation, parameterType, typeSymbol, isReadOnly, namespaceString, typeName, isRecord, attributesData.ToArray(), comment);
		}

		void output(SourceProductionContext spc, ImmutableArray<GatheredData?> data)
		{
			foreach (var (fileName, values) in
				from nullableTuple in data
				where nullableTuple is not null
				let tuple = nullableTuple.Value
				let fullName = $"{tuple.Namesapce}.{tuple.Type}"
				group tuple by fullName into @group
				let fileName = $"{@group.Key}.g.{Shortcuts.PrimaryConstructorParameter}.cs"
				select (FileName: fileName, Values: @group.ToArray()))
			{
				var (fieldDeclarations, propertyDeclarations) = (new List<string>(), new List<string>());
				foreach (
					var (
						parameterName, typeKind, refKind, scopedKind, nullableAnnotation, parameterType, typeSymbol, isReadOnly,
						namespaceString, typeName, isRecord, attributesData, comment
					) in values
				)
				{
					switch (attributesData)
					{
						case [{ ConstructorArguments: [{ Value: "Field" or "field" }], NamedArguments: var namedArgs }]:
						{
							var targetMemberName = getTargetMemberName(namedArgs, parameterName, "_<@");
							var accessibilityModifiers = getAccessibilityModifiers(namedArgs, "private ");
							var refModifiers = getRefModifiers(namedArgs, scopedKind, refKind, typeKind, isReadOnly, true);
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

								#pragma warning disable CS9094
								"""
								: string.Empty;
							fieldDeclarations.Add(
								$"""
								/// <summary>
									{docComments ?? $"/// The generated field declaration for parameter <c>{parameterName}</c>."}
									/// </summary>
									[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
									[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								{pragmaWarningDisable}{accessibilityModifiers}{refModifiers}{parameterTypeName}{targetMemberName} = {assigning};{pragmaWarningRestor}
								"""
							);

							break;
						}
						case [{ ConstructorArguments: [{ Value: "Property" or "property" }], NamedArguments: var namedArgs }]:
						{
							var targetMemberName = getTargetMemberName(namedArgs, parameterName, ">@");
							var accessibilityModifiers = getAccessibilityModifiers(namedArgs, "public ");
							var refModifiers = getRefModifiers(namedArgs, scopedKind, refKind, typeKind, isReadOnly, false);
							var docComments = getDocComments(comment);
							var parameterTypeString = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
							var parameterTypeName = getParameterType(parameterType, nullableAnnotation);
							var assigning = getAssigningExpression(refModifiers, parameterName);
							var memberNotNullAttribute = namedArgs.TryGetValueOrDefault<string?>("MembersNotNull", out var memberNotNullExpr)
								&& (memberNotNullExpr?.Contains(':') ?? false)
								&& memberNotNullExpr.Split(':') is [var booleanExpr, var memberNamesExpr]
								&& booleanExpr.Trim().ToCamelCasing() is var finalBooleanStringValue and ("true" or "false")
								&& (from memberName in memberNamesExpr.Split(',') select memberName.Trim()).ToArray() is var memberNamesArray and not []
								&& string.Join(", ", from memberName in memberNamesArray select $"nameof({memberName})") is var nameofExpressions
								? $"""
								[global::System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute({finalBooleanStringValue}, {nameofExpressions})]
									
								"""
								: string.Empty;

							propertyDeclarations.Add(
								$$"""
								/// <summary>
									{{docComments ?? $"/// The generated property declaration for parameter <c>{parameterName}</c>."}}
									/// </summary>
									[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
									[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
									{{memberNotNullAttribute}}{{accessibilityModifiers}}{{refModifiers}}{{parameterTypeName}}{{targetMemberName}} { get; } = {{assigning}};
								"""
							);

							break;
						}
					}
				}

				var typeKindString = values[0] switch
				{
					{ IsRecord: true, TypeKind: TypeKind.Class } => "record",
					{ IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
					{ TypeKind: TypeKind.Class } => "class",
					{ TypeKind: TypeKind.Struct } => "struct",
					{ TypeKind: TypeKind.Interface } => "interface"
				};
				var genericTypeParameters = values[0].TypeSymbol switch
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
					namespace {{values[0].Namesapce}};

					partial {{typeKindString}} {{values[0].Type}}{{genericTypeParameters}}
					{
						{{(fieldDeclarations.Count == 0 ? "// No field members." : string.Join("\r\n\r\n\t", fieldDeclarations))}}


						{{(propertyDeclarations.Count == 0 ? "// No property members." : string.Join("\r\n\r\n\t", propertyDeclarations))}}
					}
					"""
				);
			}
		}

		static string getTargetMemberName(NamedArgs namedArgs, string parameterName, string defaultPattern)
			=> namedArgs.TryGetValueOrDefault<string?>("GeneratedMemberName", out var customizedFieldName)
			&& customizedFieldName is not null
				? customizedFieldName
				: namedArgs.TryGetValueOrDefault<string?>("NamingRule", out var namingRule) && namingRule is not null
					? namingRule.InternalHandle(parameterName)
					: defaultPattern.InternalHandle(parameterName);

		static string getAccessibilityModifiers(NamedArgs namedArgs, string @default)
			=> namedArgs.TryGetValueOrDefault<string?>("Accessibility", out var a) && a is not null ? $"{a.Trim().ToLower()} " : @default;

		static string getRefModifiers(NamedArgs namedArgs, ScopedKind scopedKind, RefKind refKind, TypeKind typeKind, bool isReadOnly, bool isField)
			=> (namedArgs.TryGetValueOrDefault<string?>("RefKind", out var l) && l is not null ? $"{l} " : null)
			?? (scopedKind, refKind, typeKind, isReadOnly, isField) switch
			{
				(0, RefKind.In, TypeKind.Struct, false, _) => "readonly ref readonly ",
				(0, RefKind.In, TypeKind.Struct, true, _) => "ref readonly ",
				(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, false, _) => "readonly ref ",
				(0, RefKind.Ref or RefKind.RefReadOnly, TypeKind.Struct, true, _) => "ref ",
				(_, _, TypeKind.Struct, false, _) => "readonly ",
				(_, _, TypeKind.Struct, _, true) => "readonly ",
				_ => null
			}
			?? string.Empty;

		static string? getDocComments(string? comment)
			=> comment is null or ""
				? null
				: string.Join(
					Environment.NewLine,
					from line in comment.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					select $"/// {line.Trim()}"
				);

		static string getParameterType(ITypeSymbol parameterType, NullableAnnotation nullableAnnotation)
		{
			var r = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			return parameterType.TypeKind != TypeKind.Struct && nullableAnnotation == NullableAnnotation.Annotated
				? $"{r}? "
				: $"{r} ";
		}

		static string getAssigningExpression(string refModifiers, string parameterName)
			=> refModifiers switch { not ("" or "readonly ") => $"ref {parameterName}", _ => parameterName };
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
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

	/// <summary>
	/// Try to get the target value of the named argument collection, whose key is equal to the specified one.
	/// </summary>
	/// <typeparam name="T">The type of the target value.</typeparam>
	/// <param name="this">The collection of named arguments.</param>
	/// <param name="name">The name to be compared.</param>
	/// <param name="resultValue">The final found result value.</param>
	/// <returns>A <see cref="bool"/> result indicating whether we can use the argument <paramref name="resultValue"/>.</returns>
	public static bool TryGetValueOrDefault<T>(this NamedArgs @this, string name, out T? resultValue)
	{
		foreach (var (key, value) in @this)
		{
			if (key == name)
			{
				resultValue = (T?)value.Value;
				return true;
			}
		}

		resultValue = default;
		return false;
	}
}
