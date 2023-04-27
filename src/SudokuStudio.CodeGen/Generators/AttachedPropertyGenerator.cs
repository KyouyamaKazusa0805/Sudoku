namespace Sudoku.Diagnostics.CodeGen.Generators;

using PropertyData = (string PropertyName, ITypeSymbol PropertyType, DocumentationCommentData DocumentationCommentData, string? DefaultValueGeneratingMemberName, DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind, object? DefaultValue, string? CallbackMethodName, bool IsNullable);

/// <summary>
/// Defines a generator that generates the source code for attached properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AttachedPropertyGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName("SudokuStudio.ComponentModel.AttachedPropertyAttribute`1", nodePredicate, transform)
				.Where(static data => data is not null)
				.Select(static (data, _) => data!)
				.Combine(context.CompilationProvider)
				.Where(static pair => pair.Right.AssemblyName == "SudokuStudio")
				.Select(static (pair, _) => pair.Left)
				.Collect(),
			output
		);


		static bool nodePredicate(SyntaxNode n, CancellationToken _)
			=> n is ClassDeclarationSyntax { TypeParameterList: null, Modifiers: var m and not [] }
			&& m.Any(SyntaxKind.StaticKeyword) && m.Any(SyntaxKind.PartialKeyword);

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

				var docCref = default(string);
				var docPath = default(string);
				var defaultValueGenerator = default(string);
				var defaultValue = default(object);
				var callbackMethodName = default(string);
				var docSummary = default(string);
				var docRemarks = default(string);
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

				var defaultValueGeneratorKind = default(DefaultValueGeneratingMemberKind?);
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
					(
						propertyName, propertyType, (docSummary, docRemarks, docCref, docPath),
						defaultValueGenerator, defaultValueGeneratorKind, defaultValue, callbackMethodName, isNullable
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

				var attachedProperties = new List<string>();
				var setterMethods = new List<string>();
				foreach (var (_, propertiesData) in group)
				{
					foreach (
						var (
							propertyName, propertyType, docData, generatorMemberName,
							generatorMemberKind, defaultValue, callbackMethodName, isNullable
						) in propertiesData
					)
					{
						var propertyTypeStr = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
						var doc = XamlBinding.GetDocumentationComment(propertyName, docData, false);

						var defaultValueCreatorStr = XamlBinding.GetPropertyMetadataString(defaultValue, propertyType, generatorMemberName, generatorMemberKind, callbackMethodName, propertyTypeStr);
						if (defaultValueCreatorStr is null)
						{
							// Error case has been encountered.
							continue;
						}

						var nullableToken = isNullable ? "?" : string.Empty;

						attachedProperties.Add(
							$"""
							/// <summary>
								/// Defines a attached property that binds with setter and getter methods <c>{propertyName}</c>.
								/// </summary>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								public static readonly global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property =
									global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached("{propertyName}", typeof({propertyTypeStr}), typeof({containingTypeStr}), {defaultValueCreatorStr});
							"""
						);

						setterMethods.Add(
							$$"""
							{{doc}}
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								[global::System.Diagnostics.DebuggerStepThroughAttribute]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static void Set{{propertyName}}(global::Microsoft.UI.Xaml.DependencyObject obj, {{propertyTypeStr}}{{nullableToken}} value)
									=> obj.SetValue({{propertyName}}Property, value);

								{{doc}}
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								[global::System.Diagnostics.DebuggerStepThroughAttribute]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static {{propertyTypeStr}}{{nullableToken}} Get{{propertyName}}(global::Microsoft.UI.Xaml.DependencyObject obj)
									=> ({{propertyTypeStr}})obj.GetValue({{propertyName}}Property);
							"""
						);
					}
				}

				spc.AddSource(
					$"{containingType.ToFileName()}.g.{Shortcuts.AttachedProperty}.cs",
					$$"""
					// <auto-generated />

					#nullable enable

					namespace {{namespaceStr["global::".Length..]}};

					partial class {{containingType.Name}}
					{
						//
						// Declaration of attached properties
						//
						#region Attached properties
						{{string.Join("\r\n\r\n\t", attachedProperties)}}
						#endregion


						//
						// Declaration of interactive methods
						//
						#region Interactive properties
						{{string.Join("\r\n\r\n\t", setterMethods)}}
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
/// Indicates the internal data. For more information please visit doc comments for type <see cref="PropertyData"/>.
/// </param>
/// <seealso cref="PropertyData"/>
file sealed record Data(INamedTypeSymbol Type, List<PropertyData> PropertiesData);
