namespace Sudoku.SourceGeneration.Handlers;

internal static class AttachedPropertyHandler
{
	public static void Output(SourceProductionContext spc, ImmutableArray<CollectedResult> values)
	{
		var types = new List<string>();
		foreach (var group in values.GroupBy(static data => data.Type, (IEqualityComparer<INamedTypeSymbol>)SymbolEqualityComparer.Default))
		{
			var containingType = group.Key;
			var containingTypeStr = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var namespaceStr = containingType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var attachedProperties = new List<string>();
			var setterMethods = new List<string>();
			var staticPropertyImpls = new List<string>();
			foreach (var (_, (propertyName, propertyType, generatorMemberName, generatorMemberKind, defaultValue, callbackMethodName, isNullable)) in group)
			{
				var propertyTypeStr = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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
							/// Defines a attached property that binds with setter and getter methods <see cref="{propertyName}"/>.
							/// </summary>
							/// <seealso cref="{propertyName}"/>
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{typeof(AttachedPropertyHandler).FullName}", "{Value}")]
							public static readonly global::Microsoft.UI.Xaml.DependencyProperty {propertyName}Property =
								global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached("{propertyName}", typeof({propertyTypeStr}), typeof({containingTypeStr}), {defaultValueCreatorStr});
					"""
				);

				setterMethods.Add(
					$$"""
					/// <summary>
							/// Sets the attached property <see cref="{{propertyName}}"/> with the specified value.
							/// </summary>
							/// <param name="obj">The containing object of the property.</param>
							/// <param name="value">The value to be set.</param>
							/// <seealso cref="{{propertyName}}"/>
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(AttachedPropertyHandler).FullName}}", "{{Value}}")]
							[global::System.Diagnostics.DebuggerStepThroughAttribute]
							[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							public static void Set{{propertyName}}(global::Microsoft.UI.Xaml.DependencyObject obj, {{propertyTypeStr}}{{nullableToken}} value)
								=> obj.SetValue({{propertyName}}Property, value);

							/// <summary>
							/// Gets the attached property <see cref="{{propertyName}}"/> of its containing value.
							/// </summary>
							/// <param name="obj">The containing object of the property.</param>
							/// <returns>The value returned.</returns>
							/// <seealso cref="{{propertyName}}"/>
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(AttachedPropertyHandler).FullName}}", "{{Value}}")]
							[global::System.Diagnostics.DebuggerStepThroughAttribute]
							[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							public static {{propertyTypeStr}}{{nullableToken}} Get{{propertyName}}(global::Microsoft.UI.Xaml.DependencyObject obj)
								=> ({{propertyTypeStr}})obj.GetValue({{propertyName}}Property);
					"""
				);

				staticPropertyImpls.Add(
					$$"""
					/// <summary>
							/// Sets the property with specified value.
							/// </summary>
							/// <remarks>
							/// <b><i>This property shouldn't be used. This property only records an entry of a bound attached property.</i></b>
							/// </remarks>
							[global::System.ObsoleteAttribute("This property should not be used.", true)]
							public static partial {{propertyTypeStr}}{{nullableToken}} {{propertyName}}
							{
								[global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
								get => throw null!;

								[global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute]
								set => throw null!;
							}
					"""
				);
			}

			types.Add(
				$$"""
				namespace {{namespaceStr["global::".Length..]}}
				{
					partial class {{containingType.Name}}
					{
						//
						// Declaration of attached properties
						//
						#region Attached properties
						{{string.Join("\r\n\r\n\t\t", attachedProperties)}}
						#endregion


						//
						// Declaration of interactive methods
						//
						#region Interactive properties
						{{string.Join("\r\n\r\n\t\t", setterMethods)}}
						#endregion


						//
						// Implements on static partial entries
						//
						#region Static Properties Implementation
						{{string.Join("\r\n\r\n\t\t", staticPropertyImpls)}}
						#endregion
					}
				}
				"""
			);
		}

		spc.AddSource(
			"AttachedProperties.g.cs",
			$$"""
			{{Banner.AutoGenerated}}

			#nullable enable

			{{string.Join("\r\n\r\n", types)}}
			"""
		);
	}

	public static CollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
	{
		if (gasc is not
			{
				TargetSymbol: IPropertySymbol
				{
					ContainingType: var typeSymbol,
					Type: var propertyType,
					Name: var propertyName
				},
				Attributes: [{ NamedArguments: var namedArgs }],
				SemanticModel.Compilation: var compilation
			})
		{
			return null;
		}

		var defaultValueGenerator = default(string);
		var defaultValue = default(object);
		var callbackMethodName = default(string);
		foreach (var pair in namedArgs)
		{
			if (pair is ("DefaultValue", { Value: { } v }))
			{
				defaultValue = v;
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
			where relatedPropertyName == propertyName || $"{relatedPropertyName}?" == propertyName
			let attributesData = methodSymbol.GetAttributes()
			where attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, callbackAttribute))
			select methodName
		).FirstOrDefault();

		const string defaultValueFieldSuffix = "DefaultValue";
		var defaultValueAttribute = compilation.GetTypeByMetadataName("SudokuStudio.ComponentModel.DefaultAttribute")!;
		defaultValueGenerator ??= (
			from fieldSymbol in typeSymbol.GetMembers().OfType<IFieldSymbol>()
			where fieldSymbol.IsStatic
			let fieldName = fieldSymbol.Name
			where fieldName.EndsWith(defaultValueFieldSuffix)
			let relatedPropertyName = fieldName[..fieldName.IndexOf(defaultValueFieldSuffix)]
			where relatedPropertyName == propertyName || $"{relatedPropertyName}?" == propertyName
			let attributesData = fieldSymbol.GetAttributes()
			where attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, defaultValueAttribute))
			select fieldName
		).FirstOrDefault();

		var defaultValueGeneratorKind = default(DefaultValueGeneratingMemberKind?);
		if (defaultValueGenerator is not null)
		{
			bool e(ITypeSymbol t) => SymbolEqualityComparer.Default.Equals(t, propertyType);
			defaultValueGeneratorKind = typeSymbol.GetAllMembers().FirstOrDefault(m => m.Name == defaultValueGenerator) switch
			{
				IFieldSymbol { Type: var t, IsStatic: true } when e(t) => DefaultValueGeneratingMemberKind.Field,
				IPropertySymbol { Type: var t, IsStatic: true } when e(t) => DefaultValueGeneratingMemberKind.Property,
				IMethodSymbol { Parameters: [], ReturnType: var t, IsStatic: true } when e(t) => DefaultValueGeneratingMemberKind.ParameterlessMethod,
				null => DefaultValueGeneratingMemberKind.CannotReference,
				_ => DefaultValueGeneratingMemberKind.Otherwise
			};
		}

		if (defaultValueGeneratorKind is DefaultValueGeneratingMemberKind.CannotReference or DefaultValueGeneratingMemberKind.Otherwise)
		{
			// Invalid generator name.
			return null;
		}

		return new(
			typeSymbol,
			new(
				propertyName,
				propertyType,
				defaultValueGenerator,
				defaultValueGeneratorKind,
				defaultValue,
				callbackMethodName,
				propertyType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated }
			)
		);
	}


	/// <summary>
	/// The nesting data structure for <see cref="CollectedResult"/>.
	/// </summary>
	/// <seealso cref="CollectedResult"/>
	internal sealed record Data(
		string PropertyName,
		ITypeSymbol PropertyType,
		string? DefaultValueGeneratingMemberName,
		DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
		object? DefaultValue,
		string? CallbackMethodName,
		bool IsNullable
	);

	/// <summary>
	/// Indicates the data collected via <see cref="AttachedPropertyHandler"/>.
	/// </summary>
	/// <seealso cref="AttachedPropertyHandler"/>
	internal sealed record CollectedResult(INamedTypeSymbol Type, Data PropertiesData);
}
