namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a generator that generates the source code for dependency properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DependencyPropertyGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName("SudokuStudio.ComponentModel.DependencyPropertyAttribute`1", nodePredicate, transform)
				.Where(static data => data is not null)
				.Select(static (data, _) => data!)
				.Combine(context.CompilationProvider)
				.Where(static pair => pair.Right.AssemblyName == "SudokuStudio")
				.Select(static (pair, _) => pair.Left)
				.Collect(),
			output
		);


		static bool nodePredicate(SyntaxNode n, CancellationToken _)
			=> n is ClassDeclarationSyntax { TypeParameterList: null, Modifiers: var m and not [] } && m.Any(SyntaxKind.PartialKeyword);

		static Data? transform(GeneratorAttributeSyntaxContext gasc, CancellationToken ct)
		{
			if (gasc is not
				{
					TargetSymbol: INamedTypeSymbol typeSymbol,
					Attributes: var attributes,
					SemanticModel.Compilation: var compilation
				})
			{
				return null;
			}

			if (compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.DependencyObject") is not { } dependencyObjectType)
			{
				return null;
			}

			if (!typeSymbol.IsDerivedFrom(dependencyObjectType))
			{
				return null;
			}

			var propertiesData = new List<PropertyData>();
			foreach (var attributeData in attributes)
			{
				if (attributeData is not
					{
						ConstructorArguments: [{ Value: string propertyName }],
						NamedArguments: var namedArgs,
						AttributeClass.TypeArguments: [ITypeSymbol { IsReferenceType: var isReferenceType } propertyType]
					})
				{
					continue;
				}

				var docCref = (string?)null;
				var docPath = (string?)null;
				var defaultValueGenerator = (string?)null;
				var defaultValue = (object?)null;
				var callbackMethodName = (string?)null;
				var docSummary = (string?)null;
				var docRemarks = (string?)null;
				var membersNotNullWhenReturnsTrue = (string[]?)null;
				var isNullable = false;
				var accessibility = Accessibility.Public;
				foreach (var pair in namedArgs)
				{
					switch (pair)
					{
						case ("DocReferencedMemberName", { Value: string v }):
						{
							docCref = v;
							break;
						}
						case ("DocReferencedPath", { Value: string v }):
						{
							docPath = v;
							break;
						}
						case ("DefaultValueGeneratingMemberName", { Value: string v }):
						{
							defaultValueGenerator = v;
							break;
						}
						case ("DefaultValue", { Value: { } v }):
						{
							defaultValue = v;
							break;
						}
						case ("CallbackMethodName", { Value: string v }):
						{
							callbackMethodName = v;
							break;
						}
						case ("DocSummary", { Value: string v }):
						{
							docSummary = v;
							break;
						}
						case ("DocRemarks", { Value: string v }):
						{
							docRemarks = v;
							break;
						}
						case ("IsNullable", { Value: bool v }) when isReferenceType:
						{
							isNullable = v;
							break;
						}
						case ("Accessibility", { Value: int v }):
						{
							accessibility = (Accessibility)v;
							break;
						}
						case ("MembersNotNullWhenReturnsTrue", { Values: { } rawValues }):
						{
							membersNotNullWhenReturnsTrue = (from rawValue in rawValues select (string)rawValue.Value!).ToArray();
							break;
						}
					}
				}

				const string callbackMethodSuffix = "PropertyCallback";
				var callbackAttribute = compilation.GetTypeByMetadataName("SudokuStudio.ComponentModel.CallbackAttribute")!;
				callbackMethodName ??= (
					from methodSymbol in typeSymbol.GetMembers().OfType<IMethodSymbol>()
					where methodSymbol is { IsStatic: true, ReturnsVoid: true }
					let methodName = methodSymbol.Name
					where methodName.EndsWith(callbackMethodSuffix)
					let relatedPropertyName = methodName[..methodName.IndexOf(callbackMethodSuffix)]
					where relatedPropertyName == propertyName
					let attributesData = methodSymbol.GetAttributes()
					where attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, callbackAttribute))
					select methodName
				).FirstOrDefault();

				const string defaultValueFieldSuffix = "DefaultValue";
				var defaultValueAttribute = compilation.GetTypeByMetadataName("SudokuStudio.ComponentModel.DefaultValueAttribute")!;
				defaultValueGenerator ??= (
					from fieldSymbol in typeSymbol.GetMembers().OfType<IFieldSymbol>()
					where fieldSymbol.IsStatic
					let fieldName = fieldSymbol.Name
					where fieldName.EndsWith(defaultValueFieldSuffix)
					let relatedPropertyName = fieldName[..fieldName.IndexOf(defaultValueFieldSuffix)]
					where relatedPropertyName == propertyName
					let attributesData = fieldSymbol.GetAttributes()
					where attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, defaultValueAttribute))
					select fieldName
				).FirstOrDefault();

				var defaultValueGeneratorKind = (DefaultValueGeneratingMemberKind?)null;
				if (defaultValueGenerator is not null)
				{
					defaultValueGeneratorKind = typeSymbol.GetAllMembers().FirstOrDefault(m => m.Name == defaultValueGenerator) switch
					{
						IFieldSymbol { Type: var t, IsStatic: true } when e(t)
							=> DefaultValueGeneratingMemberKind.Field,
						IPropertySymbol { Type: var t, IsStatic: true } when e(t)
							=> DefaultValueGeneratingMemberKind.Property,
						IMethodSymbol { Parameters: [], ReturnType: var t, IsStatic: true } when e(t)
							=> DefaultValueGeneratingMemberKind.ParameterlessMethod,
						null
							=> DefaultValueGeneratingMemberKind.CannotReference,
						_
							=> DefaultValueGeneratingMemberKind.Otherwise
					};
				}

				if (defaultValueGeneratorKind is DefaultValueGeneratingMemberKind.CannotReference or DefaultValueGeneratingMemberKind.Otherwise)
				{
					// Invalid generator name.
					continue;
				}

				propertiesData.Add(
					new(
						propertyName,
						propertyType,
						new(docSummary, docRemarks, docCref, docPath),
						defaultValueGenerator,
						defaultValueGeneratorKind,
						defaultValue,
						callbackMethodName,
						isNullable,
						accessibility,
						membersNotNullWhenReturnsTrue
					)
				);


				bool e(ITypeSymbol t) => SymbolEqualityComparer.Default.Equals(t, propertyType);
			}

			return new(typeSymbol, propertiesData);
		}

		void output(SourceProductionContext spc, ImmutableArray<Data> data)
		{
			foreach (var group in data.GroupBy<Data, INamedTypeSymbol>(static data => data.Type, SymbolEqualityComparer.Default))
			{
				var containingType = group.Key;
				var containingTypeStr = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
				var namespaceStr = containingType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

				var dependencyProperties = new List<string>();
				var properties = new List<string>();
				foreach (var (_, propertiesData) in group)
				{
					foreach (
						var (
							propertyName, propertyType, docData, generatorMemberName,
							generatorMemberKind, defaultValue, callbackMethodName, isNullable,
							accessibility, membersNotNullWhenReturnsTrue
						) in propertiesData
					)
					{
						var propertyTypeStr = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
						var doc = XamlBinding.GetDocumentationComment(propertyName, docData, true);

						var defaultValueCreatorStr = XamlBinding.GetPropertyMetadataString(defaultValue, propertyType, generatorMemberName, generatorMemberKind, callbackMethodName, propertyTypeStr);
						if (defaultValueCreatorStr is null)
						{
							// Error case has been encountered.
							continue;
						}

						var nullableToken = isNullable ? "?" : string.Empty;
						var accessibilityModifier = accessibility.GetName();
						var memberNotNullComment = membersNotNullWhenReturnsTrue is not null ? string.Empty : "//";
						var notNullMembersStr = membersNotNullWhenReturnsTrue switch
						{
							null or [] => "new string[0]",
							_ => string.Join(", ", from element in membersNotNullWhenReturnsTrue select $"nameof({element})")
						};

						dependencyProperties.Add(
							$"""
							/// <summary>
								/// Defines a denpendency property that binds with property <see cref="{propertyName}"/>.
								/// </summary>
								/// <seealso cref="{propertyName}"/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								{accessibilityModifier} static readonly global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property =
									global::Microsoft.UI.Xaml.DependencyProperty.Register(nameof({propertyName}), typeof({propertyTypeStr}), typeof({containingTypeStr}), {defaultValueCreatorStr});
							"""
						);

						properties.Add(
							$$"""
							{{doc}}
								{{memberNotNullComment}}[global::System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute(true, {{notNullMembersStr}})]
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								{{accessibilityModifier}} {{propertyTypeStr}}{{nullableToken}} {{propertyName}}
								{
									[global::System.Diagnostics.DebuggerStepThroughAttribute]
									[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
									get => ({{propertyTypeStr}}{{nullableToken}})GetValue({{propertyName}}Property);

									[global::System.Diagnostics.DebuggerStepThroughAttribute]
									[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
									set => SetValue({{propertyName}}Property, value);
								}
							"""
						);
					}
				}

				spc.AddSource(
					$"{containingType.ToFileName()}.g.{Shortcuts.DependencyProperty}.cs",
					$$"""
					// <auto-generated />

					#nullable enable

					namespace {{namespaceStr["global::".Length..]}};

					partial class {{containingType.Name}}
					{
						//
						// Declaration of dependency properties
						//
						#region Dependency properties
						{{string.Join("\r\n\r\n\t", dependencyProperties)}}
						#endregion


						//
						// Declaration of interative properties
						//
						#region Interative properties
						{{string.Join("\r\n\r\n\t", properties)}}
						#endregion
					}
					"""
				);
			}
		}
	}
}

