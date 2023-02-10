namespace Sudoku.Diagnostics.CodeGen.Generators;

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
				.ForAttributeWithMetadataName("System.Diagnostics.CodeGen.AttachedPropertyAttribute`1", nodePredicate, transform)
				.Where(static data => data is not null)
				.Select(static (data, _) => data!.Value)
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
						AttributeClass.TypeArguments: [ITypeSymbol propertyType]
					})
				{
					continue;
				}

				var docCref = (string?)null;
				var docPath = (string?)null;
				var defaultValueGenerator = (string?)null;
				var defaultValue = (object?)null;
				var callbackMethodName = (string?)null;
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
						defaultValueGenerator, defaultValueGeneratorKind, defaultValue, callbackMethodName
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

				var attachedProperties = new List<string>();
				var setterMethods = new List<string>();
				foreach (var (_, propertiesData) in group)
				{
					foreach (
						var (
							propertyName, propertyType, docCref, docPath, generatorMemberName,
							generatorMemberKind, defaultValue, callbackMethodName
						) in propertiesData
					)
					{
						var propertyTypeStr = propertyType.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints);
						var doc = (docCref, docPath) switch
						{
							(null, null) or (null, not null)
								=>
								$"""
								/// <summary>
									/// Indicates the interactive setter or getter methods that uses attached property <see cref="{propertyName}Property"/> to get or set value.
									/// </summary>
									/// <seealso cref="{propertyName}Property" />
								""",
							(not null, null) => $"""/// <inheritdoc cref="{docCref}"/>""",
							(not null, not null) => $"""/// <inheritdoc cref="{docCref}" path="{docPath}"/>"""
						};

						var defaultValueCreatorStr = (defaultValue, generatorMemberName, generatorMemberKind, callbackMethodName) switch
						{
							(char c, _, _, null) => $", new('{c}')",
							(char c, _, _, _) => $", new('{c}', {callbackMethodName})",
							(string s, _, _, null) => $", new(\"{s}\")",
							(string s, _, _, _) => $", new(\"{s}\", {callbackMethodName})",
							(not null, _, _, null) => $", new({defaultValue.ToString().ToLower()})", // true -> "True"
							(not null, _, _, _) => $", new({defaultValue.ToString().ToLower()}, {callbackMethodName})", // true -> "True"
							(_, null, _, null) => string.Empty,
							(_, null, _, _) => $", new(default({propertyTypeStr}), {callbackMethodName})",
							(_, not null, { } kind, _) => kind switch
							{
								DefaultValueGeneratingMemberKind.Field or DefaultValueGeneratingMemberKind.Property
									=> callbackMethodName switch
									{
										null => $", new({generatorMemberName})",
										_ => $", new({generatorMemberName}, {callbackMethodName})"
									},
								DefaultValueGeneratingMemberKind.ParameterlessMethod
									=> callbackMethodName switch
									{
										null => $", new({generatorMemberName}())",
										_ => $", new({generatorMemberName}(), {callbackMethodName})"
									},
								_ => null
							},
							_ => null
						};
						if (defaultValueCreatorStr is null)
						{
							// Error case has been encountered.
							continue;
						}

						attachedProperties.Add(
							$"""
							/// <summary>
								/// Defines a attached property that binds with setter and getter methods <c>{propertyName}</c>.
								/// </summary>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								public static readonly global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property =
									global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached("{propertyName}", typeof({propertyTypeStr}), typeof({containingTypeStr}){defaultValueCreatorStr});
							"""
						);

						setterMethods.Add(
							$$"""
							{{doc}}
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								[global::System.Diagnostics.DebuggerStepThroughAttribute]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static void Set{{propertyName}}(global::Microsoft.UI.Xaml.DependencyObject obj, {{propertyTypeStr}} value)
									=> obj.SetValue({{propertyName}}Property, value);

								{{doc}}
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								[global::System.Diagnostics.DebuggerStepThroughAttribute]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static {{propertyTypeStr}} Get{{propertyName}}(global::Microsoft.UI.Xaml.DependencyObject obj)
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
						// Declaration of interative methods
						//
						#region Interative properties
						{{string.Join("\r\n\r\n\t", setterMethods)}}
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
/// <param name="CallbackMethodName">The callback method name.</param>
file readonly record struct PropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	string? DocCref,
	string? DocPath,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue,
	string? CallbackMethodName
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
