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
						accessibility
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
							generatorMemberKind, defaultValue, callbackMethodName, isNullable, accessibility
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
						var accessibilityModifier = accessibility.GetName();

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

file readonly record struct Data(INamedTypeSymbol Type, List<PropertyData> PropertiesData);

/// <summary>
/// The name attribute that applies to a field in enumeration field, indicating its name.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
file sealed class NAttribute : Attribute
{
	public NAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the name of the attribute.
	/// </summary>
	public string Name { get; }
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Gets the name of the accessibility enumeration field.
	/// </summary>
	/// <param name="this">The field.</param>
	/// <returns>The name of the field.</returns>
	public static string GetName(this Accessibility @this)
		=> typeof(Accessibility).GetField(@this.ToString())!.GetCustomAttribute<NAttribute>()!.Name;
}

/// <summary>
/// Defines an accessibility kind.
/// </summary>
file enum Accessibility
{
	None,

	/// <summary>
	/// Indicates the accessibility is <see langword="file"/>-scoped.
	/// </summary>
	[N("file")]
	File,

	/// <summary>
	/// Indicates the accessibility is <see langword="private"/>.
	/// </summary>
	[N("private")]
	Private,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected"/>.
	/// </summary>
	[N("protected")]
	Protected,

	/// <summary>
	/// Indicates the accessibility is <see langword="private protected"/>.
	/// </summary>
	[N("private protected")]
	PrivateProtected,

	/// <summary>
	/// Indicates the accessibility is <see langword="internal"/>.
	/// </summary>
	[N("internal")]
	Internal,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected internal"/>.
	/// </summary>
	[N("protected internal")]
	ProtectedInternal,

	/// <summary>
	/// Indicates the accessibility is <see langword="public"/>.
	/// </summary>
	[N("public")]
	Public
}

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
file readonly record struct PropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	DocumentationCommentData DocumentationCommentData,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue,
	string? CallbackMethodName,
	bool IsNullable,
	Accessibility Accessibility
);