/// <summary>
/// The gathered data. This type is only used for describing the details of gathered result.
/// </summary>
/// <param name="Type">The type symbol whose corresponding type is one where attributes declared.</param>
/// <param name="PropertiesData">
/// Indicates the internal data. For more information please visit doc commnets for type <see cref="PropertyData"/>.
/// </param>
/// <seealso cref="PropertyData"/>
file sealed record Data(INamedTypeSymbol Type, List<PropertyData> PropertiesData);

/// <summary>
/// Indicates the property generating data.
/// </summary>
/// <param name="PropertyName">Indicates the property name.</param>
/// <param name="PropertyType">Indicates the property type.</param>
/// <param name="DocumentationCommentData">The documentation data.</param>
/// <param name="DefaultValueGeneratingMemberName">
/// Indicates the referenced member name that points to a member that can create a default value of the current dependency property.
/// </param>
/// <param name="DefaultValueGeneratingMemberKind">
/// Indicates the kind of the referenced member specified as the argument <see cref="DefaultValueGeneratingMemberName"/>.
/// </param>
/// <param name="DefaultValue">Indicates the real default value.</param>
/// <param name="CallbackMethodName">Indicates the callback method name.</param>
/// <param name="IsNullable">Indicates whether the source generator will emit nullable token for reference typed properties.</param>
/// <param name="Accessibility">The accessibility.</param>
/// <param name="MembersNotNullWhenReturnsTrue">
/// Indicates the member names that won't be <see langword="null"/> if the dependency property returns <see langword="true"/>.
/// </param>
file sealed record PropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	DocumentationCommentData DocumentationCommentData,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue,
	string? CallbackMethodName,
	bool IsNullable,
	Accessibility Accessibility,
	string[]? MembersNotNullWhenReturnsTrue
);
