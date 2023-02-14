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
				.Select(static (data, _) => data!.Value)
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
				var isNullable = false;
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
					}
				}

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
						isNullable
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
				var containingTypeStr = containingType.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints);
				var namespaceStr = containingType.ContainingNamespace.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints);

				var dependencyProperties = new List<string>();
				var properties = new List<string>();
				foreach (var (_, propertiesData) in group)
				{
					foreach (
						var (
							propertyName, propertyType, docData, generatorMemberName,
							generatorMemberKind, defaultValue, callbackMethodName, isNullable
						) in propertiesData
					)
					{
						var propertyTypeStr = propertyType.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints);
						var doc = XamlBinding.GetDocumentationComment(propertyName, docData, true);

						var defaultValueCreatorStr = XamlBinding.GetPropertyMetadataString(defaultValue, generatorMemberName, generatorMemberKind, callbackMethodName, propertyTypeStr);
						if (defaultValueCreatorStr is null)
						{
							// Error case has been encountered.
							continue;
						}

						var nullableToken = isNullable ? "?" : string.Empty;

						dependencyProperties.Add(
							$"""
							/// <summary>
								/// Defines a denpendency property that binds with property <see cref="{propertyName}"/>.
								/// </summary>
								/// <seealso cref="{propertyName}"/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								public static readonly global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property =
									global::Microsoft.UI.Xaml.DependencyProperty.Register(nameof({propertyName}), typeof({propertyTypeStr}), typeof({containingTypeStr}), {defaultValueCreatorStr});
							"""
						);

						properties.Add(
							$$"""
							{{doc}}
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								public {{propertyTypeStr}}{{nullableToken}} {{propertyName}}
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

file readonly record struct Data(INamedTypeSymbol Type, List<PropertyData> PropertiesData);

/// <summary>
/// Indicates the property generating data.
/// </summary>
/// <param name="PropertyName">Indicates the property name.</param>
/// <param name="PropertyType">Indicates the property type.</param>
/// <param name="DocumentationCommentData">The documentation data.</param>
/// 
/// <param name="DefaultValueGeneratingMemberName">
/// Indicates the referenced member name that points to a member that can create a default value of the current dependency property.
/// </param>
/// <param name="DefaultValueGeneratingMemberKind">
/// Indicates the kind of the referenced member specified as the argument <see cref="DefaultValueGeneratingMemberName"/>.
/// </param>
/// <param name="DefaultValue">Indicates the real default value.</param>
/// <param name="CallbackMethodName">Indicates the callback method name.</param>
/// <param name="IsNullable">Indicates whether the source generator will emit nullable token for reference typed properties.</param>
file readonly record struct PropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	DocumentationCommentData DocumentationCommentData,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue,
	string? CallbackMethodName,
	bool IsNullable
);
