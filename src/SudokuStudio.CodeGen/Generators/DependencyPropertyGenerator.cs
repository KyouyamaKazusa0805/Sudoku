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
						AttributeClass.TypeArguments: [ITypeSymbol propertyType]
					})
				{
					continue;
				}

				var docCref = (string?)null;
				var docPath = (string?)null;
				var defaultValueGenerator = (string?)null;
				var defaultValue = (object?)null;
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
						propertyName, propertyType, docCref, docPath,
						defaultValueGenerator, defaultValueGeneratorKind, defaultValue
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
					foreach (var (propertyName, propertyType, docCref, docPath, generatorMemberName, generatorMemberKind, defaultValue) in propertiesData)
					{
						var propertyTypeStr = propertyType.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints);
						var doc = (docCref, docPath) switch
						{
							(null, null) or (null, not null)
								=>
								$"""
								/// <summary>
									/// Indicates the interactive property that uses dependency property <see cref="{propertyName}Property"/> to get or set value.
									/// </summary>
									/// <seealso cref="{propertyName}Property" />
								""",
							(not null, null) => $"""/// <inheritdoc cref="{docCref}"/>""",
							(not null, not null) => $"""/// <inheritdoc cref="{docCref}" path="{docPath}"/>"""
						};

						var defaultValueCreatorStr = (defaultValue, generatorMemberName, generatorMemberKind) switch
						{
							(char c, _, _) => $", '{c}'",
							(string s, _, _) => $", \"{s}\"",
							(not null, _, _) => $", {defaultValue.ToString().ToLower()}", // true -> "True"
							(_, null, _) => string.Empty,
							(_, not null, { } kind) => kind switch
							{
								DefaultValueGeneratingMemberKind.Field or DefaultValueGeneratingMemberKind.Property => $", {generatorMemberName}",
								DefaultValueGeneratingMemberKind.ParameterlessMethod => $", {generatorMemberName}()",
								_ => null
							},
							_ => null
						};
						if (defaultValueCreatorStr is null)
						{
							// Error case has been encountered.
							continue;
						}

						dependencyProperties.Add(
							$"""
							/// <summary>
								/// Defines a denpendency property that binds with property <see cref="{propertyName}"/>.
								/// </summary>
								/// <seealso cref="{propertyName}"/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								public static readonly global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property =
									RegisterDependency<{propertyTypeStr}, {containingTypeStr}>(nameof({propertyName}){defaultValueCreatorStr});
							"""
						);

						properties.Add(
							$$"""
							{{doc}}
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								public {{propertyTypeStr}} {{propertyName}}
								{
									[global::System.Diagnostics.DebuggerStepThroughAttribute]
									[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
									get => ({{propertyTypeStr}})GetValue({{propertyName}}Property);

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
/// <param name="DocCref">Indicates the referenced member name that will be used for displaying <c>inheritdoc</c> part.</param>
/// <param name="DocPath">Indicates the referenced path that will be used for displaying <c>inheritdoc</c> part.</param>
/// <param name="DefaultValueGeneratingMemberName">
/// Indicates the referenced member name that points to a member that can create a default value of the current dependency property.
/// </param>
/// <param name="DefaultValueGeneratingMemberKind">
/// Indicates the kind of the referenced member specified as the argument <see cref="DefaultValueGeneratingMemberName"/>.
/// </param>
/// <param name="DefaultValue">Indicates the real default value.</param>
file readonly record struct PropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	string? DocCref,
	string? DocPath,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue
);

/// <summary>
/// Defines a kind of default value generating member.
/// </summary>
file enum DefaultValueGeneratingMemberKind
{
	/// <summary>
	/// Indicates the member type is a field.
	/// </summary>
	Field,

	/// <summary>
	/// Indicates the member type is a property.
	/// </summary>
	Property,

	/// <summary>
	/// Indicates the member type is a parameterless method.
	/// </summary>
	ParameterlessMethod,

	/// <summary>
	/// Indicates the member cannot be referenced.
	/// </summary>
	CannotReference,

	/// <summary>
	/// Otherwise.
	/// </summary>
	Otherwise
}
